using FubuTestingSupport;
using NUnit.Framework;
using ripple.MSBuild;

namespace ripple.Testing.MSBuild
{
    [TestFixture]
    public class default_framework_identifier_v4_client_profile : framework_name_detection_harness
    {
        protected override string theXml
        {
            get
            {
                return
@"<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{884D1E3D-9BF1-4A15-9C48-06A7A43A6676}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>ClassLibrary1</RootNamespace>
    <AssemblyName>ClassLibrary1</AssemblyName>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <TargetFrameworkProfile>Client</TargetFrameworkProfile>
  </PropertyGroup>
</Project>";
            }
        }

        [Test]
        public void uses_default_identifier()
        {
            theFrameworkName.Identifier.ShouldEqual(FrameworkNameDetector.DefaultIdentifier);
        }

        [Test]
        public void finds_the_version()
        {
            var version = theFrameworkName.Version;
            version.Major.ShouldEqual(4);
            version.Minor.ShouldEqual(0);
        }

        [Test]
        public void finds_the_profile()
        {
            theFrameworkName.Profile.ShouldEqual("Client");
        }
    }

    [TestFixture]
    public class default_framework_identifier_v45_no_profile : framework_name_detection_harness
    {
        protected override string theXml
        {
            get
            {
                return
@"<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{884D1E3D-9BF1-4A15-9C48-06A7A43A6676}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>ClassLibrary1</RootNamespace>
    <AssemblyName>ClassLibrary1</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
  </PropertyGroup>
</Project>";
            }
        }

        [Test]
        public void uses_default_identifier()
        {
            theFrameworkName.Identifier.ShouldEqual(FrameworkNameDetector.DefaultIdentifier);
        }

        [Test]
        public void finds_the_version()
        {
            var version = theFrameworkName.Version;
            version.Major.ShouldEqual(4);
            version.Minor.ShouldEqual(5);
        }

        [Test]
        public void finds_the_profile()
        {
            theFrameworkName.Profile.ShouldBeEmpty();
        }
    }

    [TestFixture]
    public class non_default_framework_identifier_with_higher_version_and_no_profile : framework_name_detection_harness
    {
        protected override string theXml
        {
            get
            {
                return
@"<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProductVersion>8.0.50727</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{57B8F8EC-CC92-45A6-9985-9D41E3B83CF4}</ProjectGuid>
    <ProjectTypeGuids>{A1591282-1198-4647-A2B1-27E5FF5F6F3B};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}</ProjectTypeGuids>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>SilverlightClassLibrary3</RootNamespace>
    <AssemblyName>SilverlightClassLibrary3</AssemblyName>
    <TargetFrameworkIdentifier>Silverlight</TargetFrameworkIdentifier>
    <TargetFrameworkVersion>v5.0</TargetFrameworkVersion>
    <SilverlightVersion>$(TargetFrameworkVersion)</SilverlightVersion>
    <SilverlightApplication>false</SilverlightApplication>
    <ValidateXaml>true</ValidateXaml>
    <ThrowErrorsInValidation>true</ThrowErrorsInValidation>
  </PropertyGroup>
</Project>";
            }
        }

        [Test]
        public void uses_supplied_identifier()
        {
            theFrameworkName.Identifier.ShouldEqual("Silverlight");
        }

        [Test]
        public void finds_the_version()
        {
            var version = theFrameworkName.Version;
            version.Major.ShouldEqual(5);
            version.Minor.ShouldEqual(0);
        }

        [Test]
        public void no_profile()
        {
            theFrameworkName.Profile.ShouldBeEmpty();
        }
    }
}