// Define the functions which can be called from the .dll.

//Very Important !! DO NOT FORGET the following using or the script won't work (on Unity 5xx)
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class OpenCVGetMseFromWebcam : MonoBehaviour
{
    private bool _ready; //Will allow us to know if camera stream is on
    public int mse_h_g, mse_h, mse_h_d; // here we store the value of the mean square error of the three squares in opencv window
    public int threshold = 35;
    void Start()
    {
        int result = OpenCVInterop.Init();
        if (result == -1)
        {
            Debug.LogWarningFormat("[{0}] Failed to open camera stream.", GetType());
            return;
        }

        _ready = true;
    }

    void OnApplicationQuit()
    {
        if (_ready)
            OpenCVInterop.Close();
    }

    void Update()
    {
        if (!_ready)
            return;
        bool down = Input.GetKeyDown(KeyCode.R);

        if (Input.GetKeyDown(KeyCode.M))
            threshold -= 1;

        if (Input.GetKeyDown(KeyCode.P))
            threshold += 1;
        OpenCVInterop.main(ref mse_h_g, ref mse_h, ref mse_h_d, ref threshold, ref down);
    }
}