using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.AbandonedCartReminder.Domain;
using Nop.Services.Customers;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Services;

public class AbandonedCartReminderService : IAbandonedCartReminderService
{
    private readonly IRepository<ShoppingCartItem> _shoppingCartItemRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<MessageTemplate> _messageTemplateRepository;
    private readonly IRepository<AbandonedCartReminderRecord> _reminderRecordRepository;
    private readonly IStoreContext _storeContext;
    private readonly ICustomerService _customerService;
    private readonly AbandonedCartReminderSettings _settings;
    private readonly IQueuedEmailService _queuedEmailService;
    private readonly IEmailAccountService _emailAccountService;

    public AbandonedCartReminderService(
        IRepository<ShoppingCartItem> shoppingCartItemRepository,
        IRepository<Customer> customerRepository,
        IRepository<MessageTemplate> messageTemplateRepository,
        IRepository<AbandonedCartReminderRecord> reminderRecordRepository,
        IStoreContext storeContext,
        ICustomerService customerService,
        AbandonedCartReminderSettings settings,
        IQueuedEmailService queuedEmailService,
        IEmailAccountService emailAccountService)
    {
        _shoppingCartItemRepository = shoppingCartItemRepository;
        _customerRepository = customerRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _reminderRecordRepository = reminderRecordRepository;
        _storeContext = storeContext;
        _customerService = customerService;
        _settings = settings;
        _queuedEmailService = queuedEmailService;
        _emailAccountService = emailAccountService;
    }

    private bool HasReminderAlreadyBeenSent(
        IList<AbandonedCartReminderRecord> records,
        int customerId,
        int storeId)
    {
        return records.Any(r =>
            r.CustomerId == customerId &&
            r.StoreId == storeId &&
            r.ReminderSentOnUtc.HasValue &&
            !r.IsRecovered);
    }

    public async Task<IList<Customer>> GetEligibleCustomersAsync()
    {
        if (!_settings.Enabled)
            return new List<Customer>();

        var currentStore = await _storeContext.GetCurrentStoreAsync();
        var thresholdUtc = DateTime.UtcNow.AddHours(-_settings.SendAfterHours);

        var cartItems = _shoppingCartItemRepository.Table
            .Where(sci =>
                sci.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart &&
                sci.StoreId == currentStore.Id &&
                sci.Quantity > 0 &&
                sci.CreatedOnUtc <= thresholdUtc)
            .ToList();

        if (!cartItems.Any())
            return new List<Customer>();

        var customerIds = cartItems
            .Select(x => x.CustomerId)
            .Distinct()
            .ToList();

        var customers = _customerRepository.Table
            .Where(c => customerIds.Contains(c.Id))
            .ToList();

        var existingRecords = _reminderRecordRepository.Table
            .Where(r => r.StoreId == currentStore.Id && customerIds.Contains(r.CustomerId))
            .ToList();

        var result = new List<Customer>();

        foreach (var customer in customers)
        {
            if (customer.Deleted || !customer.Active)
                continue;

            if (string.IsNullOrWhiteSpace(customer.Email))
                continue;

            var customerCartItems = cartItems
                .Where(x => x.CustomerId == customer.Id)
                .ToList();

            if (!customerCartItems.Any())
                continue;

            // Admin kullanıcıları hariç tut
            if (await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName))
                continue;

            if (HasReminderAlreadyBeenSent(existingRecords, customer.Id, currentStore.Id))
                continue;

            result.Add(customer);
        }

        return result;
    }

    public async Task SendReminderAsync(Customer customer)
    {
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        // Admin kullanıcıya asla reminder gönderme
        if (await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.AdministratorsRoleName))
            return;

        var existingRecord = _reminderRecordRepository.Table
            .Where(r =>
                r.CustomerId == customer.Id &&
                r.StoreId == currentStore.Id &&
                r.ReminderSentOnUtc.HasValue &&
                !r.IsRecovered)
            .OrderByDescending(r => r.Id)
            .FirstOrDefault();

        if (existingRecord != null)
            return;

        var customerCartItems = _shoppingCartItemRepository.Table
            .Where(sci =>
                sci.CustomerId == customer.Id &&
                sci.StoreId == currentStore.Id &&
                sci.ShoppingCartTypeId == (int)ShoppingCartType.ShoppingCart &&
                sci.Quantity > 0)
            .ToList();

        if (!customerCartItems.Any())
            return;

        var cartLastUpdatedOnUtc = customerCartItems.Max(x => x.UpdatedOnUtc);

        var emailAccount = (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
        if (emailAccount == null)
            return;

        var template = _messageTemplateRepository.Table
            .FirstOrDefault(x => x.Name == "AbandonedCartReminder" && x.IsActive);

        if (template == null)
            return;

        var body = template.Body?.Replace("%Store.Name%", "Your store name");

        await _queuedEmailService.InsertQueuedEmailAsync(new QueuedEmail
        {
            Priority = QueuedEmailPriority.High,
            From = emailAccount.Email,
            FromName = emailAccount.DisplayName,
            To = customer.Email,
            ToName = customer.Email,
            Subject = template.Subject,
            Body = body,
            CreatedOnUtc = DateTime.UtcNow,
            EmailAccountId = emailAccount.Id
        });

        await _reminderRecordRepository.InsertAsync(new AbandonedCartReminderRecord
        {
            CustomerId = customer.Id,
            StoreId = currentStore.Id,
            CartLastUpdatedOnUtc = cartLastUpdatedOnUtc,
            ReminderSentOnUtc = DateTime.UtcNow,
            IsRecovered = false,
            CartSnapshot = null
        });
    }

    public async Task SendReminderEmailsAsync()
    {
        if (!_settings.Enabled)
            return;

        var customers = await GetEligibleCustomersAsync();

        Console.WriteLine($"=== Eligible customer count: {customers.Count} ===");

        foreach (var customer in customers)
        {
            await SendReminderAsync(customer);
            Console.WriteLine($"=== Mail queued for: {customer.Email} ===");
        }
    }
}