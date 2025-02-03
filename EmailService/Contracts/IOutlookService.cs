using EmailPlugin.Models;

namespace EmailService.Contracts
{
    public interface IOutlookService
    {
        Task AddEmailAsync(Email email);
        Task GenerateEmbeddingsAndUpsertAsync(int count = int.MaxValue);
        Task<List<Email>> GetMailsFromOutlook(int count = int.MaxValue);
        Task ReplyToEmailAsync(Email email);
    }
}