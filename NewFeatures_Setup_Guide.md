# 新功能设置指南 - 动态速度、弹性动画、粒子特效

## 🎉 三个新功能已完成！

### ✅ 功能 1：拖拽距离控制蛇速度

**效果**：拖拽距离越长，蛇移动越快；松开手指恢复正常速度

### ✅ 功能 2：ComboText 弹性动画

**效果**：组合文字以弹性效果弹出（Elastic），并带淡出效果退出

### ✅ 功能 3：粒子特效系统

**效果**：吃球、洗牌、胜利/失败时播放粒子特效

---

## 📂 新增/修改的文件

### 新增脚本

- `Assets/Scripts/GameScene/VFXManager.cs` - 特效管理器（新）

### 修改的脚本

- `Assets/Scripts/GameScene/TouchInputManager.cs` - 添加速度控制逻辑
- `Assets/Scripts/GameScene/SnakeController.cs` - 支持动态速度，添加吃球特效
- `Assets/Scripts/GameScene/PokerManager.cs` - 添加弹性动画，添加洗牌特效
- `Assets/Scripts/UIScripts/GameResultPopup.cs` - 添加胜利/失败特效

---

## 🔧 Unity 设置步骤

### 功能 1：拖拽距离控制速度

#### 已自动配置的参数

在 `TouchInputManager` 组件中已添加：

```
速度调节设置:
- Min Speed Multiplier: 0.5x  (短距离拖拽，较慢)
- Max Speed Multiplier: 2.0x  (长距离拖拽，最快)
- Max Drag Distance For Speed: 300px (达到最大速度的拖拽距离)
```

#### 测试方法

1. 运行游戏
2. 短距离拖拽：蛇移动较慢
3. 长距离拖拽：蛇移动加速
4. 松开手指：恢复正常速度

#### 参数调整（可选）

- **想要更快速度**：增加 `Max Speed Multiplier` (如 3.0x)
- **想要更灵敏**：减小 `Max Drag Distance For Speed` (如 200px)

---

### 功能 2：ComboText 弹性动画

#### 已自动配置的动画

在 `PokerManager` 组件中已添加：

```
组合文字动画设置:
- Pop In Duration: 0.3秒 (弹出时长)
- Pop Out Duration: 0.2秒 (退出时长)
- Pop In Curve: 弹性曲线 (0-120%-100%的回弹效果)
- Pop Out Curve: 淡出曲线 (缩小+透明度降低)
```

#### 测试方法

1. 运行游戏
2. 点击洗牌按钮
3. 观察组合文字弹出（带回弹效果）
4. 观察文字淡出（缩小+透明）

#### 调整动画（可选）

在 Unity Inspector 中：

1. 选中 `PokerManager` 组件
2. 展开 `组合文字动画设置`
3. 调整 `Pop In Curve` 曲线：
   - 添加关键帧让回弹更明显
   - 例如：在 70% 位置设置 1.3 值（更大的回弹）

---

### 功能 3：粒子特效系统

#### 第一步：创建 VFXManager

```
1. 在 Hierarchy 中：
   - 右键 GameRoot（或场景根对象）→ Create Empty
   - 重命名为 "VFXManager"

2. 添加组件：
   - Add Component → VFXManager
```

#### 第二步：选择粒子特效预制体

在 `VFXManager` 组件中拖拽以下预制体：

```
路径：Assets/Layer Lab/GUI Pro-CasualGame/Prefabs/Prefabs_DemoScene_Particle/

推荐选择：

1. Collect Ball VFX (吃球特效):
   - Particle_Stars_00  ⭐ (星星爆炸)
   - Particle_Shine_00  ✨ (闪光)
   - Particle_Coin_00   💰 (金币飞出)

2. Shuffle Cards VFX (洗牌特效):
   - Particle_Card_00   🎴 (卡牌飞舞)
   - Particle_Sparkle_00 ✨ (闪烁)

3. Victory VFX (胜利特效):
   - Particle_Firework_00 🎆 (烟花)
   - Particle_Confetti_00 🎊 (彩纸)
   - Particle_Trophy_00   🏆 (奖杯)

4. Game Over VFX (失败特效，可选):
   - Particle_Smoke_00    💨 (烟雾)
   - Particle_Lightning_00 ⚡ (闪电)
```

#### 第三步：配置 VFXManager

```
VFXManager 组件设置:
- Collect Ball VFX: 拖拽 Particle_Stars_00
- Shuffle Cards VFX: 拖拽 Particle_Card_00
- Victory VFX: 拖拽 Particle_Firework_00
- Game Over VFX: 拖拽 Particle_Smoke_00 (可选)

特效设置:
- VFX Lifetime: 2秒 (特效持续时间)
- Use Object Pooling: ✓ (性能优化)
```

#### 第四步：测试特效

```
1. 吃球特效:
   - 控制蛇吃到得分球
   - 应该在球的位置播放星星特效

2. 洗牌特效:
   - 点击洗牌按钮
   - 应该在扑克区域播放卡牌特效

3. 胜利特效:
   - 等待20秒时间到
   - 应该在屏幕中央播放烟花特效

4. 失败特效:
   - 撞到障碍物
   - 应该在屏幕中央播放烟雾特效
```

---

## 🎨 Layer Lab 粒子特效列表

Layer Lab 提供的粒子特效（部分）：

### ⭐ 收集/得分类

- `Particle_Stars_00` - 星星爆炸
- `Particle_Shine_00` - 闪光
- `Particle_Coin_00` - 金币
- `Particle_Gem_00` - 宝石
- `Particle_Heart_00` - 爱心

### 🎴 卡牌/UI 类

- `Particle_Card_00` - 卡牌飞舞
- `Particle_Sparkle_00` - 闪烁
- `Particle_Bubble_00` - 气泡

