using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

static public class DeviceInfo {

    static public void AdjustSafeArea(RectTransform rt)
    {
#if UNITY_ANDROID
        //Android ScreenSafeAREA
        ApplySafeAreaAndroid(rt);
#elif UNITY_IOS
        //IOS ScreenSafeAREA 
        ApplySafeAreaIOS(rt);
#endif
    }

    static public void ApplySafeAreaIOS(RectTransform rt)
    {
        // detecting notch
        var sa = Screen.safeArea;
        var aa = new Rect(0, 0, Screen.width, Screen.height);

        // pixel of safearea nach dem start (ohne notch anpassung)
        var wi0 = rt.rect.width;
        var hi0 = rt.rect.height;
        var ra0 = wi0 / hi0;
        // screen rect zum ratio pruefen
        var wi1 = aa.width;
        var hi1 = aa.height;
        var ra1 = wi1 / hi1;

        var scaling = wi1 / wi0;
        var left = sa.xMin;
        var right = aa.width - sa.width;
        var top = sa.yMin;
        var bottom = aa.height - sa.height;

        var sleft = left / scaling;
        var stop = top / scaling;
        var sright = right / scaling;
        var sbottom = bottom / scaling;

        //Bottom safearea to 0 (own adjusted value)
        float sbottom_adjust = 0;
        if (sbottom > 0)
        {
            sbottom = sbottom_adjust;
        }


        Debug.LogFormat("AT AdjustSafeArea (right, top, left, bottom): ({0}, {1}, {2}, {3})",
            sright, stop, sleft, sbottom);

        stop *= -1;
        sright *= -1;

        rt.offsetMax = new Vector2(sright, stop);
        rt.offsetMin = new Vector2(sleft, sbottom);
    }


    static public void ApplySafeAreaAndroid(RectTransform rt, float ScaleFactor = 1f)
    {
        var safeArea = Screen.safeArea;

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        var canvasmain = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        if (canvasmain == null)
        {
            Debug.LogError("No MainCanvas found (Pls assign MainCanvas Tag to maincanvas)!");
        }
        else
        {
            // X
            anchorMax.x /= canvasmain.pixelRect.width;
            var tempMaxX = (1 - anchorMax.x) * ScaleFactor;
            anchorMax.x = 1 - tempMaxX;

            anchorMin.x /= canvasmain.pixelRect.width;
            anchorMin.x = anchorMin.x * ScaleFactor;

            // Y
            anchorMax.y /= canvasmain.pixelRect.height;
            var tempMaxY = (1 - anchorMax.y) * ScaleFactor;
            anchorMax.y = 1 - tempMaxY;

            anchorMin.y /= canvasmain.pixelRect.height;
            anchorMin.y = anchorMin.y * ScaleFactor;

            //Debug.LogWarningFormat("Pixel Width: {0} | Height: {1} ", canvasmain.pixelRect.width, canvasmain.pixelRect.height);
            //Debug.LogWarningFormat("Anchor MaxX: {0} | MinX: {1} | MaxY: {2} | MinY: {3}",anchorMax.x ,anchorMin.x, anchorMax.y, anchorMin.y);

            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
        }
    }

    static public void ApplySafeAreaIOS(RectTransform rt, float SafeAreaFactor = 0.5f, bool Top = false, bool Bottom = false, bool Right = false, bool Left = false)
    {
        var safeArea = Screen.safeArea;

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        var canvasmain = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        if (canvasmain == null)
        {
            Debug.LogError("No MainCanvas found (Pls assign MainCanvas Tag to maincanvas)!");
        }
        else
        {            
            if (Right)
            {
                //USE Apple SafeAreas on RightSide
                anchorMax.x /= canvasmain.pixelRect.width;
                var tempMaxX = (1 - anchorMax.x) * SafeAreaFactor;
                anchorMax.x = 1 - tempMaxX;
            }
            else
            {
                anchorMax.x = 1f;
            }

            if (Left) {
                //USE Apple SafeAreas on Left
                anchorMin.x /= canvasmain.pixelRect.width;
                anchorMin.x = anchorMin.x * SafeAreaFactor;
            }
            else {
                anchorMin.x = 0f;
            }

            if (Top) {
                //USE Apple SafeAreas on LeftSide
                anchorMax.y /= canvasmain.pixelRect.height;
                var tempMaxY = (1 - anchorMax.y) * SafeAreaFactor;
                anchorMax.y = 1 - tempMaxY;
            }
            else {
                anchorMax.y = 1f;
            }

            if (Bottom) {
                //USE Apple SafeAreas on Bottom
                anchorMin.y /= canvasmain.pixelRect.height;
                anchorMin.y = anchorMin.y * SafeAreaFactor;
            }
            else {
                anchorMin.y = 0f;
            }

            //Set SafeArea Final
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
        }
    }


    static public long GetFreeDiskSpace() {
#if UNITY_EDITOR
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        foreach (DriveInfo d in allDrives) {
            if (d.IsReady == true) {
                var type = d.DriveType;
                var format = d.DriveFormat;
                var availablefreeSpc = d.AvailableFreeSpace;
                var totalfreeSpc = d.TotalFreeSpace;
                return totalfreeSpc;
            } else {
                return 0;
            }
        }

#elif UNITY_ANDROID
        AndroidJavaObject statFs = new AndroidJavaObject("android.os.StatFs", Application.persistentDataPath);
        long freeBytes = statFs.Call<long>("getAvailableBytes");
        return freeBytes;
#elif UNITY_IOS
        return 0xffffffffff; // das muesste erstmal reichen ...
#endif


        return -1;
    }
}
