using Azure;
using Azure.Search.Documents.Indexes;
using EmailPlugin.Models;
using EmailService.Contracts;
using EmailService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Embeddings;

namespace EmailApi
{
    public class Program
    {
        const string allowAll = "AllowAll";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add configuration for appsettings
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();
            builder.Services.AddSingleton<IConfiguration>(config);

            // Add services to the container.
            AddAiServicesAsync(builder);
            builder.Services.AddSingleton<IOutlookService, OutlookService>();

            // Configure cors.
            ConfigureCors(builder);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(allowAll);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static void AddAiServicesAsync(WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            // Connect to text embedding model.
            ITextEmbeddingGenerationService textEmbeddingService = new AzureOpenAITextEmbeddingGenerationService(
                configuration["AzureTextEmbedding:DeploymentName"],
                configuration["AzureTextEmbedding:Endpoint"],
                configuration["AzureTextEmbedding:Key"]);
            builder.Services.AddSingleton<ITextEmbeddingGenerationService>(textEmbeddingService);

            // Connect to vector store.
            var vectorStore = new AzureAISearchVectorStore(
                new SearchIndexClient(
                    new Uri(configuration["AzureAiSearch:Url"]),
                    new AzureKeyCredential(configuration["AzureAiSearch:Key"])));
            builder.Services.AddSingleton<IVectorStore>(vectorStore);

            // Choose a collection from the database and specify the type of key and record stored in it via Generic parameters.
            var collection = vectorStore.GetCollection<string, Email>(configuration["AzureAiSearch:EmailCollection"]);
            // Create the collection if it doesn't exist yet.
            //await collection.CreateCollectionIfNotExistsAsync();
            builder.Services.AddSingleton<IVectorStoreRecordCollection<string, Email>>(collection);
            return;
        }

        /// <summary>
        /// Configures CORS to allow all origins, headers, and methods.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        private static void ConfigureCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(setUpAction =>
            {
                setUpAction.AddPolicy(allowAll, policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });
        }
    }
}
