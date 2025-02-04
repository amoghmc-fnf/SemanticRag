using EmailPlugin.Models;
using EmailService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutlookController : ControllerBase
    {
        private readonly IOutlookService _outlookService;

        public OutlookController(IOutlookService outlookService)
        {
            _outlookService = outlookService;
        }

        [HttpGet("emails")]
        public async Task<IActionResult> GetMailsFromOutlook(int count = int.MaxValue)
        {
            var emails = await _outlookService.GetMailsFromOutlook(count);
            return Ok(emails);
        }

        [HttpPost("generate-embeddings")]
        public async Task<IActionResult> GenerateEmbeddingsAndUpsertAsync(int count = int.MaxValue)
        {
            await _outlookService.GenerateEmbeddingsAndUpsertAsync(count);
            return Ok();
        }

        [HttpGet("search-embeddings")]
        public async Task<ActionResult<List<Email>>> SearchEmails(string query, int top = 1, int skip = 0)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query cannot be empty.");
            }
            var emails = await _outlookService.GenerateEmbeddingsAndSearchAsync(query, top, skip);
            return Ok(emails);
        }

        [HttpPost("add-email")]
        public async Task<IActionResult> AddEmailAsync([FromBody] Email email)
        {
            await _outlookService.AddEmailAsync(email);
            return Ok();
        }

        [HttpPost("reply-email")]
        public async Task<IActionResult> ReplyToEmailAsync([FromBody] Email email)
        {
            await _outlookService.ReplyToEmailAsync(email);
            return Ok();
        }
    }
}