### 🎆 庆祝类

- `Particle_Firework_00` - 烟花
- `Particle_Confetti_00` - 彩纸
- `Particle_Trophy_00` - 奖杯
- `Particle_Crown_00` - 皇冠

### 💥 冲击/失败类

- `Particle_Smoke_00` - 烟雾
- `Particle_Lightning_00` - 闪电
- `Particle_Explosion_00` - 爆炸

**提示**：你可以在 Unity 中预览这些特效，选择最合适的！

---

## 🎮 功能演示

### 功能 1：动态速度

```
场景：玩家控制蛇移动

操作1（短距离拖拽）：
- 手指拖拽 50px
- 蛇速度：0.5x (较慢)

操作2（中距离拖拽）：
- 手指拖拽 150px
- 蛇速度：1.25x (正常偏快)

操作3（长距离拖拽）：
- 手指拖拽 300px+
- 蛇速度：2.0x (最快)

操作4（松开手指）：
- 松开
- 蛇速度：1.0x (恢复正常)
```

### 功能 2：弹性动画

```
场景：洗牌后检测到组合

时间线：
t=0.0s: 文字从0缩放开始
t=0.1s: 文字放大到 120% (超过目标)
t=0.2s: 文字回弹到 100% (到达目标)
t=0.3s: 弹出动画完成
t=0.8s: 开始退出动画 (0.3s弹入 + 0.5s停留)
t=0.9s: 文字缩小到 50%，透明度 50%
t=1.0s: 文字完全消失
```

### 功能 3：粒子特效

```
场景：吃到得分球

事件触发：
1. 蛇头碰到得分球
2. 播放星星爆炸特效（在球的位置）
3. 球消失
4. 分数+1
5. 2秒后特效自动回收
```

---

## ⚙️ 高级参数调整

### 调整速度曲线

在 `TouchInputManager` 中：

```csharp
// 线性速度增长（当前默认）
speedMultiplier = Mathf.Lerp(minSpeed, maxSpeed, dragDistance / maxDrag);

// 如果想要指数增长（拖越长加速越明显）：
float progress = dragDistance / maxDrag;
speedMultiplier = Mathf.Lerp(minSpeed, maxSpeed, progress * progress);

// 如果想要对数增长（初期加速快，后期慢）：
float progress = dragDistance / maxDrag;
speedMultiplier = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Sqrt(progress));
```

### 调整弹性动画曲线

在 Unity Inspector → PokerManager → Pop In Curve：

```
Elastic 回弹效果（推荐）：
关键帧：
- (0.0, 0.0)   → 起点
- (0.5, 1.2)   → 中途超过目标
- (0.7, 0.95)  → 回弹低于目标
- (1.0, 1.0)   → 最终目标

更强的回弹：
- (0.5, 1.4)   → 更夸张
- (0.7, 0.90)  → 回弹更低
```

### 调整特效生存时间

在 `VFXManager` 中：

```
VFX Lifetime: 2秒 (默认)

- 改为 1秒：特效消失快，性能好
- 改为 3秒：特效持续久，视觉丰富
```

---

## 🐛 常见问题

### 问题 1：速度控制不明显

**原因**：拖拽距离阈值太大

**解决**：

```
TouchInputManager:
- Max Drag Distance For Speed: 300 → 150
```

### 问题 2：弹性动画太快/太慢

**解决**：

```
PokerManager:
- Pop In Duration: 0.3 → 0.4 (更慢)
- Pop In Duration: 0.3 → 0.2 (更快)
```

### 问题 3：特效不显示

**检查清单**：

- [ ] VFXManager 是否在场景中？
- [ ] 粒子预制体是否正确拖拽？
- [ ] 粒子预制体的 Layer 是否正确？
- [ ] Camera 是否能看到特效位置？
- [ ] 粒子系统的 Sorting Layer 是否正确？

**解决**：

1. 检查粒子预制体的 `Sorting Layer` 设置
2. 确保 `Order in Layer` 足够高（如 100）
3. 检查 Camera 的 `Culling Mask` 是否包含粒子层

### 问题 4：特效位置不对

**原因**：世界坐标转换问题

**解决**：

```csharp
// 对于UI位置（扑克区域）
Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, uiTransform.position);
Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));

// 对于网格位置（蛇/球）
Vector3 worldPos = SnakeGridManager.Instance.GridToWorld(gridPos);
```

---

## ✅ 完整检查清单

- [ ] 拖拽短距离，蛇移动变慢
- [ ] 拖拽长距离，蛇移动加快
- [ ] 松开手指，蛇恢复正常速度
- [ ] 组合文字弹出有回弹效果
- [ ] 组合文字退出有淡出效果
- [ ] 吃球时播放特效
- [ ] 洗牌时播放特效
- [ ] 胜利时播放特效
- [ ] 特效自动回收（不卡顿）

---

## 🎨 推荐的特效组合

### 组合 1：清新风格

```
吃球: Particle_Stars_00 (星星)
洗牌: Particle_Sparkle_00 (闪烁)
胜利: Particle_Confetti_00 (彩纸)
失败: Particle_Smoke_00 (烟雾)
```

### 组合 2：华丽风格

```
吃球: Particle_Shine_00 (闪光)
洗牌: Particle_Card_00 (卡牌)
胜利: Particle_Firework_00 (烟花)
失败: Particle_Lightning_00 (闪电)
```

### 组合 3：可爱风格

```
吃球: Particle_Heart_00 (爱心)
洗牌: Particle_Bubble_00 (气泡)
胜利: Particle_Crown_00 (皇冠)
失败: 不使用特效
```

---

完成所有设置后，游戏会更加生动有趣！🎮✨

有任何问题请告诉我！
