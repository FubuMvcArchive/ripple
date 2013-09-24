using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model.Conditions;

namespace ripple.Testing.Model.Conditions
{
    [TestFixture]
    public class NotDirectoryConditionTester
    {
        private StubDirectoryCondition theInnerCondition;
        private NotDirectoryCondition theCondition;

        [SetUp]
        public void SetUp()
        {
            theInnerCondition = new StubDirectoryCondition();
            theCondition = new NotDirectoryCondition(theInnerCondition);
        }

        [Test]
        public void matches_when_inner_condition_is_true()
        {
            theInnerCondition.IsMatch(true);
            theCondition.Matches(null, null).ShouldBeTrue();
        }

        [Test]
        public void no_match_when_inner_condition_is_false()
        {
            theInnerCondition.IsMatch(false);
            theCondition.Matches(null, null).ShouldBeFalse();
        }
    }
}