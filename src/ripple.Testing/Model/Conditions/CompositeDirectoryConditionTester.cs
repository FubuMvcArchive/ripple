using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model.Conditions;

namespace ripple.Testing.Model.Conditions
{
    [TestFixture]
    public class CompositeDirectoryConditionTester
    {
        private StubDirectoryCondition c1;
        private StubDirectoryCondition c2;
        private StubDirectoryCondition c3;

        private CompositeDirectoryCondition theCompositeCondition;

        [SetUp]
        public void SetUp()
        {
            c1 = new StubDirectoryCondition();
            c2 = new StubDirectoryCondition();
            c3 = new StubDirectoryCondition();

            theCompositeCondition = new CompositeDirectoryCondition(c1, c2, c3);
        }

        [Test]
        public void matches_when_all_conditions_are_true()
        {
            c1.IsMatch(true);
            c2.IsMatch(true);
            c3.IsMatch(true);

            theCompositeCondition.Matches(null, null).ShouldBeTrue();
        }

        [Test]
        public void no_match_when_any_condition_is_false()
        {
            c1.IsMatch(true);
            c2.IsMatch(false);
            c3.IsMatch(true);

            theCompositeCondition.Matches(null, null).ShouldBeFalse();
        }
    }
}