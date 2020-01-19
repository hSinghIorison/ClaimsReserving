using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CumulativeData.Model;
using CumulativeData.SemanticType;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyCsvParser.Mapping;
using UnityAutoMoq;

namespace CumulativeData.Test
{
    [TestClass]
    public class IncrementalDataFileParserTest
    {
        private UnityAutoMoqContainer _container;
        private IncrementalDataFileParser _sut;

        [TestInitialize]
        public void TestInit()
        {
            _container = new UnityAutoMoqContainer();

            _container.RegisterType<ICsvMapping<IncrementalClaimData>, CsvIncrementalClaimDataMapping>();
            _sut = _container.Resolve<IncrementalDataFileParser>();
        }

        [TestMethod]
        [DeploymentItem(@"TestResources\IncrementalClaimData.csv")]
        public async Task Parse_ResolvesDependencies_ParsesTestFileInput()
        {
            // arrange
            _container
                .GetMock<IConfig>()
                .Setup(x => x.IncrementalDataFilePath)
                .Returns(Path.GetFullPath("IncrementalClaimData.csv"));
            
            // act
            var incrementalClaimDataList = await _sut.Parse();

            // assert
            var rowCount = 12;
            var productCount = 2;
            var rowCountForAGivenProduct = 3;

            Assert.AreEqual(rowCount, incrementalClaimDataList.Count);
            Assert.AreEqual(productCount, incrementalClaimDataList.GroupBy(x=>x.Product).Count());
            Assert.AreEqual(rowCountForAGivenProduct, incrementalClaimDataList.Count(x => x.Product == "Comp"));
            Assert.AreEqual(100, incrementalClaimDataList
                .Single(x => x.Product=="Non-Comp" 
                             && x.OriginalYear == new Year("1993")
                             && x.DevelopmentYear == new Year("1993")).Increment);
        }
    }
}