using Aperia.UT;
using Etl.Core.Extraction;
using Etl.Core.Transformation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.TransformationTest.TransformerCacheTest
{
    [TestClass]
    public class TransformerCacheTestSuite
    {

        [TestMethod]
        public void TransformerCacheHasParameter_Test()
        {
            TestBuilder.CreateInstance((TransformerCache construct) =>
            {
                var transform = new Mock<Transformer>();
                var layout = new Mock<Layout>();

                return new TransformerCache(transform.Object, layout.Object);

            }).Action((TransformerCache inst) =>
            {
                var sp = new Mock<IServiceProvider>();
                return inst.CreateTransformFunc(sp.Object);

            }).Validator<(Func<Records, Records>? result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                var extractRecord = new ExtractedRecord(new Scanner.TextBlock(new List<Scanner.TextLine>()), (1, 1), (2, 2));
                var response = result.Result.Invoke(extractRecord);
                Assert.IsTrue(1 == response.TotalRecords);
            }).TestCases(GetTestCases())
            .Run();
        }

        [TestMethod]
        public void TransformerCacheParameterLess_Test()
        {
            TestBuilder.CreateInstance((TransformerCache construct) =>
            {
                var transform = new Mock<Transformer>();
                var layout = new Mock<Layout>();
                return new TransformerCache(transform.Object, layout.Object);

            }).Action((TransformerCache inst) =>
            {
                return inst.CreateMassageFunc();

            }).Validator<(Func<Records, Records>? result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result.Result?.Invoke(new List<IDictionary<string, object?>>()), expect.result?.Invoke(new List<IDictionary<string, object?>>()));
            }).TestCases(GetTestCases())
            .Run();
        }

        private LessParameterTestCase[] GetTestCases()
        {
            return new LessParameterTestCase[] {
               new LessParameterTestCase("case_return_Null", new TransformerCache(new Transformer(), new Layout())
                , NA.Value
                , ((List<IDictionary<string, object?>> a) => null, null))
           };
        }

        public class LessParameterTestCase : ITestCaseParam<TransformerCache, NA, (Func<List<IDictionary<string, object?>>, List<IDictionary<string, object?>>>? result, Exception? ex)>
        {
            public LessParameterTestCase(string caseName, TransformerCache constructArgs, NA actionArgs, (Func<List<IDictionary<string, object?>>, List<IDictionary<string, object?>>>? result, Exception? ex) expectation)
            { 
                CaseName = caseName;
                ConstructArgs = constructArgs;
                ActionArgs = actionArgs;
                Expectation = expectation;
            }
            public string CaseName { get; set; }
            public TransformerCache ConstructArgs { get; set; }
            public NA ActionArgs { get; set; }
            public (Func<Records, Records>? result, Exception? ex) Expectation { get; set; }
        }


        [TestMethod]
        public void TransformerCacheParameterLessHasMassageAssembly_Test()
        {
            TestBuilder.CreateInstance((TransformerCache construct) =>
            {
                var transform = new Mock<Transformer>();
                var layout = new Mock<Layout>();

                var transformerCache = new TransformerCache(transform.Object, layout.Object);
                transformerCache.GetType().GetField("_massageAssembly", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(transformerCache, Assembly.GetExecutingAssembly());

                return transformerCache;

            }).Action((TransformerCache inst) =>
            {
                return inst.CreateMassageFunc();

            }).Validator<(Func<Records, Records>? result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                var mockRecords = new Mock<Records>();
                var r = result.Result?.Invoke(mockRecords.Object);
                Assert.AreEqual(r, mockRecords.Object);
            }).TestCases(GetTestCases())
            .Run();
        }

    }
}
