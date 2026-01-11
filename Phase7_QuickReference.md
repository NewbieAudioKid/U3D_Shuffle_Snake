# Phase 7 - UI特性快速参考

## 📋 三个新UI特性总结

### ✅ 特性 1：倒计时显示
**位置**：`Canvas → Top → top_level → level_name1`

**实现**：
- 脚本：`TimerDisplay.cs`
- 功能：显示20秒倒计时（20 → 19 → 18 ... → 0）
- 特效：剩余5秒时数字变红色

**Unity设置**：
```
1. 选中 level_name1
2. Add Component → TimerDisplay
3. 拖拽 level_name1 自己到 Timer Text 字段
```

---

### ✅ 特性 2：分数显示
**位置**：`Canvas → Top → top_money → money_Panel → Money_text`

**实现**：
- 脚本：`ScoreDisplay.cs`
- 功能：实时显示当前分数
- 更新：吃得分球时自动增加

**Unity设置**：
```
1. 选中 Money_text
2. Add Component → ScoreDisplay
3. 拖拽 Money_text 自己到 Score Text 字段
```

---

### ✅ 特性 3：胜利弹窗显示分数
**位置**：`Canvas → popup_gameResult → Container_window → Txt_title`

**实现**：
- 脚本：`GameResultPopup.cs`（已更新）
- 胜利：显示 "Victory!" + "Score: XX"
- 失败：显示 "Failed"（不显示分数）

**Unity设置**：
```
1. 在 Container_window 下创建新的 TextMeshPro：
   - 名称：ScoreText
   - 位置：在 Txt_title 下方（Y: -50）
   - 文字：Score: 0
   - 字体大小：48
   - 颜色：Yellow

2. 选中 popup_gameResult
3. 在 GameResultPopup 组件中：
   - 拖拽 ScoreText 到 Score Text 字段
```

---

## 🎮 游戏逻辑流程

### 游戏开始
```
1. 加载 GameScene
2. GameManager.StartGame() 自动调用
3. 倒计时开始：20 → 19 → 18 ...
4. 分数重置：0
```

### 游戏进行中
```
- 倒计时每秒-1
- 吃球分数+1
- 时间≤5秒，倒计时变红
```

### 游戏结束

#### 时间到（胜利）
```
GameManager.EndGame(true)
  → GameResultPopup.ShowVictory()
  → 显示 "Victory!"
  → 显示 "Score: XX"
```

#### 撞障碍/自己（失败）
```
GameManager.EndGame(false)
  → GameResultPopup.ShowGameOver()
  → 显示 "Failed"
  → 不显示分数
```

---

## 📂 相关文件

### 新增脚本
- `Assets/Scripts/UIScripts/TimerDisplay.cs`
- `Assets/Scripts/UIScripts/ScoreDisplay.cs`

### 修改的脚本
- `Assets/Scripts/UIScripts/GameResultPopup.cs`
  - 添加 `scoreText` 字段
  - 修改 `ShowVictory()` 和 `ShowGameOver()` 的标题
  - 在胜利时显示分数，失败时隐藏分数

- `Assets/Scripts/GameManager.cs`
  - 添加 `OnSceneLoaded()` 事件监听
  - GameScene 加载时自动调用 `StartGame()`

### 相关文档
- `Phase7_Setup_Guide.md` - 完整设置指南

---

## 🔧 快速测试步骤

1. ✅ 运行游戏
2. ✅ 检查倒计时从20开始
3. ✅ 检查分数从0开始
4. ✅ 吃球后分数增加
5. ✅ 等待20秒，弹出胜利窗口并显示分数
6. ✅ 撞障碍，弹出失败窗口（无分数）

---

## 🎨 UI层级结构

```
Canvas
├── Top
│   ├── top_level
│   │   └── level_name1  ← 添加 TimerDisplay
│   └── top_money
│       └── money_Panel
│           └── Money_text  ← 添加 ScoreDisplay
└── popup_gameResult  ← GameResultPopup组件
    └── Container_window
        ├── Txt_title  (显示 "Victory!" 或 "Failed")
        └── ScoreText  (新建，显示 "Score: XX")
```

---

## ⚙️ 参数调整

| 参数 | 位置 | 默认值 | 说明 |
|------|------|--------|------|
| Game Time | GameManager | 20 | 游戏总时长（秒） |
| Warning Threshold | TimerDisplay | 5 | 倒计时变红的时间 |
| Normal Color | TimerDisplay | White | 正常时倒计时颜色 |
| Warning Color | TimerDisplay | Red | 警告时倒计时颜色 |
| Prefix | ScoreDisplay | "" | 分数前缀（如 "Score: "） |
| Suffix | ScoreDisplay | "" | 分数后缀（如 " pts"） |

---

完成设置后运行游戏测试！🎮

