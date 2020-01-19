namespace CumulativeData
{
    public interface IConfig
    {
        string IncrementalDataFilePath { get; set; }
        string CumulativeDataFilePath { get; set; }
    }
}