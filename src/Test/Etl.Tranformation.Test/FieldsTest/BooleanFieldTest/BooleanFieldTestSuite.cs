using Aperia.UT;
using Etl.Core.Transformation.Actions;
using Etl.Tranformation.Fields;
using Microsoft.Extensions.DependencyInjection;

namespace Etl.Tranformation.Test.FieldsTest.BooleanFieldTest
{
    [TestClass]
    public class BooleanFieldTestSuite
    {
        [TestMethod]
        public void BooleanField_Test()
        {
            TestBuilder.CreateInstance((NA _) =>
            {
                var builder = new BooleanFieldTestBuilder();

                return builder.Build();
            }).Action((IServiceCollection inst, string arg) =>
            {
                var provider = inst.BuildServiceProvider();
                var field = new BooleanField();
                var actionType = field.DefaultAction.GetType().GetProperty("ActionType", System.Reflection.BindingFlags.NonPublic
                                                                            | System.Reflection.BindingFlags.Instance).GetValue(field.DefaultAction) as Type;
                var transformAction = (ITransformActionInst)provider.GetRequiredService(actionType);
                return transformAction.Execute(arg, null);
            })
            .Validator<(bool? result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result, expect.result);
            })
            .TestCases(GetTestCases())
            .Run();
        }

        private BooleanFieldTestCase[] GetTestCases()
        {
            return new BooleanFieldTestCase[]
            {
            new BooleanFieldTestCase("Should_Be_True_Test_Case", NA.Value, "true", (true, null)),
            new BooleanFieldTestCase("Should_Be_False_Test_Case", NA.Value, null, (null, null))
            };
        }
    }
}