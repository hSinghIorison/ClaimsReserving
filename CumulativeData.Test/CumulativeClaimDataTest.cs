using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CumulativeData.Model;
using CumulativeData.SemanticType;
using log4net;
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
        private CumulativeClaim _sut;

        [TestInitialize]
        public void TestInit()
        {
            _container = new UnityAutoMoqContainer();
            _container.RegisterType<ICsvMapping<IncrementalClaimData>, CsvIncrementalClaimDataMapping>();
            _container.RegisterType<IIncrementalDataFileParser, IncrementalDataFileParser>();

            _sut = _container.Resolve<CumulativeClaim>();
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
            var claimData = await _sut.Process(incrementalClaims);

            // assert
            var expected = new Year("1990");
            claimData.EarliestOriginalYear
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
            var claimData = await _sut.Process(incrementalClaims);

            // assert
            byte expected = 4;
            Assert.AreEqual(expected, claimData.DevelopmentYears);
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\IncrementalClaimData.csv")]
        public async Task Process_ProductGroupsWithClaimTriangles_CumulativeDataRowsAreCreated()
        {
            // arrange
            _container
                .GetMock<IConfig>()
                .Setup(x => x.IncrementalDataFilePath)
                .Returns(Path.GetFullPath("IncrementalClaimData.csv"));

            var incrementalDataFileParser = _container.Resolve<IIncrementalDataFileParser>();
            var incrementalClaims = await incrementalDataFileParser.Parse();

            // act
            var actualRows = await _sut.Process(incrementalClaims);

            // assert
            var expected = new List<CumulativeDataRow>();
            
            var cumulativeDataRow1 = new CumulativeDataRow("Comp");
            cumulativeDataRow1.AddIncrement(0);
            cumulativeDataRow1.AddIncrement(0);
            cumulativeDataRow1.AddIncrement(0);
            cumulativeDataRow1.AddIncrement(0);
            cumulativeDataRow1.AddIncrement(0);
            cumulativeDataRow1.AddIncrement(0);
            cumulativeDataRow1.AddIncrement(0);
            cumulativeDataRow1.AddIncrement(110);
            cumulativeDataRow1.AddIncrement(280);
            cumulativeDataRow1.AddIncrement(200);

            var cumulativeDataRow2 = new CumulativeDataRow("Non-Comp");
            cumulativeDataRow2.AddIncrement(45.2);
            cumulativeDataRow2.AddIncrement(110);
            cumulativeDataRow2.AddIncrement(110);
            cumulativeDataRow2.AddIncrement(147);
            cumulativeDataRow2.AddIncrement(50);
            cumulativeDataRow2.AddIncrement(125);
            cumulativeDataRow2.AddIncrement(150);
            cumulativeDataRow2.AddIncrement(55);
            cumulativeDataRow2.AddIncrement(140);
            cumulativeDataRow2.AddIncrement(100);

            expected.Add(cumulativeDataRow1);
            expected.Add(cumulativeDataRow2);

            foreach (var cumulativeDataRow in actualRows.Rows)
            {
                var cumulativeDataRows = expected.Single(x => x.Product == cumulativeDataRow.Product);
                Assert.AreEqual(cumulativeDataRows.ToString(), cumulativeDataRow.ToString());
            }
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\IncrementalClaimData.csv")]
        public async Task Process_ProductGroupsWithClaimTriangles_CumulativeDataFileCreated()
        {
            // arrange
            _container
                .GetMock<IConfig>()
                .Setup(x => x.IncrementalDataFilePath)
                .Returns(Path.GetFullPath("IncrementalClaimData.csv"));

            var cTestOutput = @"C:\Test\Output";
            _container
                .GetMock<IConfig>()
                .Setup(x => x.CumulativeDataFilePath)
                .Returns(Path.GetFullPath(cTestOutput));

            var incrementalDataFileParser = _container.Resolve<IIncrementalDataFileParser>();
            var incrementalClaims = await incrementalDataFileParser.Parse();

            // act
            var actualRows = await _sut.Process(incrementalClaims);
            var cumulativeDataFileProducer = new CumulativeDataFileProducer(_container.Resolve<IConfig>(), _container.Resolve<ILog>());
            var file = await cumulativeDataFileProducer.CreateFile(actualRows);
            
            // assert
            Assert.IsTrue(File.Exists(Path.Combine(cTestOutput,file)));
        }
    }
}