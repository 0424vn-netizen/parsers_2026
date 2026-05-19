using Aperia.UT;
using Etl.Core.Transformation.Actions;
using Etl.Tranformation.Fields;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.DateFieldTest
{
    [TestClass]
    public class DateFieldTestSuite
    {
        [TestMethod]
        public void DateField_Test()
        {
            TestBuilder.CreateInstance((NA _) =>
            {
                var builder = new DateFieldTestBuilder();

                return builder.Build();
            }).Action((IServiceCollection inst, string arg) =>
            {
                var provider = inst.BuildServiceProvider();
                var field = new DateField();
                var actionType = field.DefaultAction.GetType().GetProperty("ActionType", System.Reflection.BindingFlags.NonPublic
                                                                            | System.Reflection.BindingFlags.Instance).GetValue(field.DefaultAction) as Type;
                var transformAction = (ITransformActionInst)provider.GetRequiredService(actionType);
                return transformAction.Execute(arg, null);
            })
            .Validator<(DateTime? result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result, expect.result);
            })
            .TestCases(GetTestCases())
            .Run();
        }

        private DateFieldTestCase[] GetTestCases()
        {
            return new DateFieldTestCase[]
            {
            new DateFieldTestCase("Should_Be_Successful_Test_Case", NA.Value, new DateTime(2023,1,1).ToString(), (new DateTime(2023,1,1), null)),
            new DateFieldTestCase("Should_Be_Null_Test_Case", NA.Value, null, (null, null))
            };
        }
    }
}