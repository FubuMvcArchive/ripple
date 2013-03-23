# ripple

ripple was originally created to aide the Fubu team in managing large numbers of dependencies both locally and in continuation integration environments. It was designed to fill a gap in usage found with the NuGet client.

## Why not use NuGet?

NuGet worked great for "Fixed" versions of dependencies. However, our differing opinions quickly began to cause friction. We believe that all of our projects should be built against the latest of their Fubu dependencies. For example, any of our projects that depend on FubuMVC.Core.dll should *always* be built against the *latest* version of FubuMVC.Core.dll.

## Is there overlap with NuGet?

Yes. We still utilize NuGet feeds but we just layer on additional concepts - namely Float vs. Fixed dependencies and feeds. 

## Can I use it on my own project?

We're working very hard on documentation for FubuMVC at the moment. When there is time, we will come back and add proper documentation to ripple. In the meantime, ripple is distributed via our buildsupport submodule.

## What are the future plans?

Someday, we will create an actual gem instead of the buildsupport submodule. We will also enhance ripple so that it can operate simply off current directory.
