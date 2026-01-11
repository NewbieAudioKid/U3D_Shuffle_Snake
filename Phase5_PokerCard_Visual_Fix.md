# 扑克牌显示优化指南

## 🎯 解决两个问题

1. ✅ 扑克牌背景透明 → 添加白色底
2. ✅ 游戏视角中模糊/马赛克 → 调整 Sprite 导入设置

---

## 📦 第一步：优化 Sprite 导入设置（解决模糊问题）

### 选中所有扑克牌 PNG 文件

```
路径：Assets/Resources/Poke/png/
```

**在 Unity 中操作：**

1. 打开 `Assets/Resources/Poke/png/` 文件夹
2. 全选所有 PNG 文件（Cmd+A）
3. 在 Inspector 面板中设置：

```
Texture Type: Sprite (2D and UI)  ✓

Sprite Mode: Single

Pixels Per Unit: 100  ✓

Mesh Type: Full Rect

Extrude Edges: 0

Pivot: Center

Generate Mip Maps: ❌ 取消勾选  <--- 重要！

Filter Mode: Bilinear  ✓  <--- 关键设置！
（如果还是模糊可以尝试 Trilinear）

Compression: None  ✓  <--- 重要！
（如果文件太大可以选 High Quality）

Max Size: 2048  ✓  <--- 根据素材大小调整
（如果素材更大可以选 4096）

Format: RGBA 32 bit  ✓  <--- 保证高清
```

4. 点击 **Apply** 按钮

---

## 🎴 第二步：重新创建 CardPrefab（带白色背景）

### 删除旧的 CardPrefab

```
1. 在 Assets/Prefabs/ 中删除旧的 CardPrefab
2. 在场景中删除所有残留的卡牌对象
```

### 创建新的 CardPrefab 结构

**在 Hierarchy 中操作：**

```
1. 右键 Canvas → UI → Image
2. 重命名为 "CardPrefab"
```

### 配置 CardPrefab（父对象 - 白色背景）

在 Inspector 中设置：

```
Rect Transform:
- Width: 120   ✓ (根据需要调整)
- Height: 180  ✓ (根据需要调整)
- Anchor Presets: Middle Center

Image 组件:
- Source Image: 留空（或删除）✓
- Color: White (255, 255, 255, 255) ✓  <--- 这是白色底
- Material: None (Default)
- Raycast Target: ✓ 勾选（用于点击检测）
```

### 添加卡牌图片子对象

```
1. 右键 CardPrefab → UI → Image
2. 重命名为 "CardImage"
```

### 配置 CardImage（子对象 - 扑克牌图片）

在 Inspector 中设置：

```
Rect Transform:
- Anchors: Stretch (全方向拉伸)
  - Left: 5
  - Right: 5
  - Top: 5
  - Bottom: 5
  （这样图片会比背景小一圈，露出白边）

Image 组件:
- Source Image: 留空 ✓  <--- 代码会动态设置
- Color: White (255, 255, 255, 255) ✓
- Material: None (Default)
- Raycast Target: ❌ 取消勾选（不需要点击）
- Preserve Aspect: ✓ 勾选  <--- 保持长宽比
```

### 保存为预制体

```
1. 将 CardPrefab 拖拽到 Assets/Prefabs/ 文件夹
2. 删除场景中的实例
```

---

## 🔧 第三步：修改 PokerManager.cs 代码

### 更新 UpdateCardDisplay() 方法

找到 `PokerManager.cs` 中的 `UpdateCardDisplay()` 方法，替换为以下代码：

