﻿using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;

namespace SemanticRag
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();

            var configuration = builder.Build();

            // Connect to text embedding model.
            var textEmbeddingService = new AzureOpenAITextEmbeddingGenerationService(
                configuration["AzureTextEmbedding:DeploymentName"],
                configuration["AzureTextEmbedding:Endpoint"],
                configuration["AzureTextEmbedding:Key"]);

            // Connect to vector store.
            var vectorStore = new AzureAISearchVectorStore(
                new SearchIndexClient(
                    new Uri(configuration["AzureAiSearch:Url"]),
                    new AzureKeyCredential(configuration["AzureAiSearch:Key"])));

            // Choose a collection from the database and specify the type of key and record stored in it via Generic parameters.
            var collection = vectorStore.GetCollection<string, Hotel>("skhotels");

            // Create the collection if it doesn't exist yet.
            await collection.CreateCollectionIfNotExistsAsync();


            await GenerateEmbeddingsAndUpsertAsync(textEmbeddingService, collection);
            // Azure AI Search takes a while to update.
            Thread.Sleep(5 * 1000);
            await GenerateEmbeddingsAndSearchAsync(textEmbeddingService, collection);

            await TextSearchWithVectorStore(textEmbeddingService, collection);
        }

        private static async Task TextSearchWithVectorStore(AzureOpenAITextEmbeddingGenerationService textEmbeddingService, IVectorStoreRecordCollection<string, Hotel> collection)
        {
            // Create a text search instance using the vector store record collection.
            var textSearch = new VectorStoreTextSearch<Hotel>(collection, textEmbeddingService);

            // Search and return results as TextSearchResult items
            var query = "Top hotels?";
            KernelSearchResults<TextSearchResult> textResults = await textSearch.GetTextSearchResultsAsync(query, new() { Top = 2, Skip = 0 });
            Console.WriteLine("\n--- Text Search Results ---\n");
            await foreach (TextSearchResult result in textResults.Results)
            {
                Console.WriteLine($"Name:  {result.Name}");
                Console.WriteLine($"Value: {result.Value}");
            }
        }

        public static async Task GenerateEmbeddingsAndUpsertAsync(ITextEmbeddingGenerationService textEmbeddingGenerationService, IVectorStoreRecordCollection<string, Hotel> collection)
        {
            // Upsert a record.
            string descriptionText = "A place where everyone can be happy.";
            string hotelId = "1";

            // Generate the embedding.
            ReadOnlyMemory<float> embedding =
                await textEmbeddingGenerationService.GenerateEmbeddingAsync(descriptionText);

            // Create a record and upsert with the already generated embedding.
            await collection.UpsertAsync(new Hotel
            {
                HotelId = hotelId,
                HotelName = "Hotel Happy",
                Description = descriptionText,
                DescriptionEmbedding = embedding,
                Tags = new[] { "luxury", "pool" }
            });
        }

        public static async Task GenerateEmbeddingsAndSearchAsync(ITextEmbeddingGenerationService textEmbeddingGenerationService, IVectorStoreRecordCollection<string, Hotel> collection)
        {
            // Upsert a record.
            string descriptionText = "Find me a hotel with happiness in mind.";

            // Generate the embedding.
            ReadOnlyMemory<float> searchEmbedding =
                await textEmbeddingGenerationService.GenerateEmbeddingAsync(descriptionText);

            // Do the search, passing an options object with a Top value to limit resulst to the single top match.
            var searchResult = await collection.VectorizedSearchAsync(searchEmbedding, new() { Top = 1 });

            // Inspect the returned hotel.
            await foreach (var record in searchResult.Results)
            {
                Console.WriteLine("Found hotel description: " + record.Record.Description);
                Console.WriteLine("Found record score: " + record.Score);
            }
        }

    }
}
