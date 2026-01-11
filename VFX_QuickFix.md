# VFX 看不到 - 快速修复方案

## 🚀 已添加的代码修复

我已经在代码中添加了以下修复：

### ✅ 修复 1：强制设置 Z 坐标
所有特效的 Z 坐标现在都强制设置为 0，确保在摄像机视野内。

### ✅ 修复 2：添加详细调试日志
运行游戏时，Console 会显示：
- 特效是否成功播放
- 粒子系统状态
- 位置信息

---

## 🎯 现在请按以下步骤操作：

### Step 1: 设置粒子预制体的 Sorting Layer（最重要！）

```
对每个粒子预制体执行以下操作：

1. 在 Project 窗口中找到：
   Assets/Layer Lab/GUI Pro-CasualGame/Prefabs/Prefabs_DemoScene_Particle/

2. 选中你使用的粒子预制体（一个一个来）：
   ✓ Particle_Stars_00
   ✓ Particle_Card_00
   ✓ Particle_Firework_00
   ✓ Particle_Smoke_00
   ✓ Particle_Trail_00

3. 在 Inspector 中找到 Particle System 组件

4. 展开 Renderer 部分

5. 设置：
   ┌─────────────────────────────────┐
   │ Renderer                        │
   │ ├─ Render Mode: Billboard       │
   │ ├─ Sorting Layer: Default       │
   │ └─ Order in Layer: 999          │ ← 关键！
   └─────────────────────────────────┘

6. 对所有5个预制体重复此操作
```

**为什么是 999？**
- 数字越大，渲染层级越高
- 999 确保特效显示在所有游戏对象上面

---

### Step 2: 运行游戏并查看 Console

```
1. 点击 Unity 的 Play 按钮

2. 打开 Console 窗口（Window → General → Console）

3. 触发特效（吃球、洗牌等）

4. 查看 Console 输出：

   如果看到：
   ✅ 播放特效：Particle_Stars_00 位置：(x, y, 0)
   → 说明特效已经生成

   如果看到：
   ⚠️ Collect Ball VFX 预制体未设置！
   → 说明需要在 VFXManager 中拖拽预制体
```

---

### Step 3: 检查 VFXManager 的引用

```
1. 在 Hierarchy 中选中 VFXManager

2. 在 Inspector 中检查：
   ┌─────────────────────────────────┐
   │ VFX Manager (Script)            │
   │                                 │
   │ 粒子特效预制体（Layer Lab）：   │
   │ ├─ Collect Ball VFX: ⚪        │ ← 不能是 None!
   │ ├─ Shuffle Cards VFX: ⚪        │
   │ ├─ Victory VFX: ⚪              │
   │ ├─ Game Over VFX: ⚪            │
   │ └─ Snake Head Trail VFX: ⚪     │
   └─────────────────────────────────┘

3. 如果是 "None (GameObject)"：
   - 从 Project 窗口拖拽对应的粒子预制体
```

---

### Step 4: 简单测试

```
最简单的测试方法：

1. 在 Project 窗口中找到 Particle_Stars_00

2. 直接拖拽到 Hierarchy（场景中）

3. 设置 Transform → Position = (0, 0, 0)

4. 点击 Play

5. 如果能看到星星特效：
   → 说明粒子本身没问题
   → 问题在于 Sorting Layer 设置
   
6. 如果看不到：
   → 按 Step 1 设置 Order in Layer = 999
```

---

## 📊 诊断流程图

```
运行游戏
    ↓
Console 有 "✅ 播放特效" 日志？
    ├─ 有 → Hierarchy 的 VFX_Container 下有对象？
    │      ├─ 有 → Scene 视图能看到？
    │      │      ├─ 能 → 设置 Order in Layer = 999
    │      │      └─ 不能 → 检查粒子系统设置
    │      └─ 没有 → 检查代码调用
    │
    └─ 没有 → Console 有 "⚠️ 预制体未设置" 警告？
           ├─ 有 → 在 VFXManager 中拖拽预制体
           └─ 没有 → 检查 VFXManager 是否存在
```

---

## 🐛 常见问题速查

### 问题 1：Console 显示 "预制体未设置"
**解决**：在 VFXManager 中拖拽粒子预制体

### 问题 2：Console 显示 "播放特效" 但看不到
**解决**：设置 Order in Layer = 999

### 问题 3：什么日志都没有
**解决**：检查 VFXManager 对象是否存在，脚本是否附加

### 问题 4：只在 Scene 视图看到，Game 视图看不到
**解决**：设置 Order in Layer = 999

### 问题 5：粒子太小看不清
**解决**：在粒子预制体中增加 Start Size

---

## ✅ 最终检查清单

完成以下所有项目后，特效应该就能正常显示：

- [ ] 5个粒子预制体的 Order in Layer 都设置为 999
- [ ] VFXManager 中所有预制体引用都不是 None
- [ ] Console 显示 "✅ 播放特效" 日志
- [ ] Hierarchy 的 VFX_Container 下能看到生成的对象
- [ ] Main Camera 的 Culling Mask 包含 Default 层

---

## 💡 测试用的简单命令

在 Console 中应该看到这样的日志：

```
✅ 播放特效：Particle_Stars_00 位置：(2.5, 3.0, 0)
   粒子系统：isPlaying=True, particleCount=15, emission=True, duration=2
```

如果看到：
```
⚠️ Particle_Stars_00 上没有找到 ParticleSystem 组件！
```
说明预制体有问题，需要检查预制体结构。

---

## 🎨 推荐设置（复制这些值）

### 粒子预制体标准设置：

```
Particle System → Renderer:
├─ Render Mode: Billboard
├─ Sorting Layer: Default
└─ Order in Layer: 999 ✨✨✨

Particle System → Main:
├─ Start Size: 0.5
├─ Start Color: Alpha = 255
└─ Simulation Space: World

Particle System → Emission:
└─ Rate over Time: 30
```

---

完成以上步骤后，如果还是看不到，请：
1. 运行游戏
2. 截图 Console 的日志
3. 截图 Hierarchy 窗口（展开 VFX_Container）
4. 截图粒子预制体的 Inspector（Renderer 部分）
5. 告诉我，我会继续帮你排查！

特效一定能显示出来的！加油！🎉✨

