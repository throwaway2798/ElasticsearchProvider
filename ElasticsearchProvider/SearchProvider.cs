using Elasticsearch.Net;

using ElasticsearchProvider.DataStructures;

using Nest;

using Nest.JsonNetSerializer;

using Newtonsoft.Json.Linq;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace ElasticsearchProvider
{
    public class SearchProvider
    {
        private IElasticClient client;


        public SearchProvider() : this(new List<Uri>() { new Uri("http://localhost:9200") }) 
        {
        }

        public SearchProvider(List<Uri> uris)
        {
            string defaultIndex;


            ConnectionSettings settings;


            defaultIndex = "default";


            settings = new ConnectionSettings(new SniffingConnectionPool(uris, false), JsonNetSerializer.Default);

            settings.DefaultIndex(defaultIndex);

            settings.DefaultMappingFor<MgmtContainer>(ctmd => ctmd.IndexName(MgmtContainer.Index));

            settings.DefaultMappingFor<PropertyContainer>(ctmd => ctmd.IndexName(PropertyContainer.Index));


            client = new ElasticClient(settings);


            #region "Cleanup"

            if (client.Indices.Exists(defaultIndex).Exists)
            {
                client.Indices.Delete(defaultIndex);
            }

            if (client.Indices.Exists(MgmtContainer.Index).Exists)
            {
                client.Indices.Delete(MgmtContainer.Index);
            }

            if (client.Indices.Exists(PropertyContainer.Index).Exists)
            {
                client.Indices.Delete(PropertyContainer.Index);
            }

            #endregion


            MgmtContainer.Create(client);


            PropertyContainer.Create(client);
        }


        public BulkResponse Index<T>(string path, bool removeDuplicates, IEqualityComparer<T> comparer) where T : class
        {
            FileStream stream;


            IEnumerable<T> values;


            stream = File.OpenRead(path);

            using (stream)
            {
                values = client.SourceSerializer.Deserialize<T[]>(stream);


                if (removeDuplicates)
                {
                    values = values.Distinct(comparer);
                }


                return client.IndexMany(values);
            }
        }


        public ISearchResponse<JObject> Search(string phase, List<string> markets = null, int maxResponseCount = 25)
        {
            if (markets == null)
            {
                markets = new List<string>();
            }
            else
            {
                for (int i = 0; i < markets.Count; i++)
                {
                    markets[i] = markets[i].ToLowerInvariant();
                }
            }


            return client.Search<JObject>(sd => sd
                .Index(Indices.Index(typeof(PropertyContainer)).And(typeof(MgmtContainer)))
                .Size(maxResponseCount)
                .Query(qcd => (qcd
                    .Terms(tqd => tqd
                        .Field(Infer.Field<PropertyContainer>(e => e.Property.Market))
                        .Terms(markets)) &&
                (qcd.MultiMatch(mmqd => mmqd
                        .Fields(fd => fd
                            .Field(Infer.Field<PropertyContainer>(e => e.Property.Name))
                            .Field(Infer.Field<PropertyContainer>(e => e.Property.FormerName))
                            .Field(Infer.Field<PropertyContainer>(e => e.Property.StreetAddress))
                            .Field(Infer.Field<PropertyContainer>(e => e.Property.City)))
                            .Query(phase)) ||
                 qcd.Term(Infer.Field<PropertyContainer>(e => e.Property.State), phase.ToLowerInvariant()))) ||
                (qcd.Terms(tqd => tqd
                        .Field(Infer.Field<MgmtContainer>(e => e.Mgmt.Market))
                        .Terms(markets)) &&
                (qcd.Match(mqd => mqd
                        .Field(Infer.Field<MgmtContainer>(e => e.Mgmt.Name))
                        .Query(phase)) ||
                 qcd.Term(Infer.Field<MgmtContainer>(e => e.Mgmt.State), phase.ToLowerInvariant())))));
        }
    }
}
