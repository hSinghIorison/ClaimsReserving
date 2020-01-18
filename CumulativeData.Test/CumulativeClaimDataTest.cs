using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CumulativeData.SemanticType;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SemanticComparison.Fluent;
using TinyCsvParser.Mapping;
using UnityAutoMoq;

namespace CumulativeData.Test
{
    [TestClass]
    public class CumulativeClaimDataTest
    {
        private UnityAutoMoqContainer _container;
        private CumulativeClaimData _sut;

        [TestInitialize]
        public void TestInit()
        {
            _container = new UnityAutoMoqContainer();
            _container.RegisterType<ICsvMapping<IncrementalClaimData>, CsvIncrementalClaimDataMapping>();
            _container.RegisterType<IIncrementalDataFileParser, IncrementalDataFileParser>();

            _sut = _container.Resolve<CumulativeClaimData>();
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\IncrementalClaimData.csv")]
        public async Task Process_DataHas1990AsEarliestDevelopmentYear_CanFind()
        {
            // arrange
            _container
                .GetMock<IConfig>()
                .Setup(x => x.IncrementalDataFilePath)
                .Returns(Path.GetFullPath("IncrementalClaimData.csv"));

            var incrementalDataFileParser = _container.Resolve<IIncrementalDataFileParser>();
            var incrementalClaims = await incrementalDataFileParser.Parse();

            // act
            await _sut.Process(incrementalClaims);

            // assert
            var expected = new Year("1990");
            _sut.EarliestOriginalYear
                .AsSource()
                .OfLikeness<Year>()
                .ShouldEqual(expected);
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\IncrementalClaimData.csv")]
        public async Task Process_DataHas4DevelopmentYear_CanFind()
        {
            // arrange
            _container
                .GetMock<IConfig>()
                .Setup(x => x.IncrementalDataFilePath)
                .Returns(Path.GetFullPath("IncrementalClaimData.csv"));

            var incrementalDataFileParser = _container.Resolve<IIncrementalDataFileParser>();
            var incrementalClaims = await incrementalDataFileParser.Parse();

            // act
            await _sut.Process(incrementalClaims);

            // assert
            byte expected = 4;
            Assert.AreEqual(expected, _sut.DevelopmentYears);
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\IncrementalClaimData.csv")]
        public async Task Process_ProductGroupsWithClaimTriangles_CanBeCreated()
        {
            // arrange
            _container
                .GetMock<IConfig>()
                .Setup(x => x.IncrementalDataFilePath)
                .Returns(Path.GetFullPath("IncrementalClaimData.csv"));

            var incrementalDataFileParser = _container.Resolve<IIncrementalDataFileParser>();
            var incrementalClaims = await incrementalDataFileParser.Parse();

            // act
            await _sut.Process(incrementalClaims);

            // assert
            byte expectedCountOfProducts = 2;
            var sutProductGroups = _sut.ProductGroups;
            Assert.AreEqual(expectedCountOfProducts, sutProductGroups.Keys.Count);
            
            ClaimTriangle firstExpectedClaimTriangleForFirstProduct = ClaimTriangle
                .Create(new Year("1992"), new Year("1992"), 110 );
            sutProductGroups["Comp"].First().AsSource().OfLikeness<ClaimTriangle>()
                .ShouldEqual(firstExpectedClaimTriangleForFirstProduct);

            ClaimTriangle lastExpectedClaimTriangleForSecondProduct = ClaimTriangle
                .Create(new Year("1993"), new Year("1993"), 100);
            sutProductGroups["Non-Comp"].Last().AsSource().OfLikeness<ClaimTriangle>()
                .ShouldEqual(lastExpectedClaimTriangleForSecondProduct);
        }
    }
}