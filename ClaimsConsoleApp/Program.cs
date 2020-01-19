using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using CumulativeData;
using CumulativeData.Model;
using log4net;
using Microsoft.Practices.Unity;
using TinyCsvParser.Mapping;

namespace ClaimsConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            var container = new UnityContainer();
            try
            {
                
                container.RegisterType<ILog>(new InjectionFactory(factory => LogManager.GetLogger("Cumulative Data")));
                container.RegisterInstance<IConfig>(new Config
                {
                    CumulativeDataFilePath = ConfigurationManager.AppSettings["CumulativeDataFilePath"],
                    IncrementalDataFilePath = ConfigurationManager.AppSettings["IncrementalDataFilePath"]
                });
                container.RegisterType<ICsvMapping<IncrementalClaimData>, CsvIncrementalClaimDataMapping>();
                container.RegisterType<IIncrementalDataFileParser, IncrementalDataFileParser>();
                container.RegisterType<ICumulativeClaim, CumulativeClaim>();
                container.RegisterType<ICumulativeDataFileProducer, CumulativeDataFileProducer>();


                var incrementalDataFileParser = container.Resolve<IncrementalDataFileParser>();
                var cumulativeClaim = container.Resolve<CumulativeClaim>();
                var cumulativeDataFileProducer = container.Resolve<CumulativeDataFileProducer>();

                List<IncrementalClaimData> incrementalData = await incrementalDataFileParser.Parse();

                CumulativeClaimData cumulativeClaimData = await cumulativeClaim.Process(incrementalData);

                await cumulativeDataFileProducer.CreateFile(cumulativeClaimData);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                var logger = container.Resolve<ILog>();
                logger.Error(e);
                Console.WriteLine(e);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
