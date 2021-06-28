using Nest;

using System.Collections.Generic;


namespace ElasticsearchProvider.DataStructures
{
    public class PropertyContainer
    {
        public Property Property { get; set; }


        public const string Index = "propertycs";


        public static CreateIndexResponse Create(IElasticClient client)
        {
            return client.Indices.Create(Index, cid => cid
                .Settings(isd => isd
                    .Analysis(ad => ad
                        .TokenFilters(tfd => tfd
                            .Stop("my_token_filter", stfd => stfd
                                .StopWords("_english_")))
                        .Tokenizers(td => td
                            .EdgeNGram("my_tokenizer", engtd => engtd
                                .MinGram(3)
                                .MaxGram(15)
                                .TokenChars(TokenChar.Letter, TokenChar.Digit)))
                        .Analyzers(asd => asd
                            .Custom("my_analyzer", cad => cad
                            .Filters("my_token_filter", "lowercase", "trim")
                            .Tokenizer("my_tokenizer")))
                        .Normalizers(nd => nd
                            .Custom("my_normalizer", cnd => cnd
                                .Filters("uppercase")))))
                .Map<PropertyContainer>(tmd => tmd
                    .AutoMap()
                    .Properties(pd => pd
                        .Number(npd => npd
                            .Name(e => e.Property.PropertyId)
                            .Type(NumberType.Integer))
                        .Text(tpd => tpd
                            .Name(e => e.Property.Name)
                            .Analyzer("my_analyzer"))
                        .Text(tpd => tpd
                            .Name(e => e.Property.FormerName)
                            .Analyzer("my_analyzer"))
                        .Text(tpd => tpd
                            .Name(e => e.Property.StreetAddress)
                            .Analyzer("my_analyzer"))
                        .Text(tpd => tpd
                            .Name(e => e.Property.City)
                            .Analyzer("my_analyzer"))
                        .Keyword(kpd => kpd
                            .Name(e => e.Property.Market))
                        .Keyword(kpd => kpd
                            //.Normalizer("my_normalizer")
                            .Name(e => e.Property.State))
                        .Number(npd => npd
                            .Name(e => e.Property.Lat)
                            .Type(NumberType.Float))
                        .Number(npd => npd
                            .Name(e => e.Property.Lng)
                            .Type(NumberType.Float)))));
        }
    }


    public class PropertyContainerEqualityComparer : IEqualityComparer<PropertyContainer>
    {
        bool IEqualityComparer<PropertyContainer>.Equals(PropertyContainer x, PropertyContainer y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.Property == null && y.Property == null)
            {
                return true;
            }
            else if (x.Property == null || y.Property == null)
            {
                return false;
            }


            return x.Property.PropertyId == y.Property.PropertyId &&
                x.Property.Name == y.Property.Name &&
                x.Property.FormerName == y.Property.FormerName &&
                x.Property.StreetAddress == y.Property.StreetAddress &&
                x.Property.City == y.Property.City &&
                x.Property.Market == y.Property.Market &&
                x.Property.State == y.Property.State &&
                x.Property.Lat == y.Property.Lat &&
                x.Property.Lng == y.Property.Lng;
        }


        int IEqualityComparer<PropertyContainer>.GetHashCode(PropertyContainer obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else if (obj.Property == null)
            {
                return 0;
            }


            return obj.Property.PropertyId.GetHashCode() ^
                obj.Property.Name.GetHashCode() ^
                obj.Property.FormerName.GetHashCode() ^
                obj.Property.StreetAddress.GetHashCode() ^
                obj.Property.City.GetHashCode() ^
                obj.Property.Market.GetHashCode() ^
                obj.Property.State.GetHashCode() ^
                obj.Property.Lat.GetHashCode() ^
                obj.Property.Lng.GetHashCode();
        }
    }
}
