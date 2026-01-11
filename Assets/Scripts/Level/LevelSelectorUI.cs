// ================================================================================
// TL;DR:
// 关卡选择器（新版本：贪吃蛇游戏不需要关卡系统，保留以防万一）
// 简化版本，直接启动游戏场景
//
// 目标：
// - 提供关卡选择界面的点击响应接口
// - 启动游戏场景
//
// 非目标：
// - 不处理关卡解锁逻辑
// - 不处理场景切换动画
// ================================================================================
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorUI : MonoBehaviour
{
    public void OnClickLevel1()
    {
        // 直接加载游戏场景
        SceneManager.LoadScene("GameScene");
    }

    // 如果以后需要多关卡，可以在这里扩展
}
