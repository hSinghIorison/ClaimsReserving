using System;

namespace CumulativeData.SemanticType
{
    public class SemanticTypeBase<T>
    {
        public T Value { get; private set; }

        protected SemanticTypeBase(Func<T, bool> isValid, T value)
        {
            if ((Object) value == null)
            {
                throw new ArgumentException($"Trying to use null as the value of {GetType()}");
            }

            if (isValid != null && !isValid(value))
            {
                throw new ArgumentException($"Trying to set a {GetType()} to {value} which is invalid" );
            }

            Value = value;
        }
        
    }
}