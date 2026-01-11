# Google C# Style Guide - 代码注释规范

## 📖 概述

本项目遵循 [Google C# Style Guide](https://google.github.io/styleguide/csharp-style.html) 的注释规范，确保代码的可读性和可维护性。

---

## ✅ 注释规范总结

### 1. 文件头注释

每个文件顶部应包含：
- **功能摘要**（TL;DR）
- **目标**（Goals）
- **非目标**（Non-Goals）

```csharp
// Copyright 2026 NewbieAudioKid
//
// Licensed under the MIT License.
//
// <summary>
// GameManager 管理游戏的全局状态、计时器、分数和场景切换。
// 使用 DontDestroyOnLoad 单例模式确保在场景切换时持久存在。
//
// Goals:
// - 管理游戏状态（开始、暂停、结束）
// - 管理分数和最高分（使用 PlayerPrefs 持久化）
// - 管理20秒倒计时
// - 触发游戏结束弹窗（胜利/失败）
// - 提供场景切换接口
//
// Non-Goals:
// - 不处理贪吃蛇移动逻辑（由 SnakeController 负责）
// - 不处理扑克牌逻辑（由 PokerManager 负责）
// - 不处理用户输入（由 TouchInputManager 负责）
// </summary>
```

### 2. 类注释

使用 XML 文档注释（三斜杠 `///`）：

```csharp
/// <summary>
/// 游戏全局管理器，负责游戏状态、分数、计时器和场景切换。
/// 使用单例模式确保全局唯一访问。
/// </summary>
/// <remarks>
/// 该类使用 DontDestroyOnLoad 在场景切换时不被销毁。
/// 通过 GameManager.Instance 访问单例实例。
/// </remarks>
public class GameManager : MonoBehaviour
{
    // ...
}
```

### 3. 方法注释

包含：
- **功能描述**
- **参数说明**（如有）
- **返回值说明**（如有）
- **异常说明**（如有）
- **示例代码**（如果复杂）

```csharp
/// <summary>
/// 增加玩家分数，并在超过最高分时自动保存。
/// </summary>
/// <param name="points">要增加的分数值，必须为正整数。</param>
/// <remarks>
/// 如果当前分数超过历史最高分，会自动调用 SaveHighScore()。
/// 分数变化会触发 ScoreDisplay 自动更新UI。
/// </remarks>
/// <example>
/// <code>
/// GameManager.Instance.AddScore(10);  // 增加10分
/// </code>
/// </example>
public void AddScore(int points)
{
    currentScore += points;
    
    if (currentScore > highScore)
    {
        highScore = currentScore;
        SaveHighScore();
    }
}
```

### 4. 字段/属性注释

使用 XML 注释或行内注释：

```csharp
/// <summary>
/// 当前游戏分数。
/// </summary>
[Header("游戏状态")]
public int currentScore = 0;

/// <summary>
/// 历史最高分，从 PlayerPrefs 加载。
/// </summary>
public int highScore = 0;

/// <summary>
/// 游戏时长（秒），默认20秒。
/// </summary>
public float gameTime = 20f;

/// <summary>
/// 剩余时间（秒），每帧递减。
/// </summary>
public float remainingTime = 20f;

/// <summary>
/// 游戏是否正在运行。
/// false 时计时器停止，蛇停止移动。
/// </summary>
public bool isGameRunning = false;
```

### 5. 复杂逻辑注释

在代码块前添加说明：

```csharp
// 检查2x2区域是否都为空（避免重叠）
for (int x = 0; x < 2; x++)
{
    for (int y = 0; y < 2; y++)
    {
        Vector2Int checkPos = bottomLeft + new Vector2Int(x, y);
        if (IsCellOccupied(checkPos))
            return false;
    }
}
```

### 6. TODO 注释

```csharp
// TODO(username): 添加难度设置，支持可变速度
// TODO(username): 实现排行榜系统
```

---

## 📋 完整示例：GameManager.cs

```csharp
// Copyright 2026 NewbieAudioKid
//
// Licensed under the MIT License.
//
// <summary>
// GameManager 管理游戏的全局状态、计时器、分数和场景切换。
// 使用 DontDestroyOnLoad 单例模式确保在场景切换时持久存在。
//
// Goals:
// - 管理游戏状态（开始、暂停、结束）
// - 管理分数和最高分（使用 PlayerPrefs 持久化）
// - 管理20秒倒计时
// - 触发游戏结束弹窗（胜利/失败）
// - 提供场景切换接口
//
// Non-Goals:
// - 不处理贪吃蛇移动逻辑（由 SnakeController 负责）
// - 不处理扑克牌逻辑（由 PokerManager 负责）
// - 不处理用户输入（由 TouchInputManager 负责）
// </summary>

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 游戏全局管理器，负责游戏状态、分数、计时器和场景切换。
/// 使用单例模式确保全局唯一访问。
/// </summary>
/// <remarks>
/// 该类使用 DontDestroyOnLoad 在场景切换时不被销毁。
/// 通过 GameManager.Instance 访问单例实例。
/// </remarks>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 单例实例，全局访问点。
    /// </summary>
    public static GameManager Instance;

    // ==================== 游戏状态 ====================
    
    /// <summary>
    /// 当前游戏分数。
    /// </summary>
    [Header("游戏状态")]
    public int currentScore = 0;

    /// <summary>
    /// 历史最高分，从 PlayerPrefs 加载。
    /// </summary>
    public int highScore = 0;

    /// <summary>
    /// 游戏时长（秒），默认20秒。
    /// </summary>
    public float gameTime = 20f;

    /// <summary>
    /// 剩余时间（秒），每帧递减。
    /// </summary>
    public float remainingTime = 20f;

    /// <summary>
    /// 游戏是否正在运行。
    /// false 时计时器停止，蛇停止移动。
    /// </summary>
    public bool isGameRunning = false;

    // ==================== 生命周期 ====================

    /// <summary>
    /// Unity Awake 回调，初始化单例实例。
    /// 如果已存在实例，销毁当前对象。
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

    /// <summary>
    /// Unity OnEnable 回调，注册场景加载事件。
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unity OnDisable 回调，注销场景加载事件。
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载完成的回调。
    /// 如果加载的是 GameScene，自动开始游戏。
    /// </summary>
    /// <param name="scene">加载的场景。</param>
    /// <param name="mode">加载模式。</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            StartGame();
        }
    }

    // ==================== 游戏控制 ====================

    /// <summary>
    /// 开始游戏，重置分数和计时器。
    /// </summary>
    /// <remarks>
    /// 调用此方法会：
    /// 1. 重置当前分数为0
    /// 2. 重置剩余时间为 gameTime
    /// 3. 启动计时器协程
    /// 4. 设置 isGameRunning 为 true
    /// </remarks>
    public void StartGame()
    {
        currentScore = 0;
        remainingTime = gameTime;
        isGameRunning = true;
        StartCoroutine(GameTimer());
    }

    /// <summary>
    /// 暂停游戏，停止时间流逝。
    /// </summary>
    /// <remarks>
    /// 设置 Time.timeScale = 0，暂停所有物理和动画。
    /// </remarks>
    public void PauseGame()
    {
        isGameRunning = false;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 恢复游戏，继续时间流逝。
    /// </summary>
    public void ResumeGame()
    {
        isGameRunning = true;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 游戏计时器协程，每帧递减 remainingTime。
    /// 时间到达0时触发游戏胜利。
    /// </summary>
    /// <returns>协程迭代器。</returns>
    IEnumerator GameTimer()
    {
        while (remainingTime > 0 && isGameRunning)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        if (isGameRunning)
        {
            // 时间到，游戏胜利
            EndGame(true);
        }
    }

    // ==================== 分数管理 ====================

    /// <summary>
    /// 增加玩家分数，并在超过最高分时自动保存。
    /// </summary>
    /// <param name="points">要增加的分数值。</param>
    /// <remarks>
    /// 如果当前分数超过历史最高分，会自动调用 SaveHighScore()。
    /// </remarks>
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
    /// 保存最高分到 PlayerPrefs。
    /// </summary>
    /// <remarks>
    /// 使用键 "SnakeHighScore" 存储。
    /// </remarks>
    void SaveHighScore()
    {
        PlayerPrefs.SetInt("SnakeHighScore", highScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 从 PlayerPrefs 加载最高分。
    /// 如果不存在，默认为0。
    /// </summary>
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("SnakeHighScore", 0);
    }

    // ==================== 游戏结束处理 ====================

    /// <summary>
    /// 结束游戏，显示胜利或失败弹窗。
    /// </summary>
    /// <param name="isWin">
    /// true 表示胜利（时间到），false 表示失败（撞到障碍物）。
    /// </param>
    public void EndGame(bool isWin)
    {
        isGameRunning = false;
        Time.timeScale = 1f; // 恢复时间流速（防止暂停状态）
        
        if (isWin)
            TriggerVictory();
        else
            TriggerGameOver();
    }

    /// <summary>
    /// 触发胜利弹窗，显示最终分数。
    /// </summary>
    /// <remarks>
    /// 调用 GameResultPopup.Instance.ShowVictory()。
    /// 如果 GameResultPopup 不存在，输出错误日志。
    /// </remarks>
    public void TriggerVictory()
    {
        if (GameResultPopup.Instance != null)
            GameResultPopup.Instance.ShowVictory();
        else
            Debug.LogError("❌ 场景里找不到 GameResultPopup！");
    }

    /// <summary>
    /// 触发失败弹窗。
    /// </summary>
    /// <remarks>
    /// 调用 GameResultPopup.Instance.ShowGameOverDelayed()。
    /// </remarks>
    public void TriggerGameOver()
    {
        if (GameResultPopup.Instance != null)
            GameResultPopup.Instance.ShowGameOverDelayed();
        else
            Debug.LogError("❌ 场景里找不到 GameResultPopup！");
    }

    // ==================== 场景切换 ====================

    /// <summary>
    /// 重新开始游戏，重新加载当前场景。
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 返回主菜单，加载 MenuScene。
    /// </summary>
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}
```

---

## 🎯 关键要点

### ✅ DO（应该做）

1. **使用 XML 文档注释**（三斜杠 `///`）
2. **为所有 public 成员添加注释**
3. **使用 `<summary>`、`<param>`、`<returns>` 标签**
4. **在复杂逻辑前添加说明性注释**
5. **保持注释简洁明了**

### ❌ DON'T（不要做）

1. **不要注释显而易见的代码**
   ```csharp
   // ❌ 错误示例
   i++; // i加1
   
   // ✅ 正确做法：不需要注释
   i++;
   ```

2. **不要使用过时的注释**
   ```csharp
   // ❌ 错误示例
   /// <summary>
   /// 返回玩家健康值（已废弃，现在返回分数）
   /// </summary>
   ```

3. **不要在注释中包含代码**
   ```csharp
   // ❌ 错误示例
   // int oldScore = currentScore + points;
   // currentScore = oldScore;
   ```

---

## 📚 参考资源

- [Google C# Style Guide](https://google.github.io/styleguide/csharp-style.html)
- [Microsoft C# XML Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)
- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)

---

## 🔧 应用到其他脚本

所有核心脚本都应遵循此规范：

- ✅ `GameManager.cs` - 已完成
- ✅ `SnakeGridManager.cs` - 需要添加
- ✅ `SnakeController.cs` - 需要添加
- ✅ `TouchInputManager.cs` - 需要添加
- ✅ `PokerManager.cs` - 需要添加
- ✅ `VFXManager.cs` - 需要添加

每个脚本都应包含：
1. 文件头注释（TL;DR, Goals, Non-Goals）
2. 类级 XML 注释
3. 所有 public 方法的 XML 注释
4. 关键字段的注释
5. 复杂逻辑的行内注释

---

完成注释后，代码的可读性和可维护性将大幅提升！📖✨

