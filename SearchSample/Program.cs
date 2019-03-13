using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace SearchSample
{
    class Program
    {
        //Step 1 Identify your Azure Search service's admin api-key
        private static readonly string searchServiceName = "<Azure Service name>";
        private static readonly string adminAPIKey = "Azure Admin Key";
        static void Main(string[] args)
        {
            try
            {
                //Step 2 Create an instance of the SearchServiceClient class
                var searchServiceClient = CreateSearchServiceClient();
                CreateIndex(searchServiceClient);
                Console.WriteLine("********* Azure search Index created successfully *************");                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Step 2 Create an instance of the SearchServiceClient class
        /// </summary>
        /// <returns></returns>
        private static SearchServiceClient CreateSearchServiceClient()
        {
            SearchServiceClient searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminAPIKey));
            return searchServiceClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchServiceClient"></param>
        private static void CreateIndex(SearchServiceClient searchServiceClient)
        {
            // Step 3 Define your Azure Search index
            var index = new Index()
            {
                Name = "hotels1",
                Fields = FieldBuilder.BuildForType<Hotel>()
            };

            // Step 4 Create index
            searchServiceClient.Indexes.Create(index);
        }
    }
}
