using CumulativeData.SemanticType;

namespace CumulativeData.Model
{
    public class IncrementalClaimData
    {
        public string Product { get; set; }
        public Year OriginalYear { get; set; }
        public Year DevelopmentYear { get; set; }
        public double Increment { get; set; }
    }
}