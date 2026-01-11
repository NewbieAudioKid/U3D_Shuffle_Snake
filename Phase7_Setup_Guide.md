# Phase 7 设置指南 - 20 秒计时器和游戏结束系统

## ✅ 代码已完成

已创建/更新的脚本：

1. **TimerDisplay.cs** - 倒计时显示器（新）
2. **ScoreDisplay.cs** - 分数显示器（新）
3. **GameResultPopup.cs** - 游戏结束弹窗（已更新）
4. **GameManager.cs** - 游戏管理器（已更新）

---

## 🎮 新增功能

### ✨ 功能特性

1. ✅ **20 秒倒计时**

   - 游戏开始时从 20 秒开始倒计时
   - 时间不足 5 秒时数字变红色警告
   - 时间到达 0 时游戏结束（胜利）

2. ✅ **实时分数显示**

   - 吃到得分球时分数增加
   - 分数实时更新显示

3. ✅ **游戏结束弹窗**
   - 胜利（时间到）：显示 "Victory!" + 最终分数
   - 失败（撞障碍/撞自己）：显示 "Failed"

---

## 🔧 Unity 中的设置步骤

### 第一步：设置倒计时显示

```
1. 在 Hierarchy 中找到：
   Canvas → Top → top_level → level_name1

2. 选中 level_name1（TextMeshProUGUI 组件）

3. Add Component → TimerDisplay

4. 在 TimerDisplay 组件中：
   - Timer Text: 拖拽 level_name1 自己 ✓
   - Normal Color: White (255, 255, 255)
   - Warning Color: Red (255, 0, 0)
   - Warning Threshold: 5 (剩余5秒时变红)
```

### 第二步：设置分数显示

```
1. 在 Hierarchy 中找到：
   Canvas → Top → top_money → money_Panel → Money_text

2. 选中 Money_text（TextMeshProUGUI 组件）

3. Add Component → ScoreDisplay

4. 在 ScoreDisplay 组件中：
   - Score Text: 拖拽 Money_text 自己 ✓
   - Prefix: 留空（或输入 "Score: "）
   - Suffix: 留空（或输入 " pts"）
```

### 第三步：更新游戏结束弹窗

```
1. 在 Hierarchy 中找到：
   Canvas → popup_gameResult

2. 选中 popup_gameResult，找到 GameResultPopup 组件

3. 在 Container_window 下创建分数文本：
   - 右键 Container_window → UI → Text - TextMeshPro
   - 重命名为 "ScoreText"

4. 设置 ScoreText：
   - Rect Transform:
     * Anchor: Middle Center
     * Pos X: 0
     * Pos Y: -50 (在标题下方)
     * Width: 400
     * Height: 80

   - TextMeshPro 设置:
     * Text: "Score: 0"
     * Font: LilitaOne-Regular
     * Font Size: 48
     * Alignment: Center
     * Color: Yellow (255, 255, 0) 或其他颜色

5. 返回 GameResultPopup 组件：
   - Score Text: 拖拽刚创建的 ScoreText ✓
```

### 第四步：确保 GameManager 存在

```
1. 检查场景中是否有 GameManager：
   - 如果没有：
     * Hierarchy 右键 → Create Empty
     * 重命名为 "GameManager"
     * Add Component → GameManager

   - 如果已存在：
     * 确保已经添加了 GameManager 组件

2. GameManager 组件设置：
   - Game Time: 20 (游戏时长20秒)
   - 其他参数保持默认
```

---

## 🎯 游戏流程

### 游戏开始

```
1. 加载 GameScene
2. GameManager 自动调用 StartGame()
3. 倒计时从 20 开始
4. 分数从 0 开始
5. 蛇开始移动
```

### 游戏进行中

```
- 倒计时每秒减少：20 → 19 → 18 ...
- 吃到得分球：分数 +1
- 剩余时间 ≤ 5 秒：数字变红色
```

### 游戏结束

#### 情况 1：时间到（胜利）

