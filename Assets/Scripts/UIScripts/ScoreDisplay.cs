// ================================================================================
// TL;DR:
// 分数显示器，显示当前游戏分数
// 绑定到 Canvas → Top → top_money → money_Panel → Money_text
//
// 目标：
// - 实时显示当前分数
// - 分数变化时可添加动画效果（可选）
//
// 非目标：
// - 不处理分数逻辑（由 GameManager 负责）
// ================================================================================
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [Header("UI引用")]
    public TextMeshProUGUI scoreText; // Money_text

    [Header("显示设置")]
    public string prefix = ""; // 前缀（如 "Score: "）
    public string suffix = ""; // 后缀（如 " pts"）

    private int lastScore = -1; // 用于检测分数变化

    void Start()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && scoreText != null)
        {
            int currentScore = GameManager.Instance.currentScore;

            // 只在分数变化时更新（优化性能）
            if (currentScore != lastScore)
            {
                scoreText.text = prefix + currentScore.ToString() + suffix;
                lastScore = currentScore;
            }
        }
    }
}

