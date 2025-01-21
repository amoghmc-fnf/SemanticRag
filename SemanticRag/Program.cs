using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Embeddings;

namespace SemanticRag
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();

            var configuration = builder.Build();

            // Connect to text embedding model.
            var textEmbeddingService = new AzureOpenAITextEmbeddingGenerationService(
                configuration["AzureTextEmbedding:DeploymentName"],
                configuration["AzureTextEmbedding:Endpoint"],
                configuration["AzureTextEmbedding:Key"]);

            // Connect to vector store
            var vectorStore = new AzureAISearchVectorStore(
                new SearchIndexClient(
                    new Uri(configuration["AzureAiSearch:Url"]),
                    new AzureKeyCredential(configuration["AzureAiSearch:Key"])));
        }
    }
}
