// ================================================================================
// TL;DR:
// 开始按钮业务逻辑控制器，处理点击事件并触发场景跳转。
// 配合 ElasticButton 实现"先播动画，再跳转"的优雅过渡。
//
// 目标：
// - 响应开始按钮点击事件
// - 延迟跳转以配合按钮弹性动画
// - 启动游戏场景
//
// 非目标：
// - 不处理按钮视觉动画（由 ElasticButton 负责）
// - 不处理场景加载细节（由 SceneManager 负责）
// ================================================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_StartButton : MonoBehaviour 
{
    public float delayBeforeLoad = 0.4f; // 配合 ElasticButton 的动画时长

    public void ClickStartGame() 
    {
        StartCoroutine(WaitAndGo());
    }
    
    IEnumerator WaitAndGo()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        
        // 直接加载游戏场景
        SceneManager.LoadScene("GameScene");
    }
}
