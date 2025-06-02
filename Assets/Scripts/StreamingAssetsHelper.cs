#if UNITY_ANDROID && !UNITY_EDITOR
#define ANDROID_RUNTIME
#endif

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class StreamingAssetsHelper
{

#if ANDROID_RUNTIME

    private const string LIB_NAME = "unitynativeassets";

    [DllImport(LIB_NAME)]
    private static extern void Native_SetAssetManager(IntPtr javaAssetManager);

    [DllImport(LIB_NAME)]
    private static extern int Native_ReadAllBytes(string path, out IntPtr outBuffer);

    [DllImport(LIB_NAME)]
    private static extern void Native_FreeBuffer(IntPtr buffer);


    private static IntPtr GetAssetManagerFromUnity()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject assets = activity.Call<AndroidJavaObject>("getAssets");
            return assets.GetRawObject();
        }
    }

#endif

    public static void InitAssetManager()
    {
#if ANDROID_RUNTIME
        Native_SetAssetManager(AndroidJNI.NewGlobalRef(GetAssetManagerFromUnity()));
#endif
    }

    public static byte[] ReadAllBytes(string relativePath)
    {
#if ANDROID_RUNTIME
        IntPtr bufferPtr;
        int length = Native_ReadAllBytes(relativePath, out bufferPtr);
        if (length <= 0 || bufferPtr == IntPtr.Zero)
            return null;

        byte[] result = new byte[length];
        Marshal.Copy(bufferPtr, result, 0, length);
        Native_FreeBuffer(bufferPtr);
        return result;
#else
        string path = Path.Combine(Application.streamingAssetsPath, relativePath);
        if (File.Exists(path))
            return File.ReadAllBytes(path);
        return null;
#endif
    }
}