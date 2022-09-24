using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaMgr : MonoBehaviour
{
    public RectTransform SafeArea;

    [Header("Apple SafeArea Settings")]
    public float AppleSafeAreaFactor = 0.5f;
    public bool AppleTop = false;
    public bool AppleBottom = false;
    public bool AppleRight = false;
    public bool AppleLeft = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SafeArea != null)
        {
            var rt = SafeArea.GetComponent<RectTransform>();
            AdjustSafeArea(rt, AppleSafeAreaFactor, AppleTop, AppleBottom, AppleRight, AppleLeft);
        }
    }

    static public void AdjustSafeArea(RectTransform rt, float AppleSafeAreaFactor, bool AppleTop, bool AppleBottom, bool AppleRight, bool AppleLeft)
    {
#if UNITY_ANDROID
        //Android ScreenSafeAREA
        DeviceInfo.ApplySafeAreaAndroid(rt);
#endif

#if UNITY_IOS
        //IOS ScreenSafeAREA
        DeviceInfo.ApplySafeAreaIOS(rt, AppleSafeAreaFactor, AppleTop, AppleBottom, AppleRight, AppleLeft);
#endif
    }

}
