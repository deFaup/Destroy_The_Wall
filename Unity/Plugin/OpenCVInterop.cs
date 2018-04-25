using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class OpenCVInterop : MonoBehaviour
{
    [DllImport("Unity_CV_2_v4")]
    public static extern int Init();

    [DllImport("Unity_CV_2_v4")]
    public static extern void Close();

    [DllImport("Unity_CV_2_v4")]
    public static extern void main(ref int mse_h_g, ref int mse_h, ref int mse_h_d, ref int threshold, ref bool down);
}
