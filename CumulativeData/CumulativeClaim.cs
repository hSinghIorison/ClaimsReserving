using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CumulativeData.SemanticType;

namespace CumulativeData
{
    public class CumulativeClaim
    {
        private Dictionary<string, List<ClaimTriangle>> _productGroups;
        private  Year _earliestOriginalYear;
        private byte _developmentYears;
        public async Task<CumulativeClaimData> Process(List<IncrementalClaimData> incrementalClaims)
        {
            await Task.Run(() =>
            {
                _earliestOriginalYear = incrementalClaims.Min(x => x.OriginalYear);
                var year = incrementalClaims.Max(x => x.OriginalYear);
                _developmentYears = (byte) (year - _earliestOriginalYear);
            });

            await Task.Run(() =>
            {
                _productGroups = incrementalClaims
                    .GroupBy(x => x.Product)
                    .ToDictionary(x => x.Key, ToClaimSet);
            });

            List<Task<CumulativeDataRow>> tasks = new List<Task<CumulativeDataRow>>();

            foreach (var productGroup in _productGroups)
            {
                Task<CumulativeDataRow> task = Task.Run( () =>  MakeRow(productGroup));
                tasks.Add(task);
            }

            var rows = await Task.WhenAll(tasks);

            return new CumulativeClaimData
            {
                EarliestOriginalYear = _earliestOriginalYear,
                DevelopmentYears = _developmentYears,
                Rows = rows.ToList()
            };

        }

        private CumulativeDataRow MakeRow(KeyValuePair<string, List<ClaimTriangle>> productGroup)
        {
            var cumulativeDataRow = new CumulativeDataRow(productGroup.Key);
            Year maxDevelopmentYear = new Year ((_earliestOriginalYear.DateTimeYear + _developmentYears).ToString());
            for (int originalYear = _earliestOriginalYear.DateTimeYear;
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

    public class CumulativeClaimData    
    {
        public Year EarliestOriginalYear { get; set; }
        public byte DevelopmentYears { get; set; }
        public List<CumulativeDataRow> Rows { get; set; }
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