using Aperia.UT;
using Etl.Core;
using Etl.Core.Transformation.Actions;
using Etl.Core.Transformation.Fields;
using Etl.Tranformation.Actions;
using Etl.Tranformation.Test.FieldsTest.BooleanFieldTest;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.ActionsTest.CheckPatternActionTest
{
    [TestClass]
    public class CheckPatternActionTestSuite
    {
        [TestMethod]
        public void CheckPatternAction_Test()
        {
            TestBuilder.CreateInstance((NA _) =>
            {
                var builder = new CheckPatternActionTestBuilder();

                return builder.Build();
            }).Action((IServiceCollection inst, (string? input, string? pattern) arg) =>
            {
                var provider = inst.BuildServiceProvider();
                var checkAction = new CheckPatternAction { Pattern = arg.pattern };
                var actionType = checkAction.GetType().GetProperty("ActionType", System.Reflection.BindingFlags.NonPublic
                                                                            | System.Reflection.BindingFlags.Instance).GetValue(checkAction) as Type;

                var transformField = (ITransformActionInst)provider.GetRequiredService(actionType);
                (transformField as IInitialization)?.Initialize(checkAction, provider);

                try
                {
                    transformField.Execute(arg.input, null);
                    return true;
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            })
            .Validator<(bool result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result, expect.result);
                Assert.AreEqual(result?.Exception?.GetType(), expect.ex?.GetType());
            })
            .TestCases(GetTestCases())
            .Run();
        }

        private CheckPatternActionTestCase[] GetTestCases()
        {
            return new CheckPatternActionTestCase[]
            {
                new CheckPatternActionTestCase("Should_Be_True_Test_Case", NA.Value, ("02/2023", @"^\d{2}\/\d{4}$"), (true, null)),
                new CheckPatternActionTestCase("Should_Be_True_Test_Case_1", NA.Value, ("02/2023", null), (true, null)),
                new CheckPatternActionTestCase("Should_Be_Exception_Test_Case", NA.Value, (null, @"^\d{2}\/\d{4}$"), (false, new InvalidOperationException($"Not match pattern '{@"^\d{2}\/\d{4}$"}'"))),
                new CheckPatternActionTestCase("Should_Be_Exception_Test_Case_1", NA.Value, ("02", @"^\d{2}\/\d{4}$"), (false, new InvalidOperationException($"Not match pattern '{@"^\d{2}\/\d{4}$"}'"))),
            };
        }
    }
}
