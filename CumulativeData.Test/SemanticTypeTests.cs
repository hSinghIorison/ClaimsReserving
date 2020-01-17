using System;
using AutoFixture;
using CumulativeData.SemanticType;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace CumulativeData.Test
{
    [TestClass]
    public class SemanticTypeTests
    {
        private IFixture _fixture;

        [TestInitialize]
        public void TestInit()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void Year_NullArgument_ExceptionThrown()
        {
            // act & assert
            var argumentException = HelperExtensions.Throws<ArgumentException>(()=> new Year(null));
        }

        [TestMethod]
        public void Year_InvalidArgument_ExceptionThrown()
        {
            // act& assert
            var argumentException = HelperExtensions.Throws<ArgumentException>(() => new Year(_fixture.Create<string>()));
        }

        [TestMethod]
        public void Year_DateTime_ReturnsYear()
        {
            // arrange
            var someYear = "1991";

            // act
            Year year = new Year(someYear);

            // assert
            Assert.AreEqual(someYear, year.DateTimeYear.ToString());
        }

        [TestMethod]
        public void Equals_Year1AndYear2HavingSameString_AreEqual()
        {
            // arrange
            var someYear = "1991";

            var year1 = new Year(someYear);
            var year2 = new Year(someYear);

            // act
            var actual = year1.Equals(year2);
            
            // assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Equals2_Year1AndYear2HavingSameString_AreEqual()
        {
            // arrange
            var someYear = "1991";

            var year1 = new Year(someYear);
            var year2 = new Year(someYear);

            // act
            var actual = Year.Equals(year1, year2);

            // assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Equals_Year1AndYear2AsObjectHavingSameString_AreEqual()
        {
            // arrange
            var someYear = "1991";

            var year1 = new Year(someYear);
            object year2 = new Year(someYear);

            // act
            var actual = year1.Equals( year2);

            // assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Equals_Year1AndYearHavingSameString_AreEqualUsingOperator()
        {
            // arrange
            var someYear = "1991";

            var year1 = new Year(someYear);
            var year2 = new Year(someYear);

            // act
            var actual = year1 == year2;

            // assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Equals_Year1AndYear2HavingDifferentString_AreNotEqualUsingOperator()
        {
            // arrange
            var someYear1 = "1991";
            var someYear2 = "2002";

            var year1 = new Year(someYear1);
            var year2 = new Year(someYear2);

            // act
            var actual = year1 != year2;

            // assert
            Assert.IsTrue(actual);
        }
    }
}
