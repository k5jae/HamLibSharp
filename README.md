# HamLibSharp
Mono/.NET Binding for the Ham Radio Control Libraries (https://github.com/N0NB/hamlib)

The goal of this binding is to support desktop platforms HamLib supports. Currently tested with Linux (Ubuntu 16.04) on both 64 and 32 bit architectures and Windows as a 32-bit process. 

This binding is a work-in-progress. It targets HamLib 1.2, 3.0.1 and 3.1(git). Native structs are abstracted by interfaces, which allows client code to "just work"

What has been tested:
* Rig VFO Frequency adjustments (both read and write)
* PTT keying

This binding uses .NET P/Invoke functionality to make calls into C libraries. Data structures from the C library have been recreated in the binding and data is marshaled from native memory into managed memory. The native library is not hard linked and the .so or .dll must be in the path for this binding to locate it at runtime. The project file will copy files from bin_libs into the output directory.

More description and HOWTO to follow.
