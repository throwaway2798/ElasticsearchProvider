using Nest;

using System.Collections.Generic;


namespace ElasticsearchProvider.DataStructures
{
    public class MgmtContainer
    {
        public Mgmt Mgmt { get; set; }


        public const string Index = "mgmtcs";


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
                .Map<MgmtContainer>(tmd => tmd
                    .AutoMap()
                    .Properties(pd => pd
                        .Number(npd => npd
                            .Name(e => e.Mgmt.MgmtId)
                            .Type(NumberType.Integer))
                        .Text(tpd => tpd
                            .Name(e => e.Mgmt.Name)
                            .Analyzer("my_analyzer"))
                        .Keyword(kpd => kpd
                            .Name(e => e.Mgmt.Market))
                        .Keyword(kpd => kpd
                            //.Normalizer("my_normalizer")
                            .Name(e => e.Mgmt.State)))));
        }
    }


    public class MgmtContainerEqualityComparer : IEqualityComparer<MgmtContainer>
    {
        bool IEqualityComparer<MgmtContainer>.Equals(MgmtContainer x, MgmtContainer y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.Mgmt == null && y.Mgmt == null)
            {
                return true;
            }
            else if (x.Mgmt == null || y.Mgmt == null)
            {
                return false;
            }


            return x.Mgmt.MgmtId == y.Mgmt.MgmtId &&
                x.Mgmt.Name == y.Mgmt.Name &&
                x.Mgmt.Market == y.Mgmt.Market &&
                x.Mgmt.State == y.Mgmt.State;
        }


        int IEqualityComparer<MgmtContainer>.GetHashCode(MgmtContainer obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else if (obj.Mgmt == null)
            {
                return 0;
            }


            return obj.Mgmt.MgmtId.GetHashCode() ^
                obj.Mgmt.Name.GetHashCode() ^
                obj.Mgmt.Market.GetHashCode() ^
                obj.Mgmt.State.GetHashCode();
        }
    }
}
