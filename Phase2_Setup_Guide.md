# Phase 2 设置指南 - 贪吃蛇控制器

## ✅ 代码已完成

已创建的脚本：
1. **SnakeController.cs** - 贪吃蛇核心控制器
2. **TemporaryKeyboardInput.cs** - 临时键盘输入（用于测试）

---

## 🎮 Unity 中的设置步骤

### 1️⃣ 创建 Snake GameObject

在 Hierarchy 中：

```
1. 右键 Gameboard → Create Empty
2. 重命名为 "Snake"
3. 确保 Position 为 (0, 0, 0)
```

### 2️⃣ 添加组件到 Snake

选中 Snake GameObject：

```
1. Add Component → SnakeController
2. 配置参数：
   - Move Speed: 0.2 (移动速度，可调整)
   - Initial Length: 3 (初始长度)
   - Current Direction: X=0, Y=1 (向上)
```

### 3️⃣ 添加临时输入控制

选中 Gameboard（或其他GameObject）：

```
1. Add Component → TemporaryKeyboardInput
   (这是临时测试用的键盘控制，后续会替换为触摸输入)
```

### 4️⃣ 删除测试生成代码（可选）

如果不想看到随机生成的障碍物和得分球，可以：

在 `SnakeGridManager.cs` 中注释掉测试代码：

```csharp
void Start() 
{ 
    InitializeGrid();
    
    // 注释掉这一行
    // StartCoroutine(TestGeneration());
}
```

---

## 🎯 测试运行

### 运行游戏

点击 Play 按钮，应该看到：

✅ 蛇在网格中央生成（3节长）
✅ 蛇自动向上移动
✅ 可以用键盘控制：
   - **W** 或 **↑** → 向上
   - **S** 或 **↓** → 向下
   - **A** 或 **←** → 向左
   - **D** 或 **→** → 向右
   - **支持斜角移动**：同时按两个键（如 W+D = 右上）

### 测试功能

1. **基础移动** ✓
   - 蛇会自动向上移动
   - 按方向键可以改变方向

2. **边界穿越** ✓
   - 从左边出去，从右边回来
   - 上下有边界（撞墙会死亡）

3. **碰撞检测** ✓
   - 撞到障碍物 → Game Over
   - 撞到自己 → Game Over
   - 吃到得分球 → 增长 + 加分

4. **增长机制** ✓
   - 吃球后蛇会增加一节

---

## 🐛 可能的问题

### 问题1：蛇不显示
**检查**：
- Gameboard 的 SnakeGridManager 是否配置了预制体？
- Snake Head Prefab 和 Snake Body Prefab 是否拖拽正确？

### 问题2：蛇移动太快/太慢
**调整**：
- 在 Snake 的 SnakeController 组件中
- 修改 `Move Speed` 参数
- 数值越小 = 移动越快

### 问题3：蛇的位置不对
**检查**：
- Gameboard 的 Grid Offset 是否设置正确？
- 网格是否对齐红色区域？

### 问题4：Console 报错
**常见错误**：
- "SnakeGridManager 未找到" → 确保 Gameboard 有 SnakeGridManager 组件
- 预制体未找到 → 检查 Inspector 中的预制体引用

---

## 🎨 调整建议

### 蛇的速度
```
- 慢速：0.3 - 0.4
- 中速：0.2 (默认)
- 快速：0.1 - 0.15
```

### 蛇的初始长度
```
- 短：3 (默认)
- 中：5
- 长：8
```

---

## 📸 完成后

测试通过后告诉我：
1. 蛇是否正常移动？
2. 键盘控制是否响应？
3. 吃球是否增长？
4. 碰撞是否触发 Game Over？

然后我们进入 **Phase 3：触摸拖拽输入系统** 📱

