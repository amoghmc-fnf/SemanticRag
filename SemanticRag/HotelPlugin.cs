using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticRag
{
    public class HotelPlugin 
    {
        private VectorStoreTextSearch<Hotel> _hotels;
        public HotelPlugin(VectorStoreTextSearch<Hotel> hotels)
        {
            _hotels = hotels;
        }

        [KernelFunction("get_all_hotels")]
        [Description("Gets all hotels in the collection")]
        public async Task<List<Hotel>> GetAllHotels()
        {
            List<Hotel> hotels = new();
            var query = "get all hotels";
            KernelSearchResults<TextSearchResult> textResults = await _hotels.GetTextSearchResultsAsync(query);
            await foreach (TextSearchResult result in textResults.Results)
            {
                Hotel h = new Hotel();
                h.HotelName = "1" + result.Name;
                h.Description = result.Value;
                hotels.Add(h);
            }
            return hotels;

        }
        


    }
}
