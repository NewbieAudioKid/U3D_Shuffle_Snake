# 扑克牌显示问题调试清单

## 🔍 问题：扑克牌显示为白牌

---

## ✅ 检查步骤

### 1️⃣ 检查 Console 日志

运行游戏后，查看 Console 窗口：

**应该看到的日志**：

```
🎴 成功加载 54 张扑克牌
✅ 成功加载：Poke/png/1
✅ 成功加载：Poke/png/2
... (54条)
🔄 洗牌完成！当前牌：...
```

**如果看到警告**：

```
⚠️ 无法加载图片：Poke/png/1
```

→ 说明路径或资源配置有问题

---

### 2️⃣ 检查文件夹结构

确认文件夹结构正确：

```
Assets/
  Resources/
    Poke/
      png/           ← PNG文件必须在这里
        1.png
        2.png
        ...
        54.png
      cards.csv
```

**重要**：

- 必须在 `Resources` 文件夹下
- `png` 文件夹名称必须小写
- PNG 文件名必须是 `1.png`, `2.png` 等

---

### 3️⃣ 检查 PNG 导入设置

在 Unity 中：

1. **选中 `png` 文件夹**
2. **在 Inspector 中检查任意一张图片**（如 1.png）

**正确的设置**：

```
Texture Type: Sprite (2D and UI)  ← 必须是这个！
Sprite Mode: Single
Pixels Per Unit: 100
Filter Mode: Bilinear
Compression: None (或 Normal Quality)
```

**如果不是 Sprite 类型**：

1. 选中所有 PNG 文件（Ctrl/Cmd + A 在 png 文件夹中）
2. 在 Inspector 中修改 `Texture Type` 为 `Sprite (2D and UI)`
3. 点击 **Apply** 按钮
4. 等待 Unity 重新导入（可能需要几秒）

---

### 4️⃣ 检查 CardPrefab 配置

选中 `CardPrefab`：

**必须有 Image 组件**（不是 RawImage）：

```
Image Component:
  - Source Image: (可以为空)
  - Color: 白色 (R:255, G:255, B:255, A:255)
  - Material: None (Material)
  - Raycast Target: ✓
  - Preserve Aspect: ✓ (建议勾选)
```

**如果是 RawImage**：

1. 移除 RawImage 组件
2. Add Component → UI → Image

---

### 5️⃣ 检查 PokerManager 配置

选中 PokerSystem GameObject：

**在 PokerManager 组件中**：

```
Cards CSV: ✓ (已拖拽 cards.csv)
Card Container: ✓ (已拖拽)
Card Prefab: ✓ (已拖拽)
Shuffle Button: ✓ (已拖拽)
Combo Name Text: ✓ (已拖拽)
```

---

### 6️⃣ 手动测试资源加载

在 Unity 中测试资源是否能加载：

1. **打开 Console**
2. **点击 Console 右上角的 Clear 按钮**
3. **运行游戏**
4. **查看日志**

如果看到 54 条 `✅ 成功加载` 但还是白牌，继续下一步。

---

## 🛠️ 常见问题和解决方案

### 问题 A：Console 显示 "无法加载图片"

**原因**：路径不对或资源未正确导入

**解决方案**：

1. 确认 PNG 文件在 `Resources/Poke/png/` 文件夹
2. 重新导入资源：
   - 右键 `png` 文件夹 → Reimport
3. 检查文件名是否正确（1.png, 2.png...）

---

### 问题 B：日志显示 "成功加载" 但还是白牌

**原因 1**：PNG 不是 Sprite 类型
**解决方案**：

- 批量修改所有 PNG 的 Texture Type 为 `Sprite (2D and UI)`
- 点击 Apply

**原因 2**：Image 组件透明度问题
**解决方案**：

- 选中 CardPrefab
- 检查 Image 组件的 Color
- 确保 Alpha (A) = 255

**原因 3**：Canvas 渲染顺序问题
**解决方案**：

- 选中 Canvas
- 检查 Render Mode
- 确保 PokerArea 在正确的层级

---

### 问题 C：只显示第一张牌，其他都是白的

**原因**：Sprite 导入设置不一致

**解决方案**：

1. 选中 `png` 文件夹中所有 PNG
2. 统一设置 Texture Type
3. 确保所有文件都 Apply 成功

---

## 🔧 终极解决方案：重新配置资源

如果以上都不行，执行完整重置：

### 步骤 1：删除旧的 .meta 文件

```
1. 关闭 Unity
2. 删除 Assets/Resources/Poke/png/ 下所有 .meta 文件
3. 重新打开 Unity（会自动重新生成 .meta）
```

### 步骤 2：批量设置 Sprite

```
1. 选中 png 文件夹中所有 PNG
2. Inspector → Texture Type: Sprite (2D and UI)
3. Apply
4. 等待导入完成
```

### 步骤 3：验证加载

```
1. 在 Project 窗口中，双击任意 PNG
2. 应该在 Inspector 中看到预览图
3. 如果看不到预览图，说明导入失败
```

---

## 📝 报告问题时需要的信息

如果还是不行，提供以下信息：

1. **Console 日志截图**（特别是加载相关的）
2. **任意一张 PNG 的 Inspector 截图**
3. **CardPrefab 的 Inspector 截图**
4. **PokerManager 的 Inspector 截图**

---

## 💡 快速测试方法

临时添加测试代码到 `PokerManager.cs` 的 `Start()` 方法：

```csharp
void Start()
{
    // 测试单张图片加载
    Sprite testSprite = Resources.Load<Sprite>("Poke/png/1");
    if (testSprite != null)
    {
        Debug.Log("✅ 测试成功！Sprite 加载正常");
        Debug.Log($"Sprite 名称: {testSprite.name}");
        Debug.Log($"Sprite 大小: {testSprite.rect.width} x {testSprite.rect.height}");
    }
    else
    {
        Debug.LogError("❌ 测试失败！无法加载 Sprite");
    }

    LoadCardsFromCSV();
    InitializeUI();
    ShuffleCards();
}
```

运行游戏，查看 Console 输出。

---

按照这个清单逐步检查，找出问题所在！
