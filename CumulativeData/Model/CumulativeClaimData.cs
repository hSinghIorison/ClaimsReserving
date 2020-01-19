using System.Collections.Generic;
using CumulativeData.SemanticType;

namespace CumulativeData.Model
{
    public class CumulativeClaimData    
    {
        public Year EarliestOriginalYear { get; set; }
        public byte DevelopmentYears { get; set; }
        public List<CumulativeDataRow> Rows { get; set; }
    }
}