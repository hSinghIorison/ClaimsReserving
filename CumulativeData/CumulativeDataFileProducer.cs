using System;
using System.IO;
using CumulativeData.Model;
using log4net;


namespace CumulativeData
{
    public interface ICumulativeDataFileProducer
    {
        void CreateFile(CumulativeClaimData data);
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
        public void CreateFile(CumulativeClaimData data)
        {
            string fileName = $"CD_{ DateTime.Now.ToLongTimeString()}";
            try
            {
                _logger.Info($"Writing cumulative data file {fileName}");
                var filePath = $@"{_config.CumulativeDataFilePath}\{fileName}";
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(data.EarliestOriginalYear + "," + data.DevelopmentYears);
                    foreach (var cumulativeDataRow in data.Rows)
                    {
                        sw.WriteLine(cumulativeDataRow.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Could not create cumulative data file {fileName}");
                throw;
            }
        }
    }
}