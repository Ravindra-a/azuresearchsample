using System;
using System.Threading;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using System.Linq;

namespace ImportData
{
    class Program
    {
        private static readonly string searchServiceName = "<Azure Service name>";
        private static readonly string adminAPIKey = "Azure Admin Key";
        static void Main(string[] args)
        {
            var actions =
            new IndexAction<Hotel>[]
            {
                IndexAction.Upload(
                    new Hotel()
                    {
                        HotelId = "1",
                        BaseRate = 199.0,
                        Description = "Best hotel in town",
                        DescriptionFr = "Meilleur hôtel en ville",
                        HotelName = "Fancy Stay",
                        Category = "Luxury",
                        Tags = new[] { "pool", "view", "wifi", "concierge" },
                        ParkingIncluded = false,
                        SmokingAllowed = false,
                        LastRenovationDate = new DateTimeOffset(2010, 6, 27, 0, 0, 0, TimeSpan.Zero),
                        Rating = 5,
                        Location = GeographyPoint.Create(47.678581, -122.131577)
                    }),
                IndexAction.Upload(
                    new Hotel()
                    {
                        HotelId = "2",
                        BaseRate = 79.99,
                        Description = "Cheapest hotel in town",
                        DescriptionFr = "Hôtel le moins cher en ville",
                        HotelName = "Roach Motel",
                        Category = "Budget",
                        Tags = new[] { "motel", "budget" },
                        ParkingIncluded = true,
                        SmokingAllowed = true,
                        LastRenovationDate = new DateTimeOffset(1982, 4, 28, 0, 0, 0, TimeSpan.Zero),
                        Rating = 1,
                        Location = GeographyPoint.Create(49.678581, -122.131577)
                    }),
                IndexAction.MergeOrUpload(
                    new Hotel()
                    {
                        HotelId = "3",
                        BaseRate = 129.99,
                        Description = "Close to town hall and the river"
                    }),
                //IndexAction.Delete(new Hotel() { HotelId = "6" })
            };

            var batch = IndexBatch.New(actions);

            var searchServiceClient = CreateSearchServiceClient();
            var indexClient = CreateSearchIndexClient(searchServiceClient);

            try
            {
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Waiting for documents to be indexed...\n");
            Thread.Sleep(2000);
            Console.WriteLine("Documents indexed...\n");
            Console.ReadKey();
        }

        /// <summary>
        /// Step 1 Create an instance of the SearchIndexClient class
        /// </summary>
        /// <returns></returns>
        private static ISearchIndexClient CreateSearchIndexClient(SearchServiceClient serviceClient)
        {
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("hotels1");
            return indexClient;
        }

        /// <summary>
        /// Create an instance of the SearchServiceClient class
        /// </summary>
        /// <returns></returns>
        private static SearchServiceClient CreateSearchServiceClient()
        {
            SearchServiceClient searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminAPIKey));
            return searchServiceClient;
        }
    }
}
