using CumulativeData.SemanticType;

namespace CumulativeData.Model
{
    public class ClaimTriangle
    {
        private ClaimTriangle(){}    
        public Year OriginalYear { get; private set; }
        public Year DevelopmentYear { get; private set; }
        public double Increment { get; private set; }

        public static ClaimTriangle Create(Year originalYear, Year developmentYear, double increment)
        {
            return new ClaimTriangle
            {
                DevelopmentYear = developmentYear,
                OriginalYear = originalYear,
                Increment = increment
            };
        }
    }
}