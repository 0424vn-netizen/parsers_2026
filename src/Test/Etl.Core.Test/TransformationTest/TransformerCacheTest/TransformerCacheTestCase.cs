using Aperia.UT;
using Etl.Core.Transformation;

namespace Etl.Core.Test.TransformationTest.TransformerCacheTest
{
    public class TransformerCacheTestCase : ITestCaseParam<TransformerCacheTestBuilder, NA, (TransformResult result, Exception? ex)>
    {
        public TransformerCacheTestCase(string caseName, TransformerCacheTestBuilder constructArgs, NA actionArgs, (TransformResult result, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = actionArgs;
        }
        public string CaseName { get; set; }
        public TransformerCacheTestBuilder ConstructArgs { get; set; }
        public NA ActionArgs { get; set; }
        public (TransformResult result, Exception? ex) Expectation { get; set; }
    }
}
