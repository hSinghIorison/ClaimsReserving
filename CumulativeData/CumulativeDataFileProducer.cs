using System;
using System.IO;

namespace CumulativeData
{
    public class CumulativeDataFileProducer 
    {
        private readonly IConfig _config;

        public CumulativeDataFileProducer(IConfig config)
        {
            _config = config;
        }
        public void CreateFile(CumulativeClaimData data)
        {
            try
            {
                var filePath = $@"{_config.CumulativeDataFilePath}\CD_{DateTime.Now.ToLongTimeString()}";
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
                throw;
            }
        }
    }
}