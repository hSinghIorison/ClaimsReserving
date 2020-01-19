namespace CumulativeData
{
    public interface IConfig
    {
        string IncrementalDataFilePath { get; set; }
        string CumulativeDataFilePath { get; set; }
    }

    public class Config : IConfig
    {
        public string IncrementalDataFilePath { get;  set; }
        public string CumulativeDataFilePath { get;  set; }
        
    }
}