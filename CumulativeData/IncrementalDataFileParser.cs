using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CumulativeData.SemanticType;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace CumulativeData
{
    public class CsvIncrementalClaimDataMapping : CsvMapping<IncrementalClaimData>
    {
        public CsvIncrementalClaimDataMapping()
        {

            MapProperty(0, x => x.Product);
            MapProperty(1, x => x.OriginalYear, new YearTypeConverter());
            MapProperty(2, x => x.DevelopmentYear, new YearTypeConverter());
            MapProperty(3, x => x.Increment);
        }
    }

    public class YearTypeConverter : ITypeConverter<Year>
    {
        public bool TryConvert(string value, out Year result)
        {
            result = new Year(value);
            return true;
        }

        public Type TargetType => typeof(Year);
    }

    public class IncrementalDataFileParser : IIncrementalDataFileParser
    {
        private readonly CsvParser<IncrementalClaimData> _csvParser;
        private readonly IConfig _config;

        public IncrementalDataFileParser(
            ICsvMapping<IncrementalClaimData> mapping,
            IConfig config) 
        {
            _config = config;
            var csvParserOptions = new CsvParserOptions(true, ',');
            _csvParser = new CsvParser<IncrementalClaimData>(csvParserOptions, mapping);
        }

        public async Task<List<IncrementalClaimData>> Parse()
        {
            try
            {
                return await Task.Run(() =>
                {
                    var results = _csvParser.ReadFromFile(_config.IncrementalDataFilePath, Encoding.UTF8);
                    return results.Select(x => x.Result).ToList();
                });
            }
            catch (AggregateException e)
            {
                throw e.Flatten();
            }
            catch (Exception)
            {
                throw;
            }

        }
    }

    public interface IIncrementalDataFileParser
    {
        Task<List<IncrementalClaimData>> Parse();
    }

    public interface IConfig
    {
        string IncrementalDataFilePath { get; set; }
    }
}