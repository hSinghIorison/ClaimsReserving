using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CumulativeData.SemanticType;

namespace CumulativeData
{
    public class CumulativeClaimData
    {
        public Dictionary<string, HashSet<ClaimTriangle>> ProductGroups { get; private set; }
        public Year EarliestOriginalYear { get; private set; }
        public byte DevelopmentYears { get; private set; }
        public async Task Process(List<IncrementalClaimData> incrementalClaims)
        {
            await Task.Run(() =>
            {
                EarliestOriginalYear = incrementalClaims.Min(x => x.OriginalYear);
                DevelopmentYears = (byte) (incrementalClaims.Max(x => x.OriginalYear) - EarliestOriginalYear);
            });

            await Task.Run(() =>
            {
                ProductGroups = incrementalClaims
                    .GroupBy(x => x.Product)
                    .ToDictionary(x => x.Key, ToClaimHashSet);
            });

            List<Task<CumulativeDataRow>> tasks = new List<Task<CumulativeDataRow>>();

            foreach (var productGroup in ProductGroups)
            {
                Task<CumulativeDataRow> task = Task.Run( () =>  MakeRow(productGroup));
                tasks.Add(task);
            }

            var rows = await Task.WhenAll(tasks);

            var cumulativeDataRows = rows.ToList();

        }

        private CumulativeDataRow MakeRow(KeyValuePair<string, HashSet<ClaimTriangle>> productGroup)
        {
            return new CumulativeDataRow { Product = productGroup.Key };
        }

        private HashSet<ClaimTriangle> ToClaimHashSet(IGrouping<string, IncrementalClaimData> incrementalClaims)
        {
            var claimTriangles = new HashSet<ClaimTriangle>();
            foreach (var data in incrementalClaims.OrderBy(x=>x.OriginalYear).ThenBy(x=>x.DevelopmentYear))
            {
                claimTriangles.Add(ClaimTriangle.Create(data.OriginalYear, data.DevelopmentYear, data.Increment));
            }

            return claimTriangles;
        }
    }

    public class CumulativeDataRow  
    {
        public string Product { get; set; }
    }

    public class ClaimTriangle
    {
        private ClaimTriangle(){}    
        public Year OriginalYear { get; private set; }
        public Year DevelopmentYear { get; private set; }
        public double Increment { get; private set; }

        public static ClaimTriangle Create(Year originalYear, Year developmentYear, double increment)
        {
            return new ClaimTriangle
            {
                DevelopmentYear = developmentYear,
                OriginalYear = originalYear,
                Increment = increment
            };
        }
    }
}