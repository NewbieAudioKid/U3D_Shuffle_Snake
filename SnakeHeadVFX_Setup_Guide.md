# 蛇头拖尾特效设置指南

## ✨ 新功能：蛇头拖尾特效

### 效果说明

- 蛇头会持续发射粒子特效（拖尾效果）
- 粒子发射速率随蛇的移动速度动态变化
- 慢速：少量粒子 💫
- 快速：大量粒子 ✨✨✨
- 游戏结束时特效自动移除

---

## 🔧 Unity 设置步骤

### 第一步：选择蛇头拖尾特效

在 `VFXManager` 组件中添加新的粒子特效：

```
路径：Assets/Layer Lab/GUI Pro-CasualGame/Prefabs/Prefabs_DemoScene_Particle/

推荐选择（拖尾特效）：
- Particle_Trail_00     ✨ (拖尾轨迹)
- Particle_Sparkle_00   💫 (闪烁轨迹)
- Particle_Smoke_00     💨 (烟雾轨迹)
- Particle_Fire_00      🔥 (火焰轨迹)
- Particle_Electric_00  ⚡ (电光轨迹)
- Particle_Star_Trail_00 ⭐ (星星轨迹)
```

### 第二步：配置 VFXManager

```
选中 VFXManager → Inspector:

粒子特效预制体:
- Snake Head Trail VFX: 拖拽 Particle_Trail_00 (或你选择的特效)

蛇头特效设置:
- Enable Snake Head VFX: ✓ (勾选以启用)
- Min Trail Emission: 10  (慢速时的粒子发射速率)
- Max Trail Emission: 50  (快速时的粒子发射速率)
```

### 第三步：测试特效

```
1. 运行游戏
2. 观察蛇头是否有拖尾特效
3. 尝试拖拽屏幕：
   - 短距离拖拽（慢速）→ 粒子少
   - 长距离拖拽（快速）→ 粒子多
4. 游戏结束时特效消失
```

---

## 🎨 推荐的拖尾特效

### 风格 1：梦幻星光 ⭐

```
特效: Particle_Star_Trail_00
效果: 星星轨迹，适合可爱风格
颜色: 金色/彩色
```

### 风格 2：电光火石 ⚡

```
特效: Particle_Electric_00
效果: 电光轨迹，适合科技风格
颜色: 蓝色/紫色
```

### 风格 3：烈焰蛟龙 🔥

```
特效: Particle_Fire_00
效果: 火焰轨迹，适合热血风格
颜色: 红色/橙色
```

### 风格 4：幽灵之尾 💨

```
特效: Particle_Smoke_00
效果: 烟雾轨迹，适合神秘风格
颜色: 白色/灰色
```

---

## ⚙️ 参数调整

### 调整粒子发射速率

在 `VFXManager` 组件中：

```
想要更多粒子（更华丽）:
- Min Trail Emission: 10 → 20
- Max Trail Emission: 50 → 100

想要更少粒子（更清爽）:
- Min Trail Emission: 10 → 5
- Max Trail Emission: 50 → 30

想要恒定粒子（不随速度变化）:
- Min Trail Emission: 30
- Max Trail Emission: 30
```

### 调整粒子大小

粒子大小会根据速度自动调整：

- 慢速：0.8x 大小
- 快速：1.5x 大小

如果想要固定大小，可以修改代码或在粒子系统预制体中调整。

---

## 🎮 动态效果演示

```
场景：玩家控制蛇移动

状态 1：正常移动（速度 1.0x）
→ 粒子发射速率: 30/秒
→ 粒子大小: 1.0x

状态 2：短距离拖拽（速度 0.5x）
→ 粒子发射速率: 10/秒 (最少)
→ 粒子大小: 0.8x (最小)

状态 3：长距离拖拽（速度 2.0x）
→ 粒子发射速率: 50/秒 (最多)
→ 粒子大小: 1.5x (最大)

状态 4：游戏结束
→ 特效立即移除
```

---

## 🔍 技术细节

### 实现方式

