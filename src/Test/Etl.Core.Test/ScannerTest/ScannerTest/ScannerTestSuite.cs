using Aperia.UT;
using Etl.Core.Scanner;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Etl.Core.Test.ScannerTest.ScannerTest
{
    [TestClass]
    public class ScannerTestSuite
    {
        [TestMethod]
        public void Scanner_Test()
        {
            TestBuilder.CreateInstance((ScannerTestBuilder construct) =>
            {
            var builder = new ScannerTestBuilder();
                builder.MockOnFlushAsync.Setup(x => x.Invoke(It.IsAny<ScannedRecord>())).Returns(Task.CompletedTask);
                return builder.Build();
            }).Action( async (Scanner.Scanner inst, (ScannerTestBuilder construct,  int ? take, int? skip) arg) =>
            {
                var obj = new Scanner.Scanner(arg.construct.StreamReader,
                    arg.construct.StartLayout, arg.construct.StartRecord, arg.construct.EndRecord, arg.construct.MockOnFlushAsync.Object);
                await obj.StartAsync(arg.take, arg.skip);
            }).Validator<(NA, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result.Result.Exception, expect.ex);
            }).TestCases(GetTestCases())
            .Run();
        }

        private ScannerTestCase[] GetTestCases()
        {
            return new ScannerTestCase[]
            {
                new ScannerTestCase("case 1", new ScannerTestBuilder(), ( new ScannerTestBuilder{
                        EndRecord = (regex: new Regex(@"^EndRecord\:"), offset: 0)
                        } , 1, 1), (NA.Value,null) ),
                 new ScannerTestCase("case 2", new ScannerTestBuilder(), ( new ScannerTestBuilder{
                        EndRecord = (regex: new Regex(@"^EndRecord\:"), offset: 0)
                        } , 1, 0), (NA.Value,null) ),
                  new ScannerTestCase("case 3", new ScannerTestBuilder(), ( new ScannerTestBuilder{
                          StartLayout = (regex: new Regex(@"\A"), offset: 0),
                        StartRecord = (regex: new Regex(@"\d"), offset: 0),
                        EndRecord = (regex: new Regex(@"\d"), offset: 0)
                        } , 1, 1), (NA.Value,null) ),
                  new ScannerTestCase("case 4", new ScannerTestBuilder(), ( new ScannerTestBuilder{
                          StartLayout = (regex: new Regex(@"\A"), offset: 3),
                        StartRecord = (regex: new Regex(@"\d"), offset: 3),
                        EndRecord = (regex: new Regex(@"\d"), offset: 3)
                        } , 1, 1), (NA.Value,null) )
            };
        }


        [TestMethod]
        public void ScannerDisponse_Test()
        {
            TestBuilder.CreateInstance((ScannerTestBuilder construct) =>
            {
                var builder = new ScannerTestBuilder();
                builder.MockOnFlushAsync.Setup(x => x.Invoke(It.IsAny<ScannedRecord>())).Returns(Task.CompletedTask);
                return builder.Build();
            }).Action( (Scanner.Scanner inst) =>
            {
                inst.Dispose();
            }).Validator<(NA, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(null, expect.ex);
            }).TestCases(GetScannerDisponseTestCases())
            .Run();
        }


        private ScannerDisponseTestCase[] GetScannerDisponseTestCases()
        {
            return new ScannerDisponseTestCase[] {
                new ScannerDisponseTestCase("disponseTest", new ScannerTestBuilder(),NA.Value, (NA.Value, null))
            };
        }

        private class ScannerDisponseTestCase : ITestCaseParam<ScannerTestBuilder, NA, (NA, Exception? ex)>
        {
            public ScannerDisponseTestCase(string caseName, ScannerTestBuilder constructArgs, NA actionArgs, (NA, Exception? ex) expectation)
            {
                CaseName = caseName;
                ConstructArgs = constructArgs;
                ActionArgs = actionArgs;
                Expectation = expectation;
            }

            public string CaseName { get; set; }
            public ScannerTestBuilder ConstructArgs { get; set; }
            public NA ActionArgs { get; set; }
            public (NA, Exception? ex) Expectation { get; set; }
        }


    }
}
