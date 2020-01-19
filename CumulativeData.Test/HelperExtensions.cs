using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CumulativeData.Test
{
    public static class HelperExtensions
    {
        public static T Throws<T>(Action func) where T : Exception
        {
            try
            {
                func.Invoke();
            }
            catch (T ex)
            {
                return ex;
            }

            throw new AssertFailedException($"An exception of type {typeof(T)} was expected, but not thrown");
        }
    }
}