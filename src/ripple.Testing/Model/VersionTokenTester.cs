using System;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class VersionTokenTester
    {
        [Test]
        public void gets_the_value()
        {
            var token = new VersionToken("Current", x => new SemanticVersion("1.2.0.0"));
            token.Value(new SemanticVersion("1.0.0.0")).ShouldEqual(new SemanticVersion("1.2.0.0"));
        }

        [Test]
        public void current_gets_the_current_value()
        {
            var version = new SemanticVersion("1.0.1.244");
            VersionToken.Current.Value(version).ShouldEqual(version);
        }

        [Test]
        public void next_major()
        {
            VersionToken
                .NextMajor
                .Value(new SemanticVersion("1.1.1.3467"))
                .ShouldEqual(new SemanticVersion("2.0.0.0"));
        }

        [Test]
        public void current_major()
        {
            VersionToken
                .CurrentMajor
                .Value(new SemanticVersion("1.1.1.3467"))
                .ShouldEqual(new SemanticVersion("1.0.0.0"));
        }

        [Test]
        public void next_min()
        {
            VersionToken
                .NextMinor
                .Value(new SemanticVersion("1.1.3.987"))
                .ShouldEqual(new SemanticVersion("1.2.0.0"));
        }

        [Test]
        public void finds_the_current_token()
        {
            VersionToken.Find("Current").ShouldEqual(VersionToken.Current);
            VersionToken.Find("current").ShouldEqual(VersionToken.Current);
        }

        [Test]
        public void finds_the_next_major_token()
        {
            VersionToken.Find("NextMajor").ShouldEqual(VersionToken.NextMajor);
            VersionToken.Find("nextmajor").ShouldEqual(VersionToken.NextMajor);
        }

        [Test]
        public void finds_the_next_min_token()
        {
            VersionToken.Find("NextMinor").ShouldEqual(VersionToken.NextMinor);
            VersionToken.Find("nextminor").ShouldEqual(VersionToken.NextMinor);
        }

        [Test]
        public void throws_if_the_key_cannot_be_found()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => VersionToken.Find("Joel"));
        }
    }
}