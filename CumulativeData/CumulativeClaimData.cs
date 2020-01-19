using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CumulativeData.SemanticType;

namespace CumulativeData
{
    public class CumulativeClaimData
    {
        public Dictionary<string, List<ClaimTriangle>> ProductGroups { get; private set; }
        public Year EarliestOriginalYear { get; private set; }
        public byte DevelopmentYears { get; private set; }
        public async Task<List<CumulativeDataRow>> Process(List<IncrementalClaimData> incrementalClaims)
        {
            await Task.Run(() =>
            {
                EarliestOriginalYear = incrementalClaims.Min(x => x.OriginalYear);
                var year = incrementalClaims.Max(x => x.OriginalYear);
                DevelopmentYears = (byte) (year - EarliestOriginalYear);
            });

            await Task.Run(() =>
            {
                ProductGroups = incrementalClaims
                    .GroupBy(x => x.Product)
                    .ToDictionary(x => x.Key, ToClaimSet);
            });

            List<Task<CumulativeDataRow>> tasks = new List<Task<CumulativeDataRow>>();

            foreach (var productGroup in ProductGroups)
            {
                Task<CumulativeDataRow> task = Task.Run( () =>  MakeRow(productGroup));
                tasks.Add(task);
            }

            var rows = await Task.WhenAll(tasks);

            return rows.ToList();

        }

        private CumulativeDataRow MakeRow(KeyValuePair<string, List<ClaimTriangle>> productGroup)
        {
            var cumulativeDataRow = new CumulativeDataRow(productGroup.Key);
            Year maxDevelopmentYear = new Year ((EarliestOriginalYear.DateTimeYear + DevelopmentYears).ToString());
            for (int originalYear = EarliestOriginalYear.DateTimeYear;
                originalYear < maxDevelopmentYear.DateTimeYear;
                originalYear++)
            {
                double runningIncrement = 0;
                for (int developmentYear = originalYear;
                    developmentYear < maxDevelopmentYear.DateTimeYear;
                    developmentYear++)
                {
                    var claimTriangle = productGroup.Value.SingleOrDefault(x =>
                            x.OriginalYear.DateTimeYear == originalYear &&
                            x.DevelopmentYear.DateTimeYear == developmentYear);

                    if (claimTriangle != null)
                    {
                        runningIncrement += claimTriangle.Increment;
                    }

                    cumulativeDataRow.AddIncrement(runningIncrement);
                }
            }
            return cumulativeDataRow;
        }

        private List<ClaimTriangle> ToClaimSet(IGrouping<string, IncrementalClaimData> incrementalClaims)
        {
            var claimTriangles = new List<ClaimTriangle>();
            foreach (var data in incrementalClaims.OrderBy(x=>x.OriginalYear).ThenBy(x=>x.DevelopmentYear))
            {
                claimTriangles.Add(ClaimTriangle.Create(data.OriginalYear, data.DevelopmentYear, data.Increment));
            }

            return claimTriangles;
        }
    }

    public class CumulativeDataRow  
    {
        public string Product { get; private  set; }
        public List<double> Increments { get; private set; }

        public CumulativeDataRow(string product)
        {
            Product = product;
            Increments = new List<double>();
        }

        public void AddIncrement(double increment)
        {
            Increments.Add(increment);
        }

        public override string ToString()
        {
            return Product+","+ String.Join(",", Increments);
        }
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