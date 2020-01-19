using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CumulativeData.Model;
using CumulativeData.SemanticType;
using log4net;
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
        private readonly ILog _logger;

        public IncrementalDataFileParser(
            ICsvMapping<IncrementalClaimData> mapping,
            IConfig config,
            ILog logger) 
        {
            _config = config;
            _logger = logger;
            var csvParserOptions = new CsvParserOptions(true, ',');
            _csvParser = new CsvParser<IncrementalClaimData>(csvParserOptions, mapping);
        }

        public async Task<List<IncrementalClaimData>> Parse()
        {
            try
            {
                _logger.Info($"Parsing incremental claim data file");
                return await Task.Run(() =>
                {
                    var results = _csvParser.ReadFromFile(_config.IncrementalDataFilePath, Encoding.UTF8);
                    return results.Select(x => x.Result).ToList();
                });
            }
            catch (AggregateException e)
            {
                var aggregateException = e.Flatten();
                _logger.Error($"Error while parsing", aggregateException);
                throw aggregateException;
            }
            catch (Exception e)
            {
                _logger.Error($"Error while parsing", e);
                throw;
            }

        }
    }

    public interface IIncrementalDataFileParser
    {
        Task<List<IncrementalClaimData>> Parse();
    }
}