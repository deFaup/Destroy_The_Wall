I] Introduction :

In the Assets folder of Unity you must have :

    - a "Plugins" folder where you put 
            all the .dll needed for your C++ code (ie : opencv dll and your c++ code dll)
            all the C# scripts that are a link with these dll (see II)
    
    - a "Script" folder where you put the scripts for the game behaviour (see III)
    
    
II] C# scripts in Plugins folder

Here we put all the scripts who need the dlls in the same folder to work

Since you are importing dll you need a script that will import them. Here it is "OpenCVInterop.cs"
In this script we create methods whom declarations will match the one of the function we exported in our C++ code

For example in C++ we exported an integer type function called Init() : 
                extern "C" int __declspec(dllexport) __stdcall Init()

Now in C# OpenCVInterop :
    - Firstly, we import the dll of the C++ code
      [DllImport("Unity_CV_2_v4")]
      
    - Secondly, we create a method with the same type and same name :
      public static extern int Init();
      
 Now that we have access to our C++ functions thanks to the methods in OpenCVInterop we need a second script to call these methods.
 Our second script (OpenCVGetMseFromWebcam) calls :
      - Init in the Start loop      // Open the camera stream and other actions
      - Close when you exit the game// Close the camera stream
      - main in the Update loop     // The main loop open a window which displays the stream of your webcam and compute mse values
      
  To call the public methods you can simply do : OpenCVInterop.Init();
  
  
II] C# scripts in Script folder

In your game scripts you can't access directly to the opencv methods, but you have access to the result of these methods.
The only methods which returns values that I need in the game scripts is the main method defined in OpenCVInterop.
This method is called in the Update loop of OpenCVGetMseFromWebcam and the result saved in the public variables of this class.

To get the result you need 
    - to find the game object which is an instance of OpenCVGetMseFromWebcam
      In my case it is "GameObjectPrincipal"
      
      GameObject empty;
      OpenCVGetMseFromWebcam lesmse;
      empty = GameObject.Find("GameObjectPrincipal");
    
    - then access to its component
    
    lesmse = empty.GetComponent<OpenCVGetMseFromWebcam>();
    mse_h_g = lesmse.mse_h_g;
    mse_h = lesmse.mse_h;
    mse_h_d = lesmse.mse_h_d;


