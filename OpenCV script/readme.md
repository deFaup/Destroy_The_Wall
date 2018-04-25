To use original opencv library with Unity without using a C# equivalent we need to procees as follows:
   
  - Define all the functions you need to be able to call from Unity (for example "Close()" to close the webcam !)
  - Create a C++ script with all this functions. It can, of course, include functions you won't call in Unity
  - Change the declaration of the function you need in unity like this:
      extern "C" int __declspec(dllexport) __stdcall (if your function has a type int)
   
  - Compile your C++ code in Release mode to get dynamic library (.dll)
  - In C# import the dll
