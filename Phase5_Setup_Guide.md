# Phase 5 设置指南 - 扑克牌管理和组合识别系统

## ✅ 代码已完成

已创建的脚本：

1. **PokerCardData.cs** - 扑克牌数据结构
2. **PokerComboDetector.cs** - 组合识别算法
3. **PokerManager.cs** - 扑克牌主管理器

---

## 🎴 功能特性

### ✨ 核心功能

- ✅ 加载 54 张扑克牌（从 cards.csv）
- ✅ 随机洗牌（每次抽 6 张）
- ✅ 横向显示 6 张牌
- ✅ 点击洗牌（0.1 秒 CD）
- ✅ 组合识别（斗地主规则）
- ✅ 显示组合名称（0.5 秒后消失）
- ✅ 自动生成障碍物/得分球

### 🎯 支持的组合类型

| 组合      | 英文名          | 效果                        |
| --------- | --------------- | --------------------------- |
| 对子/三张 | PAIR/THREE      | 20%生成障碍物，80%生成 1 球 |
| 三带一    | THREE WITH ONE  | 生成 10 个球                |
| 三带二    | THREE WITH PAIR | 生成 10 个球                |
| 顺子      | STRAIGHT        | 生成 20 个球                |
| 连对      | DOUBLE STRAIGHT | 生成 20 个球                |
| 飞机      | PLANE           | 生成 40 个球                |
| 四带二    | FOUR WITH TWO   | 生成 40 个球                |

---

## 🔧 Unity 中的设置步骤

### 1️⃣ 创建 PokerArea UI

在 Hierarchy 中：

```
1. 右键 Canvas → UI → Panel
2. 重命名为 "PokerArea"
3. 设置为屏幕下方20%区域：
   - Anchor Presets: Bottom Stretch
   - Pos Y: 0
   - Height: 根据屏幕调整（建议200-300）
   - 背景色：可选（半透明黑色等）
```

### 2️⃣ 创建卡牌容器

在 PokerArea 下：

```
1. 右键 PokerArea → Create Empty
2. 重命名为 "CardContainer"
3. Add Component → Horizontal Layout Group
   - Spacing: 10
   - Child Alignment: Middle Center
   - Control Child Size: 都不勾选
```

### 3️⃣ 创建卡牌预制体

```
1. Hierarchy → UI → Image
2. 重命名为 "CardPrefab"
3. 设置：
   - Width: 80
   - Height: 120
   - Preserve Aspect: ✓ (勾选)
4. 拖拽到 Assets/Prefabs/ 文件夹
5. 删除场景中的实例
```

### 4️⃣ 创建洗牌按钮

在 PokerArea 下：

```
1. 右键 PokerArea → UI → Button - TextMeshPro
2. 重命名为 "ShuffleButton"
3. 设置位置（在PokerArea右侧）
4. 按钮文字改为 "Shuffle" 或 "洗牌"
```

### 5️⃣ 创建组合名称文本

在 Canvas 中：

```
1. 右键 Canvas → UI → Text - TextMeshPro
2. 重命名为 "ComboNameText"
3. 设置：
   - Font: LilitaOne-Regular
   - Font Size: 72
   - Alignment: Center
   - Color: Yellow (或其他醒目颜色)
   - Position: 屏幕中央
4. Canvas Group（可选）：
   - Interactable: 关闭
   - Block Raycasts: 关闭
   （确保不挡住其他操作）
```

### 6️⃣ 添加 PokerManager 组件

在 Hierarchy 中：

```
1. 右键 GameRoot（或 Gameboard）→ Create Empty
2. 重命名为 "PokerSystem"
3. Add Component → PokerManager
```

### 7️⃣ 配置 PokerManager

在 **PokerManager** 组件中：

```
扑克牌配置：
- Cards CSV: 拖拽 Resources/Poke/cards.csv

UI引用：
- Card Container: 拖拽 CardContainer
- Card Prefab: 拖拽 CardPrefab 预制体
- Shuffle Button: 拖拽 ShuffleButton
- Combo Name Text: 拖拽 ComboNameText

显示设置：
- Card Spacing: 100 (卡牌间距)
- Combo Text Display Time: 0.5 (文字显示时间)

洗牌设置：
- Shuffle Cooldown: 0.1 (洗牌CD)
```

---

## 🎮 测试运行

### 运行游戏

点击 Play，应该看到：

✅ 屏幕下方显示 6 张扑克牌
✅ 点击 Shuffle 按钮可以洗牌
✅ 如果有组合，屏幕中央显示组合名称
✅ 对应生成障碍物或得分球

### 测试功能

1. **洗牌测试** ✓

   - 点击洗牌按钮
   - 卡牌刷新
   - 0.1 秒后可以再次点击

2. **组合识别** ✓

   - 多次洗牌，观察不同组合
   - 检查 Console 输出的组合类型

3. **名称显示** ✓

   - 有组合时显示英文名称
   - 0.5 秒后自动消失

4. **地图生成** ✓
   - 对子/三张：可能生成障碍物或 1 个球
   - 高级组合：生成多个得分球

---

## 🐛 可能的问题

### 问题 1：卡牌不显示

**检查**：

- Cards CSV 是否正确拖拽？
- 图片路径是否正确？（Resources/Poke/xxx.png）
- CardPrefab 是否正确配置？

### 问题 2：组合名称不显示

**检查**：

- ComboNameText 是否拖拽正确？
- Font 是否设置？
- Canvas 是否在最前面？

### 问题 3：点击按钮没反应

**检查**：

- Shuffle Button 是否拖拽正确？
- Button 组件是否存在？
- EventSystem 是否存在？（Canvas 应该自动创建）

---

## ⚙️ 参数调整

### 卡牌间距

`Card Spacing`

- 默认：100
- 调整：根据屏幕宽度调整
- 公式：(屏幕宽 - 卡宽 ×6) / 7

### 组合显示时间

`Combo Text Display Time`

- 默认：0.5 秒
- 可调整为 0.3 - 1.0 秒

### 洗牌冷却

`Shuffle Cooldown`

- 默认：0.1 秒
- 按需求可以调整为 0.05 - 0.2 秒

---

## 📝 下一步

Phase 5 完成后，进入 **Phase 6**：

- 完善扑克牌 → 地图生成联动
- 添加更多生成规则

---

完成设置并测试通过后，告诉我结果！🎴
