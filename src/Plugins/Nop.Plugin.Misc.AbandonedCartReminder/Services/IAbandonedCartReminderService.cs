using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.AbandonedCartReminder.Services;

public interface IAbandonedCartReminderService
{
    Task<IList<Customer>> GetEligibleCustomersAsync();
    Task SendReminderAsync(Customer customer);
    Task SendReminderEmailsAsync(); 
}