1. **特效附加**：

   - 蛇初始化时，特效作为子对象附加到蛇头
   - 特效跟随蛇头移动，无需手动更新位置

2. **动态调整**：

   - 速度变化时，通过修改粒子系统的 `emission.rateOverTime` 调整发射速率
   - 通过修改 `main.startSizeMultiplier` 调整粒子大小

3. **生命周期管理**：
   - 初始化时创建并附加特效
   - 蛇身增长时重新附加（因为蛇头对象可能重新创建）
   - 游戏结束时移除特效

### 代码位置

- `VFXManager.cs`:

  - `AttachSnakeHeadVFX()` - 附加特效到蛇头
  - `UpdateSnakeHeadVFXIntensity()` - 更新特效强度
  - `RemoveSnakeHeadVFX()` - 移除特效

- `SnakeController.cs`:
  - `InitializeSnake()` - 初始化时附加特效
  - `SetSpeedMultiplier()` - 速度变化时更新特效
  - `UpdateVisuals()` - 蛇身增长时重新附加特效
  - `Die()` - 游戏结束时移除特效

---

## 🐛 常见问题

### 问题 1：特效不显示

**检查清单**：

- [ ] VFXManager 中是否勾选了 `Enable Snake Head VFX`？
- [ ] `Snake Head Trail VFX` 是否拖拽了粒子预制体？
- [ ] 粒子系统的 `Sorting Layer` 设置是否正确？
- [ ] Camera 是否能看到蛇头位置？

### 问题 2：特效位置不对

**原因**：粒子系统的 Local Position 不为零

**解决**：

1. 选中粒子预制体
2. 检查 Transform → Position 是否为 (0, 0, 0)
3. 如果不是，手动调整为 (0, 0, 0)

### 问题 3：特效随速度变化不明显

**原因**：发射速率范围太小

**解决**：

```
VFXManager:
- Min Trail Emission: 10 → 5   (降低最小值)
- Max Trail Emission: 50 → 100 (提高最大值)
```

### 问题 4：特效太多导致卡顿

**原因**：粒子数量过多

**解决**：

```
1. 降低发射速率:
   - Max Trail Emission: 50 → 30

2. 减少粒子生命周期:
   - 在粒子预制体中调整 Start Lifetime

3. 减少最大粒子数:
   - 在粒子预制体中调整 Max Particles
```

---

## 🎨 自定义粒子特效

如果 Layer Lab 的特效不满足需求，可以自定义：

### 方法 1：修改现有特效

```
1. 复制 Layer Lab 的粒子预制体
2. 修改粒子系统参数：
   - Start Color: 改变颜色
   - Start Size: 改变大小
   - Start Lifetime: 改变持续时间
   - Emission: 改变发射速率
   - Shape: 改变发射形状
```

### 方法 2：创建新特效

```
1. GameObject → Effects → Particle System
2. 调整参数：
   - Renderer → Render Mode: Billboard
   - Renderer → Sorting Layer: 设置正确的层级
   - Emission → Rate over Time: 30
   - Start Lifetime: 0.5-1.0
   - Start Speed: 1-3
   - Start Size: 0.2-0.5
3. 保存为预制体
4. 拖拽到 VFXManager
```

---

## ✅ 完整检查清单

- [ ] VFXManager 已添加 `Snake Head Trail VFX`
- [ ] `Enable Snake Head VFX` 已勾选
- [ ] 运行游戏，蛇头有拖尾特效
- [ ] 拖拽屏幕，特效强度随速度变化
- [ ] 游戏结束，特效正确移除
- [ ] 性能良好，无卡顿

---

## 🎉 效果对比

### 之前

```
蛇头：普通方块 □
移动：静态，无视觉反馈
```

### 之后

```
蛇头：方块 + 拖尾特效 □✨
移动：动态，粒子随速度变化 □💫✨⭐
快速：粒子爆发 □✨💫⭐💥✨
```

---

完成设置后，蛇头会有超炫的拖尾特效！🐍✨

拖拽越长，特效越炫酷！🎮
