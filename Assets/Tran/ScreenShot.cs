using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    private bool snapFlag = false;

    public Texture2D tex;

    public void StartSnap()
    {
        snapFlag = true;
    }

    private void OnPostRender()
    {
        if (snapFlag)
        {
            tex = new Texture2D(Screen.width, Screen.height);
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            tex.Apply();
            snapFlag = false;
        }
    }
}
