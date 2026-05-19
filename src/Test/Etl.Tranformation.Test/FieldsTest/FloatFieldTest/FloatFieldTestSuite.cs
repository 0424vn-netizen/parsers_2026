using Aperia.UT;
using Etl.Core.Transformation.Actions;
using Etl.Tranformation.Fields;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.FloatFieldTest
{
    [TestClass]
    public class FloatFieldTestSuite
    {
        [TestMethod]
        public void FloatField_Test()
        {
            TestBuilder.CreateInstance((NA _) =>
            {
                var builder = new FloatFieldTestBuilder();

                return builder.Build();
            }).Action((IServiceCollection inst, string arg) =>
            {
                var provider = inst.BuildServiceProvider();
                var field = new FloatField();
                var actionType = field.DefaultAction.GetType().GetProperty("ActionType", System.Reflection.BindingFlags.NonPublic
                                                                            | System.Reflection.BindingFlags.Instance).GetValue(field.DefaultAction) as Type;
                var transformAction = (ITransformActionInst)provider.GetRequiredService(actionType);
                return transformAction.Execute(arg, null);
            })
            .Validator<(double? result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result, expect.result);
            })
            .TestCases(GetTestCases())
            .Run();
        }

        private FloatFieldTestCase[] GetTestCases()
        {
            return new FloatFieldTestCase[]
            {
                new FloatFieldTestCase("Should_Be_Null_Test_Case", NA.Value, null, (null, null)),
                new FloatFieldTestCase("Should_Be_Successful_Test_Case", NA.Value, "1.00", (1.00, null)),
                new FloatFieldTestCase("Should_Be_Successful_Test_Case", NA.Value, "1.00-", (-1.00, null))
            };
        }
    }
}
