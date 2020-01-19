using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CumulativeData.Model;
using CumulativeData.SemanticType;
using log4net;

namespace CumulativeData
{
    public interface ICumulativeClaim
    {
        Task<CumulativeClaimData> Process(List<IncrementalClaimData> incrementalClaims);
    }

    public class CumulativeClaim : ICumulativeClaim
    {
        private readonly ILog _logger;
        private Dictionary<string, List<ClaimTriangle>> _productGroups;
        private  Year _earliestOriginalYear;
        private byte _developmentYears;

        public CumulativeClaim(ILog logger)
        {
            _logger = logger;
        }

        public async Task<CumulativeClaimData> Process(List<IncrementalClaimData> incrementalClaims)
        {
            try
            {
                _logger.Info($"Processing incremental claims to work out cumulative claims");
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
                    Task<CumulativeDataRow> task = Task.Run(() => MakeRow(productGroup));
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
            catch (Exception e)
            {
                _logger.Error("Error while working out cumulative claims", e);
                throw;
            }
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
}