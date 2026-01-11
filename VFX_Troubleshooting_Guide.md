# VFX 特效不显示 - 完整排查与解决方案

## 🔍 问题：生成的 VFX 都看不到

这是 Unity 粒子特效最常见的问题之一！通常是渲染层级或摄像机设置导致的。

---

## ✅ 快速解决方案（按顺序尝试）

### 🎯 方案 1：调整 Sorting Layer（最重要！）

**问题**：粒子特效的渲染层级在背景或游戏对象下面，被遮挡了。

**解决步骤**：

```
1. 选中任意一个粒子预制体（在 Project 窗口中）
   路径：Assets/Layer Lab/.../Prefabs_DemoScene_Particle/
   例如：Particle_Stars_00

2. 在 Inspector 中找到 Particle System 组件

3. 展开 Renderer 部分，设置：
   ┌─────────────────────────────────┐
   │ Renderer                        │
   │ ├─ Render Mode: Billboard       │ ✓
   │ ├─ Sorting Layer: Default       │ ✓
   │ └─ Order in Layer: 999          │ ✓✓✓ 关键！设置为最高
   └─────────────────────────────────┘

4. 重复以上步骤，修改所有使用的粒子预制体：
   ✓ Particle_Stars_00 (吃球特效)
   ✓ Particle_Card_00 (洗牌特效)
   ✓ Particle_Firework_00 (胜利特效)
   ✓ Particle_Smoke_00 (失败特效)
   ✓ Particle_Trail_00 (蛇头拖尾)
```

**为什么要设置 Order in Layer = 999？**

- Unity 按照 Order in Layer 的值从小到大渲染
- 背景可能是 0，游戏对象可能是 1-100
- 999 确保特效在最上层显示

---

### 🎯 方案 2：检查 Z 坐标（2D 游戏必查）

**问题**：粒子特效的 Z 坐标不在摄像机视野内。

**检查步骤**：

```
1. 选中 Main Camera，查看 Position Z
   - 通常是 -10

2. 确保粒子特效的 Z 坐标在 0 到 -5 之间
   - 太远（如 Z = 10）：摄像机看不到
   - 太近（如 Z = -20）：在摄像机后面
```

**修改代码**（在播放特效时强制设置 Z）：

打开 `VFXManager.cs`，找到 `PlayVFX()` 方法，修改：

```csharp
void PlayVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation)
{
    GameObject vfxInstance;

    if (useObjectPooling)
    {
        vfxInstance = GetFromPool(vfxPrefab);
    }
    else
    {
        vfxInstance = Instantiate(vfxPrefab, vfxContainer);
    }

    // ✨ 强制设置 Z 坐标为 0（确保在摄像机视野内）
    position.z = 0f;

    vfxInstance.transform.position = position;
    vfxInstance.transform.rotation = rotation;
    vfxInstance.SetActive(true);

    // 自动销毁或回收
    StartCoroutine(RecycleVFX(vfxInstance, vfxPrefab));
}
```

---

### 🎯 方案 3：检查摄像机 Culling Mask

**问题**：摄像机可能没有渲染粒子所在的层。

**解决步骤**：

```
1. 选中 Main Camera

2. 在 Inspector 中找到 Camera 组件

3. 检查 Culling Mask：
   - 确保勾选了 "Everything" 或 "Default"
   - 如果特效在特殊层，也要勾选对应层
```

---

### 🎯 方案 4：检查粒子系统设置

**问题**：粒子太小、透明度太低、或没有发射。

**检查步骤**：

