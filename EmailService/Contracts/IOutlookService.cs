using EmailPlugin.Models;

namespace EmailService.Contracts
{
    public interface IOutlookService
    {
        Task AddEmailAsync(Email email);
        Task GenerateEmbeddingsAndUpsertAsync(int count = int.MaxValue);
        Task<List<Email>> GenerateEmbeddingsAndSearchAsync(string query, int top = 1, int skip = 0);
        Task<List<Email>> GetMailsFromOutlook(int count = int.MaxValue);
        Task ReplyToEmailAsync(Email email);
    }
}