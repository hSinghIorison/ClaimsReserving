using System;

namespace CumulativeData.SemanticType
{
    public sealed class Year : SemanticTypeBase<string>, IEquatable<Year>, IComparable<Year>
    {
        public int DateTimeYear { get; }

        public Year(string someYear) : base(IsValid, someYear)
        {
            DateTimeYear = Convert.ToInt32(Value);
        }

        public bool Equals(Year other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DateTimeYear == other.DateTimeYear;
        }

        public int CompareTo(Year other)
        {
            return DateTimeYear.CompareTo(other.DateTimeYear);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Year other && Equals(other);
        }

        public override int GetHashCode()
        {
            return DateTimeYear.GetHashCode();
        }

        public static bool Equals(Year left, Year right)
        {
            return left?.Equals(right)
                   ?? ReferenceEquals(left, right);
        }

        public static bool operator ==(Year left, Year right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Year left, Year right)
        {
            return !Equals(left, right);
        }

        public static int operator -(Year left, Year right)
        {
            return left.DateTimeYear - right.DateTimeYear + 1; //inclusive
        }

        private static bool IsValid(string arg)
        {
            int result;
            var isValid = Int32.TryParse(arg, out result);
            
            return isValid;
        }
    }
}