```
1. 在 Hierarchy 中找到运行时生成的粒子对象
   （运行游戏后，在 VFX_Container 下查找）

2. 选中粒子对象，在 Inspector 中检查 Particle System：

   ┌─────────────────────────────────┐
   │ Particle System                 │
   │ ├─ Duration: 1-5秒              │ ✓
   │ ├─ Start Lifetime: 0.5-2秒      │ ✓
   │ ├─ Start Speed: 1-5             │ ✓
   │ ├─ Start Size: 0.2-1.0          │ ✓ 太小会看不见
   │ ├─ Start Color: 不透明          │ ✓ Alpha = 255
   │ ├─ Simulation Space: World      │ ✓
   │ └─ Play On Awake: ✓             │ ✓
   └─────────────────────────────────┘

   ┌─────────────────────────────────┐
   │ Emission                        │
   │ └─ Rate over Time: >10          │ ✓ 太少会看不见
   └─────────────────────────────────┘
```

**如果粒子太小**：

- 在预制体中增加 Start Size
- 或在 VFXManager 中设置全局缩放

---

### 🎯 方案 5：使用 Scene 视图查找特效

**技巧**：特效可能生成了，但位置不对。

**查找步骤**：

```
1. 运行游戏

2. 切换到 Scene 视图（不是 Game 视图）

3. 在 Hierarchy 中展开 VFX_Container

4. 选中生成的粒子对象

5. 在 Scene 视图中查看：
   - 如果能看到：说明是渲染层级问题
   - 如果看不到：说明是粒子系统设置问题
   - 如果位置很远：说明是坐标计算问题
```

---

## 🛠️ 代码修复方案

### 修复 1：强制设置 Z 坐标

在 `VFXManager.cs` 的所有播放特效的方法中添加：

```csharp
/// <summary>
/// 播放吃球特效
/// </summary>
public void PlayCollectBallVFX(Vector3 position)
{
    if (collectBallVFX != null)
    {
        position.z = 0f; // ✨ 强制设置 Z 坐标
        PlayVFX(collectBallVFX, position, Quaternion.identity);
    }
}

/// <summary>
/// 播放洗牌特效
/// </summary>
public void PlayShuffleCardsVFX(Vector3 position)
{
    if (shuffleCardsVFX != null)
    {
        position.z = 0f; // ✨ 强制设置 Z 坐标
        PlayVFX(shuffleCardsVFX, position, Quaternion.identity);
    }
}

// ... 其他方法类似
```

### 修复 2：添加调试日志

在 `VFXManager.cs` 的 `PlayVFX()` 方法中添加：

```csharp
void PlayVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation)
{
    if (vfxPrefab == null)
    {
        Debug.LogError("❌ VFX 预制体为空！请检查 VFXManager 的引用设置。");
        return;
    }

    GameObject vfxInstance;

    if (useObjectPooling)
    {
        vfxInstance = GetFromPool(vfxPrefab);
    }
    else
    {
        vfxInstance = Instantiate(vfxPrefab, vfxContainer);
    }

    position.z = 0f; // 强制设置 Z 坐标

    vfxInstance.transform.position = position;
    vfxInstance.transform.rotation = rotation;
    vfxInstance.SetActive(true);

    // 调试日志
    Debug.Log($"✅ 播放特效：{vfxPrefab.name} 位置：{position}");

    // 检查粒子系统
    ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
    if (ps != null)
    {
        Debug.Log($"   粒子系统状态：isPlaying={ps.isPlaying}, particleCount={ps.particleCount}");
    }
    else
    {
        Debug.LogWarning($"⚠️ {vfxPrefab.name} 上没有找到 ParticleSystem 组件！");
    }

    // 自动销毁或回收
    StartCoroutine(RecycleVFX(vfxInstance, vfxPrefab));
}
```

---

## 📋 完整排查清单

按顺序检查以下项目：

### 1. VFXManager 设置

- [ ] VFXManager 对象存在于场景中
- [ ] 5 个粒子预制体都已拖拽到 VFXManager
- [ ] 预制体引用不为空（不是 "None (GameObject)"）

### 2. 粒子预制体设置

- [ ] Particle System → Renderer → Sorting Layer = Default
- [ ] Particle System → Renderer → Order in Layer = 999
- [ ] Particle System → Start Size > 0.2
- [ ] Particle System → Start Color → Alpha = 255
- [ ] Particle System → Emission → Rate over Time > 10

