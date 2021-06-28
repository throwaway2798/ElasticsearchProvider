using ElasticsearchProvider;

using ElasticsearchProvider.DataStructures;

using Nest;

using Newtonsoft.Json.Linq;

using System;

using System.Collections.Generic;


namespace ElasticsearchProviderExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SearchProvider provider;


            provider = new SearchProvider();

            provider.Index("mgmt.json", true, new MgmtContainerEqualityComparer());

            provider.Index("properties.json", true, new PropertyContainerEqualityComparer());


            //property:name
            //Lullwater at Riverwood Phase II

            Search(provider, "lullwAterr the riverwood PHASE", new List<string>() { "Augusta", "Austin" });


            Console.SetCursorPosition(0, 0);

            Console.ReadLine();

            Console.Clear();


            //property:streetAddress
            //15835 Foothill Farms Loop

            Search(provider, "15835 FOOThill ", new List<string>() { });


            Console.SetCursorPosition(0, 0);

            Console.ReadLine();

            Console.Clear();


            //mgmt:name
            //Essex Property Trust AKA Essex Apartment Homes

            Search(provider, "essex Property trU apartment", new List<string>() { });


            Console.SetCursorPosition(0, 0);

            Console.ReadLine();

            Console.Clear();


            //TX

            Search(provider, "tX", new List<string>() { });


            Console.SetCursorPosition(0, 0);

            Console.ReadLine();

            Console.Clear();

            Console.ReadLine();
        }


        private static void Display(MgmtContainer container)
        {
            Console.WriteLine(new string('-', Console.BufferWidth));

            Console.WriteLine($"Name: {container.Mgmt.Name}");

            Console.WriteLine($"Market: {container.Mgmt.Market}");

            Console.WriteLine($"State: {container.Mgmt.State}");
        }

        private static void Display(PropertyContainer container)
        {
            Console.WriteLine(new string('-', Console.BufferWidth));

            Console.WriteLine($"Name: {container.Property.Name}");

            Console.WriteLine($"FormerName: {container.Property.FormerName}");

            Console.WriteLine($"StreetAddress: {container.Property.StreetAddress}");

            Console.WriteLine($"City: {container.Property.City}");

            Console.WriteLine($"Market: {container.Property.Market}");

            Console.WriteLine($"State: {container.Property.State}");
        }


        private static void DisplayResponse(ISearchResponse<JObject> response)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine($"ResponseCount: {response.Documents.Count}");

            Console.BackgroundColor = ConsoleColor.Black;


            foreach (JObject document in response.Documents)
            {
                if (document.ContainsKey("mgmt"))
                {
                    Display(document.ToObject<MgmtContainer>());
                }
                else if (document.ContainsKey("property"))
                {
                    Display(document.ToObject<PropertyContainer>());
                }
            }
        }


        private static void Search(SearchProvider provider, string phase, List<string> markets = null, int maxResponseCount = 25)
        {
            ISearchResponse<JObject> response;


            Console.BackgroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine($"SearchPhase: {phase}; SearchMarkets: {(markets != null ? string.Join(", ", markets) : string.Empty)}");

            Console.BackgroundColor = ConsoleColor.Black;


            response = provider.Search(phase, markets, maxResponseCount);


            DisplayResponse(response);
        }
    }
}
