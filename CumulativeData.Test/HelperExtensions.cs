using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison;
using SemanticComparison.Fluent;

namespace CumulativeData.Test
{
    public static class HelperExtensions
    {
        public static List<T> InList<T>(this T x)
        {
            return new List<T> { x };
        }

        public static T[] InArray<T>(this T x)
        {
            return new List<T> { x }.ToArray();
        }

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

        public static void DoesNotThrow<T>(Action expressionUnderTest, string exceptionMessage = "Expected exception was thrown by target of invocation.") where T : Exception
        {
            try
            {
                expressionUnderTest();
            }
            catch (T)
            {
                Assert.Fail(exceptionMessage);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            Assert.IsTrue(true);
        }

    }

    public static class LikenessSequenceExtensions
    {
        public static bool SequenceLike<T, TSource>(this IEnumerable<T> that, IEnumerable<TSource> source)
        {
            return SequenceLike<T, TSource>(that, source, x => x);
        }

        public static bool SequenceLike<T, TSource>(this IEnumerable<T> that, IEnumerable<TSource> source, Func<Likeness<TSource, T>, IEquatable<T>> customizeLikeness)
        {
            return source.Select(x => customizeLikeness(x.AsSource().OfLikeness<T>())).SequenceEqual(that.Cast<object>());
        }
    }
}