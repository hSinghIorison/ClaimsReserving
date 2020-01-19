using System;
using System.IO;
using System.Threading.Tasks;
using CumulativeData.Model;
using log4net;


namespace CumulativeData
{
    public interface ICumulativeDataFileProducer
    {
        Task<string> CreateFile(CumulativeClaimData data);
    }

    public class CumulativeDataFileProducer : ICumulativeDataFileProducer
    {
        private readonly IConfig _config;
        private readonly ILog _logger;


        public CumulativeDataFileProducer(IConfig config, ILog logger )
        {
            _config = config;
            _logger = logger;
        }
        public async Task<string> CreateFile(CumulativeClaimData data)
        {
            string fileName = $"CD_{ DateTime.Now:yyyyMMddHHmmss}";
            try
            {
                _logger.Info($"Writing cumulative data file {fileName}");
                var filePath = $@"{_config.CumulativeDataFilePath}\{fileName}";
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    await sw.WriteLineAsync(data.EarliestOriginalYear.Value + "," + data.DevelopmentYears);
                    foreach(var cumulativeDataRow in data.Rows)
                    {
                       await sw.WriteLineAsync(cumulativeDataRow.ToString());
                    }
                }

                return fileName;
            }
            catch (Exception e)
            {
                _logger.Error($"Could not create cumulative data file {fileName}");
                throw;
            }
        }
    }
}