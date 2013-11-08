using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Model.Validation;

namespace ripple.Testing.Model.Validation
{
    [TestFixture]
    public class SolutionValidatorTester
    {
        [TearDown]
        public void TearDown()
        {
            SolutionValidator.Reset();
        }

        [Test]
        public void runs_the_rules()
        {
            SolutionValidator.Clear();
            SolutionValidator.RegisterRule<StubRule>();

            var result = new SolutionValidator().Validate(Solution.Empty());
            result.IsValid().ShouldBeFalse();
        }


        public class StubRule : ISolutionValidationRule
        {
            public void Validate(Solution solution, ValidationResult result)
            {
                result.AddProblem("Stub", "The problem");
            }
        }
    }
}