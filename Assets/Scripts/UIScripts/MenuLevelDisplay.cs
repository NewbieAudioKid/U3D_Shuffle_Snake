// ================================================================================
// TL;DR:
// 菜单关卡显示（新版本：贪吃蛇游戏不需要关卡系统）
// 简化版本，显示最高分或保持空白
//
// 目标：
// - 在菜单界面显示游戏信息（可选）
//
// 非目标：
// - 不显示关卡信息（新游戏无关卡系统）
// ================================================================================
using UnityEngine;
using TMPro;

public class MenuLevelDisplay : MonoBehaviour
{
    [Header("可选：显示最高分")]
    public TextMeshProUGUI[] levelTexts; // 可以用来显示其他信息

    void Start()
    {
        UpdateDisplay();
    }

    void OnEnable()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        // 如果需要显示最高分，可以在这里实现
        // 例如：levelTexts[0].text = "High Score: " + GameManager.Instance.highScore;
        
        // 目前保持为空，不显示任何内容
    }
}
