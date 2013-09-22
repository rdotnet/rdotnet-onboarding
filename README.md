R.NET quick start
==================

This repository contains C# code for new users of [R.NET](http://rdotnet.codeplex.com). Note that the github repository of this [quick start guide](https://github.com/jmp75/rdotnet-onboarding) may be transient and lead to more stable documentation on the R.NET site down the track.

Installation
-------------

* Clone (and fork if you wish) this repository with your git client. You can also use GitHub's "Download ZIP" in the bottom right hand corner.
* The solution and project files were authored with Visual Studio 2012 Express Desktop edition. You should be able to read them with VS2010 and monodevelop, but I have not tested.
* Most of the .csproj files have referenced to the compiled R.NET*.dll, as shown below. To localize this to your own system, I'd recommend you use a decent text editor, which if decent should have handy capabilities to replace in files. If you are lacking an editor I recommend [Notepad++](http://notepad-plus-plus.org) for Windows. Of course, you can update the references the manual way via visual studio: your call.

```xml
    <Reference Include="RDotNet">
      <HintPath>..\..\..\..\bin\R.NET\1.5.5\RDotNet.dll</HintPath>
    </Reference>
    <Reference Include="RDotNet.NativeLibrary">
      <HintPath>..\..\..\..\bin\R.NET\1.5.5\RDotNet.NativeLibrary.dll</HintPath>
    </Reference>
```