### 3. 摄像机设置

- [ ] Main Camera → Position Z = -10（或其他负值）
- [ ] Main Camera → Culling Mask 包含 Default 层
- [ ] Main Camera → Projection = Orthographic（2D 游戏）

### 4. 特效生成检查

- [ ] Console 中有 "✅ 播放特效" 日志
- [ ] Hierarchy 中的 VFX_Container 下有生成的对象
- [ ] 在 Scene 视图中能看到特效对象

### 5. 代码调用检查

- [ ] SnakeController 中调用了 `PlayCollectBallVFX()`
- [ ] PokerManager 中调用了 `PlayShuffleCardsVFX()`
- [ ] GameResultPopup 中调用了 `PlayVictoryVFX()`

---

## 🎨 最简单的测试方法

### 测试步骤：

```
1. 打开 Unity Editor

2. 在 Project 窗口中找到任意粒子预制体
   例如：Particle_Stars_00

3. 直接拖拽到 Hierarchy 窗口（场景中）

4. 设置 Position = (0, 0, 0)

5. 运行游戏

6. 如果能看到：
   → 说明粒子本身没问题，是代码调用或设置的问题

7. 如果看不到：
   → 说明是粒子预制体的设置问题
   → 检查 Sorting Layer 和 Order in Layer
```

---

## 💡 常见错误和解决方法

### 错误 1：Console 中没有任何日志

**原因**：VFXManager 可能没有正确初始化

**解决**：

```
1. 检查 VFXManager 对象是否在场景中
2. 检查 VFXManager.cs 脚本是否附加到对象上
3. 运行游戏，检查 Instance 是否为 null
```

### 错误 2：Console 显示 "VFX 预制体为空"

**原因**：没有在 Inspector 中拖拽预制体

**解决**：

```
1. 选中 VFXManager 对象
2. 在 Inspector 中找到空的引用
3. 从 Project 窗口拖拽对应的粒子预制体
```

### 错误 3：粒子生成了但很快消失

**原因**：粒子生命周期太短

**解决**：

```
1. 选中粒子预制体
2. Particle System → Start Lifetime: 改为 2.0
3. Particle System → Duration: 改为 5.0
```

### 错误 4：只能在 Scene 视图看到，Game 视图看不到

**原因**：Sorting Layer 问题

**解决**：

```
1. 设置 Order in Layer = 999
2. 确保 Simulation Space = World
3. 检查 Camera 的 Culling Mask
```

---

## 🔧 推荐的粒子预制体设置

### 标准设置模板：

```
Particle System:
├─ Duration: 2.0
├─ Looping: ✓ (持续特效) 或 ❌ (一次性特效)
├─ Start Delay: 0
├─ Start Lifetime: 1.0
├─ Start Speed: 3.0
├─ Start Size: 0.5
├─ Start Color: White (255, 255, 255, 255)
├─ Gravity Modifier: 0
├─ Simulation Space: World
├─ Play On Awake: ✓
└─ Max Particles: 100

Emission:
└─ Rate over Time: 30

Renderer:
├─ Render Mode: Billboard
├─ Sorting Layer: Default
└─ Order in Layer: 999 ✨✨✨
```

---

## ✅ 最终检查

如果以上方法都不行，请按以下步骤完整检查：

```
1. 打开 Unity Console
2. 运行游戏
3. 触发特效（吃球/洗牌等）
4. 检查 Console 是否有日志
5. 检查 Hierarchy → VFX_Container 下是否有对象
6. 选中生成的对象，查看 Inspector
7. 截图发送给我，包括：
   - Inspector 中的 Particle System 设置
   - Hierarchy 截图
   - Console 日志
```

---

完成以上检查后，特效应该就能正常显示了！🎉

如果还有问题，请告诉我具体看到了什么（或没看到什么），我会继续帮你排查！✨
