
# HamLibSharp
Mono/.NET Binding for the Ham Radio Control Libraries (https://github.com/N0NB/hamlib)

The goal of this binding is to support desktop platforms HamLib supports. Currently tested with Linux (Ubuntu 16.04) on both 64 and 32 bit architectures and Windows as a 32-bit process. 

This binding is a work-in-progress. It targets HamLib 1.2, 3.0.1 and 3.1(git). Native structs are abstracted by interfaces, which allows client code to "just work"

###What has been tested
 - Rig VFO Frequency adjustments (both read and write)
 - PTT keying
 - Rig Mode adjustments (both read and write)

###How it works
This binding uses .NET P/Invoke functionality to make calls into C libraries. Data structures from the C library have been recreated in the binding and data is marshaled from native memory into managed memory. The native library is not hard linked and the .so or .dll must be in the path for this binding to locate it at runtime. The project file will copy files from bin_libs into the output directory.

###What's unique
 1. Similar to the C++ binding, generally any reported errors from HamLib will be thrown as RigException to client code.
 2. The binding has its own polling loop. To use this feature, the client code must call Rig.Start() after a successful Rig.Open(...). 
  - This binding will automatically poll for common values from Rig. Currently it polls for Frequency, Mode, and PTT. The polling interval is defined when the Rig class is constructed. The client application can use the Rig public properties to get current values.
  - For "set" type functions, the binding will add the call to the TaskQueue and return immediately. For GUI apps, this means that blocking is kept to a minimum. If Rig.Start() is not called, the functions will block until HamLib returns. (WORK IN PROGRESS: Not all set functions take advantage of the TaskQueue)

###Getting Started:
1. Start by cloning the git repository to a directory of your choice
2. Use your OS distribution of hamlib, or place the libhamlib.so.2 into the ./bin_libs or the Windows libhamlib-2.dll (and all dependancies) into the ./bin_libs/x86 directory (32-bit library)
3. Use MonoDevelop or xbuild to build the .sln file
4. Run the test programs which should be located in the ./build/(CONF)/ directory.

Note: You may need to rename the HamLib .so file if you copy it from HamLib build.

More description and HOWTO to follow.