```csharp
/// <summary>
/// 更新卡牌显示
/// </summary>
void UpdateCardDisplay()
{
    // 清除旧的卡牌对象
    foreach (GameObject obj in cardObjects)
    {
        if (obj != null)
            Destroy(obj);
    }
    cardObjects.Clear();

    if (cardContainer == null || cardPrefab == null)
    {
        Debug.LogWarning("⚠️ 卡牌容器或预制体未设置");
        return;
    }

    // 禁用自动布局（如果有的话）
    var layoutGroup = cardContainer.GetComponent<UnityEngine.UI.LayoutGroup>();
    if (layoutGroup != null)
    {
        layoutGroup.enabled = false;
    }

    // 创建新的卡牌对象
    for (int i = 0; i < currentCards.Count; i++)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);

        // 设置位置（横向排列 + 可调偏移）
        RectTransform rect = cardObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            float xPos = (i - 2.5f) * cardSpacing + cardXOffset; // 居中排列 + X偏移
            float yPos = cardYOffset;                             // Y偏移
            rect.anchoredPosition = new Vector2(xPos, yPos);
        }

        // 设置图片（在子对象 CardImage 上）
        Transform cardImageTransform = cardObj.transform.Find("CardImage");
        if (cardImageTransform != null)
        {
            Image cardImage = cardImageTransform.GetComponent<Image>();
            if (cardImage != null)
            {
                if (currentCards[i].sprite != null)
                {
                    cardImage.sprite = currentCards[i].sprite;
                    Debug.Log($"✅ 第{i+1}张牌设置成功：{currentCards[i].fullName} → {currentCards[i].sprite.name}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ 第{i+1}张牌 Sprite 为空：{currentCards[i].fullName}");
                }
            }
            else
            {
                Debug.LogError($"❌ CardImage 上没有 Image 组件！");
            }
        }
        else
        {
            Debug.LogError($"❌ CardPrefab 下没有找到 CardImage 子对象！");
            Debug.LogError($"   请检查 CardPrefab 的结构是否正确");
        }

        cardObjects.Add(cardObj);
    }
}
```

---

## 🎯 第四步：调整 Canvas Scaler（优化整体清晰度）

在 Canvas 对象上：

```
Canvas Scaler 组件设置:

UI Scale Mode: Scale With Screen Size  ✓

Reference Resolution:
- X: 1920  ✓ (根据目标分辨率调整)
- Y: 1080  ✓

Screen Match Mode: Match Width Or Height

Match: 0.5  ✓ (平衡宽高)

Reference Pixels Per Unit: 100
```

---

## 🎮 测试效果

运行游戏后应该看到：

✅ 每张扑克牌都有白色边框（底）
✅ 卡牌图片清晰不模糊
✅ 图片保持原始长宽比
✅ 点击洗牌可以正常刷新

---

## 🔍 如果还是模糊的话...

### 方案 A：提高 Max Size

```
选中所有 PNG → Inspector
Max Size: 4096 (或更高)
→ Apply
```

### 方案 B：调整 Filter Mode

```
选中所有 PNG → Inspector
Filter Mode: Trilinear (更平滑)
→ Apply
```

### 方案 C：检查 Camera 设置

```
Main Camera → Inspector
Projection: Orthographic  ✓
Size: 5 (或根据需要调整)
```

### 方案 D：检查卡牌尺寸

```
如果卡牌在场景中显示太大（被拉伸），会导致模糊

解决方法：
1. 减小 CardPrefab 的 Width/Height
2. 或者增加 Card Spacing 避免拉伸
```

---

## 📐 推荐的卡牌尺寸

根据扑克牌标准比例（2:3）：

```
方案 1（小卡）：
- Width: 80
- Height: 120

方案 2（中卡）：
- Width: 100
- Height: 150

方案 3（大卡）：
- Width: 120
- Height: 180
```

根据屏幕宽度选择合适的尺寸，确保 6 张牌能完整显示。

---

## ✅ 完整检查清单

- [ ] 所有 PNG 的 Texture Type = Sprite (2D and UI)
- [ ] Filter Mode = Bilinear 或 Trilinear
- [ ] Compression = None 或 High Quality
- [ ] Max Size = 2048 或更高
- [ ] CardPrefab 有白色背景 Image
- [ ] CardPrefab 下有 CardImage 子对象
- [ ] CardImage 设置了 Preserve Aspect
- [ ] PokerManager.cs 代码已更新
- [ ] Canvas Scaler 已正确配置
- [ ] 卡牌尺寸合适（不会被过度拉伸）

---

## 🎴 最终效果图

应该看到：

```
┌───────────────┐
│ ╔═══════════╗ │ ← 白色边框
│ ║           ║ │
│ ║   扑克牌  ║ │ ← 高清图片
│ ║           ║ │
│ ╚═══════════╝ │
└───────────────┘
```

---

完成以上步骤后测试运行，如果还有问题请截图告诉我！🎴
