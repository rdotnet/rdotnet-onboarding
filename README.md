R.NET quick start
==================

This repository contains C# code for new users of [R.NET](http://rdotnet.codeplex.com). Note that the github repository of this [quick start guide](https://github.com/jmp75/rdotnet-onboarding) may be transient and lead to more stable documentation on the R.NET site down the track.

Installation
-------------

* Clone (and fork if you wish) this repository with your git client. You can also use GitHub's "Download ZIP" in the bottom right hand corner.
* The solution and project files were authored with Visual Studio 2012 Express Desktop edition. You should be able to read them with VS2010 and monodevelop, but I have not tested.
* Most of the .csproj files have references to pre-compiled .NET binaries RDotNet.dll and RDotNet.NativeLibrary.dll, expected to be in the folder named "Binaries". You can:
 * Either copy these to the Binaries folder
 * or localize this to your own system, I'd recommend you use a decent text editor, which if decent should have handy capabilities to replace in files. If you are lacking such an editor I recommend [Notepad++](http://notepad-plus-plus.org) for Windows. Of course, you can update the references the manual way via visual studio: your call.

The Csharp project files contain the following references by default:
```xml
    <Reference Include="RDotNet">
      <HintPath>..\Binaries\RDotNet.dll</HintPath>
    </Reference>
    <Reference Include="RDotNet.NativeLibrary">
      <HintPath>..\Binaries\RDotNet.NativeLibrary.dll</HintPath>
    </Reference>
```

So you can do a search and replace on the string "..\Binaries" in all the files "*.csproj" and replace with the path of your choice, e.g.:
```xml
    <Reference Include="RDotNet">
      <HintPath>..\..\..\..\bin\R.NET\1.5.5\RDotNet.dll</HintPath>
    </Reference>
    <Reference Include="RDotNet.NativeLibrary">
      <HintPath>..\..\..\..\bin\R.NET\1.5.5\RDotNet.NativeLibrary.dll</HintPath>
    </Reference>
```

Getting started
-------------

Open the solution OnboardRDotNet.sln with a "recent" version of visual studio or MonoDevelop (Xamarin Studio)
You may want to look at and experiment with the projects in the following order:
* HelloWorld
* Sample1 is taken from the R.NET web site, a simple statistical T-test
* Sample2 is mostly for issues diagnosis at this stage
* Optimization: Adapted from my day work; simplified calibration and optimization problem. Shows various ways to call an R function including vectorized function evaluations.
* Tutorial1: To Be Determined