```
- 倒计时到达 0
- 弹出 "Victory!" 窗口
- 显示最终分数：Score: XX
- 显示按钮：Menu、Retry
```

#### 情况 2：撞到障碍物/自己（失败）

```
- 立即停止游戏
- 弹出 "Failed" 窗口
- 不显示分数
- 显示按钮：Menu、Retry
```

---

## 🎮 测试运行

### 测试步骤

1. **启动游戏**

   - 点击 Play
   - 确认倒计时显示 "20"
   - 确认分数显示 "0"

2. **测试倒计时**

   - 观察倒计时：20 → 19 → 18 ...
   - 等待到 5 秒以下，确认数字变红

3. **测试分数**

   - 控制蛇吃得分球
   - 确认分数增加

4. **测试胜利**

   - 等待 20 秒倒计时结束
   - 确认弹出 "Victory!" 窗口
   - 确认显示最终分数

5. **测试失败**
   - 控制蛇撞到障碍物
   - 确认弹出 "Failed" 窗口

---

## 🐛 可能的问题

### 问题 1：倒计时不显示

**检查**：

- level_name1 是否添加了 TimerDisplay 组件？
- Timer Text 是否正确拖拽？
- GameManager 是否存在且 isGameRunning = true？

### 问题 2：分数不更新

**检查**：

- Money_text 是否添加了 ScoreDisplay 组件？
- Score Text 是否正确拖拽？
- SnakeController 是否调用了 GameManager.AddScore()？

### 问题 3：游戏结束弹窗显示错误

**检查**：

- GameResultPopup 组件是否存在？
- Score Text 是否正确拖拽到 GameResultPopup？
- Container_window 下是否创建了 ScoreText？

### 问题 4：游戏不会自动开始

**检查**：

- GameManager 是否在场景中？
- GameManager 是否设置为 DontDestroyOnLoad？
- 场景名称是否为 "GameScene"？

---

## 🎨 UI 布局建议

### 倒计时（level_name1）

```
推荐设置：
- Font Size: 60-80
- Color: White (正常) / Red (警告)
- Alignment: Center
- 位置：屏幕顶部居中偏左
```

### 分数（Money_text）

```
推荐设置：
- Font Size: 50-70
- Color: Yellow 或 Gold
- Alignment: Center
- 位置：屏幕顶部居中偏右
```

### 胜利弹窗分数（ScoreText）

```
推荐设置：
- Font Size: 48
- Color: Yellow
- Alignment: Center
- 位置：在 "Victory!" 标题下方
```

---

## 📊 参数调整

### 修改游戏时长

在 GameManager 组件中：

```
Game Time: 20 (默认)
- 改为 30：更宽裕
- 改为 15：更紧张
- 改为 60：练习模式
```

### 修改警告时间

在 TimerDisplay 组件中：

```
Warning Threshold: 5 (默认)
- 改为 10：更早警告
- 改为 3：更晚警告
```

### 修改蛇的移动速度

在 SnakeController 组件中：

```
Move Speed: 0.2 (默认)
- 改为 0.15：更快
- 改为 0.3：更慢
```

---

## ✅ 完整检查清单

- [ ] TimerDisplay.cs 已添加到 level_name1
- [ ] ScoreDisplay.cs 已添加到 Money_text
- [ ] GameResultPopup 已更新并添加 ScoreText
- [ ] GameManager 存在且配置正确
- [ ] 倒计时正常显示和倒数
- [ ] 分数正常显示和增加
- [ ] 时间到时弹出胜利窗口并显示分数
- [ ] 撞障碍时弹出失败窗口
- [ ] Retry 按钮可以重新开始
- [ ] Menu 按钮可以返回菜单

---

## 🎉 Phase 7 完成！

完成以上设置后，你的贪吃蛇游戏就有完整的计时器和游戏结束系统了！

**下一步建议**：

- 添加音效（吃球、游戏结束）
- 添加粒子效果（得分、碰撞）
- 添加难度设置（时间、速度可调）
- 添加排行榜（保存历史最高分）

---

完成设置并测试通过后，告诉我结果！🎮
