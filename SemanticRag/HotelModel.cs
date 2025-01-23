using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

public class Hotel
{
    [VectorStoreRecordKey]
    public string HotelId { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    [TextSearchResultName]
    public string HotelName { get; set; }

    [TextSearchResultValue]
    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string Description { get; set; }

    [VectorStoreRecordVector(Dimensions: 1536)]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string[] Tags { get; set; }
}