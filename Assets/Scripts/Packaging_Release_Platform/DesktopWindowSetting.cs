using UnityEngine;

public class DesktopWindowSetting : MonoBehaviour
{
    // 在游戏启动前执行这个方法
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void SetResolution()
    {
        // 只在 Windows 或 Mac 平台生效
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        // 设置窗口大小为 540x1080 (1080:2160 的一半，方便显示)
        // false 表示不全屏，即窗口模式
        Screen.SetResolution(540, 1080, false);
        
        // 可选：强制竖屏逻辑
        Screen.fullScreenMode = FullScreenMode.Windowed;
#endif
    }
}