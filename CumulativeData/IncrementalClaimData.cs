using System;
using CumulativeData.SemanticType;

namespace CumulativeData
{
    public class IncrementalClaimData
    {
        public string Product { get; set; }
        public Year OriginalYear { get; set; }
        public Year DevelopmentYear { get; set; }
        public double Increment { get; set; }
    }
}