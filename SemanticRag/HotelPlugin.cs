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
            KernelSearchResults<object> textResults = await _hotels.GetSearchResultsAsync(query);
            await foreach (Hotel result in textResults.Results)
            {
                Hotel h = new Hotel();
                h.HotelName = result.HotelName;
                h.Description = result.Description;
                hotels.Add(h);
            }
            return hotels;

        }
        


    }
}
