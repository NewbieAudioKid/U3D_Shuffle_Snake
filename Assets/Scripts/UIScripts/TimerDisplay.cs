// ================================================================================
// TL;DR:
// 倒计时显示器，显示游戏剩余时间
// 绑定到 Canvas → Top → top_level → level_name1
//
// 目标：
// - 实时显示剩余时间（倒计时格式：15, 14, 13...）
// - 时间不足5秒时变红色警告
//
// 非目标：
// - 不处理时间逻辑（由 GameManager 负责）
// ================================================================================
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [Header("UI引用")]
    public TextMeshProUGUI timerText; // level_name1

    [Header("视觉设置")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningThreshold = 5f; // 剩余5秒时变红

    void Start()
    {
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && timerText != null)
        {
            if (GameManager.Instance.isGameRunning)
            {
                // 显示倒计时（向上取整，避免显示0.x秒）
                int seconds = Mathf.CeilToInt(GameManager.Instance.remainingTime);
                seconds = Mathf.Max(0, seconds); // 确保不显示负数
                timerText.text = seconds.ToString();

                // 时间不足时变红色
                if (GameManager.Instance.remainingTime <= warningThreshold)
                {
                    timerText.color = warningColor;
                }
                else
                {
                    timerText.color = normalColor;
                }
            }
            else
            {
                // 游戏未开始或已结束
                timerText.text = "20";
                timerText.color = normalColor;
            }
        }
    }
}

