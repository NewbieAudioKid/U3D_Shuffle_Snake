// ================================================================================
// TL;DR:
// 贪吃蛇游戏的全局管理器，负责游戏状态、分数、计时器和场景切换。
// 使用 DontDestroyOnLoad 单例模式，在整个游戏生命周期内持久存在。
//
// 目标：
// - 管理游戏状态（开始、暂停、结束）
// - 管理分数和最高分
// - 管理20秒倒计时
// - 触发游戏结束弹窗（胜利/失败）
// - 提供场景切换接口
//
// 非目标：
// - 不处理贪吃蛇移动逻辑（由 SnakeController 负责）
// - 不处理扑克牌逻辑（由 PokerManager 负责）
// - 不处理输入（由 InputManager 负责）
// ================================================================================
using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ==================== 游戏状态 ====================
    [Header("游戏状态")]
    public int currentScore = 0;        // 当前分数
    public int highScore = 0;           // 最高分
    public float gameTime = 20f;        // 游戏时长（秒）
    public float remainingTime = 20f;   // 剩余时间
    public bool isGameRunning = false;  // 游戏是否进行中

    // ==================== 生命周期 ====================

    /// <summary>
    /// 单例初始化，确保全局只有一个 GameManager 实例。
    /// 使用 DontDestroyOnLoad 使其在场景切换时不被销毁。
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载时的回调
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 如果加载的是 GameScene，自动开始游戏
        if (scene.name == "GameScene")
        {
            StartGame();
        }
    }

    // ==================== 游戏控制 ====================

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        currentScore = 0;
        remainingTime = gameTime;
        isGameRunning = true;
        StartCoroutine(GameTimer());
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        isGameRunning = false;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        isGameRunning = true;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 游戏计时器协程
    /// </summary>
    IEnumerator GameTimer()
    {
        while (remainingTime > 0 && isGameRunning)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        if (isGameRunning)
        {
            // 时间到，游戏胜利（显示分数）
            EndGame(true);
        }
    }

    // ==================== 分数管理 ====================

    /// <summary>
    /// 增加分数
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
    }

    /// <summary>
    /// 保存最高分
    /// </summary>
    void SaveHighScore()
    {
        PlayerPrefs.SetInt("SnakeHighScore", highScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载最高分
    /// </summary>
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("SnakeHighScore", 0);
    }

    // ==================== 游戏结束处理 ====================

    /// <summary>
    /// 结束游戏
    /// </summary>
    /// <param name="isWin">是否胜利（贪吃蛇游戏中，时间到即结束，显示分数）</param>
    public void EndGame(bool isWin)
    {
        isGameRunning = false;
        Time.timeScale = 1f; // 恢复时间流速
        
        if (isWin)
            TriggerVictory();
        else
            TriggerGameOver();
    }

    /// <summary>
    /// 触发胜利弹窗（时间到，显示分数）
    /// </summary>
    public void TriggerVictory()
    {
        if (GameResultPopup.Instance != null)
            GameResultPopup.Instance.ShowVictory();
        else
            Debug.LogError("❌ 场景里找不到 GameResultPopup！");
    }

    /// <summary>
    /// 触发失败弹窗（撞墙/撞障碍）
    /// </summary>
    public void TriggerGameOver()
    {
        if (GameResultPopup.Instance != null)
            GameResultPopup.Instance.ShowGameOverDelayed();
        else
            Debug.LogError("❌ 场景里找不到 GameResultPopup！");
    }

    // ==================== 场景切换 ====================

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 返回菜单
    /// </summary>
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}