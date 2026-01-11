<div align="center">

# Poker Card Snake Game

**扑克牌贪吃蛇 - 策略与反应的完美结合**

A Poker-Card Driven Snake Game | ポーカーカードスネークゲーム

---

[![Unity](https://img.shields.io/badge/Unity-2021.3+-000?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-blue?style=for-the-badge)](LICENSE)

### [技术文档 Technical Documentation](https://newbieaudiokid.github.io/U3D_Shuffle_Snake/)

[中文](#中文) | [English](#english) | [日本語](#日本語)

</div>

---

<a name="中文"></a>

## 游戏概述

Poker Card Snake Game 是一款创新的贪吃蛇变体游戏，融合了扑克牌抽取机制和经典贪吃蛇玩法。玩家通过洗牌抽取扑克牌组合来生成地图元素，同时控制贪吃蛇吃球得分，在20秒内挑战高分！

### 核心玩法

#### 扑克牌系统
- **洗牌机制** - 点击扑克区域随机抽取6张牌（0.1秒冷却）
- **组合识别** - 自动识别斗地主牌型（对子、三张、顺子、飞机等）
- **动态地图** - 不同牌型生成不同的地图元素：
  - 对子/三张 → 20%障碍物 或 80%得分球
  - 三带一/三带二 → 10个得分球
  - 顺子/连对 → 20个得分球
  - 飞机/四带二 → 40个得分球

#### 贪吃蛇系统
- **网格系统** - 20×35的大型游戏区域
- **8方向移动** - 支持上下左右和4个斜角方向
- **拖拽控制** - 拖拽距离控制速度，越长越快（0.5x - 2.0x）
- **屏幕穿越** - 支持四个方向的边界穿越
- **2x2得分球** - 更大的目标，更容易吃到！

#### 游戏限制
- **20秒倒计时** - 时间到游戏胜利，显示最终分数
- **即时失败** - 撞到障碍物或自己即刻结束游戏

### 特效系统

#### 粒子特效（使用 Layer Lab 素材）
- **吃球特效** - 星星爆炸效果
- **洗牌特效** - 卡牌飞舞效果
- **胜利特效** - 烟花庆祝效果
- **失败特效** - 烟雾消散效果
- **蛇头拖尾** - 速度越快粒子越多

#### UI动画
- **弹性动画** - 组合名称弹出效果（0→120%→100%）
- **淡出效果** - 文字缩小+透明度渐变

---

## 项目结构

```
U3D_Shuffle_Snake/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs                 # 游戏状态、计时器、分数管理
│   │   ├── GameScene/
│   │   │   ├── SnakeGridManager.cs       # 20×35网格、碰撞检测、地图生成
│   │   │   ├── SnakeController.cs        # 蛇移动逻辑、增长、碰撞处理
│   │   │   ├── TouchInputManager.cs      # 拖拽输入、动态速度控制
│   │   │   ├── PokerManager.cs           # 扑克牌管理、洗牌、显示
│   │   │   ├── PokerComboDetector.cs     # 斗地主牌型识别
│   │   │   ├── PokerCardData.cs          # 扑克牌数据结构
│   │   │   ├── VFXManager.cs             # 特效管理、对象池
│   │   │   └── CellController.cs         # 格子控制器
│   │   └── UIScripts/
│   │       ├── GameResultPopup.cs        # 胜利/失败弹窗
│   │       ├── TimerDisplay.cs           # 倒计时显示
│   │       ├── ScoreDisplay.cs           # 分数显示
│   │       └── Animation/
│   │           └── SceneFader.cs         # 场景淡入淡出
│   ├── Resources/
│   │   └── Poke/
│   │       ├── cards.csv                 # 扑克牌数据
│   │       └── png/                      # 扑克牌图片资源
│   ├── Layer Lab/                        # 粒子特效素材库
│   └── Scenes/
│       ├── SplashScene.unity             # 启动画面
│       ├── MenuScene.unity               # 主菜单
│       └── GameScene.unity               # 游戏场景
├── docs/
│   └── index.html                        # GitHub Pages 技术文档
└── Phase*_Setup_Guide.md                 # 各阶段设置指南
```

---

## 技术亮点

### 核心算法

#### 1. 动态速度系统
```csharp
// 拖拽距离 → 速度倍数（线性插值）
float speedMultiplier = Mathf.Lerp(
    minSpeedMultiplier,  // 0.5x (短距离)
    maxSpeedMultiplier,  // 2.0x (长距离)
    dragDistance / maxDragDistance
);
```

#### 2. 2x2得分球碰撞优化
```csharp
// 占据4个格子，蛇头碰到任意一格都算吃到
// 碰撞面积增加4倍，大幅提升游戏体验
for (int x = 0; x < 2; x++)
    for (int y = 0; y < 2; y++)
        RegisterCell(bottomLeft + new Vector2Int(x, y), CellType.ScoreBall, ball);
```

#### 3. 扑克牌组合识别（斗地主规则）
```csharp
// 支持识别：单张、对子、三张、三带一、三带二
// 顺子、连对、飞机、四带二 等复杂牌型
public PokerComboResult DetectCombo(List<PokerCard> cards)
{
    // 从复杂到简单依次检测
    if (TryDetectFourWithTwo(...)) return result;
    if (TryDetectPlane(...)) return result;
    // ...
}
```

#### 4. 屏幕穿越算法
```csharp
// 支持四方向边界穿越（左右、上下互通）
public Vector2Int WrapGridPosition(Vector2Int gridPos)
{
    int newX = (gridPos.x + gridWidth) % gridWidth;
    int newY = (gridPos.y + gridHeight) % gridHeight;
    return new Vector2Int(newX, newY);
}
```

### 性能优化

- **HashSet 碰撞检测** - O(1)复杂度的占用格子查询
- **对象池系统** - VFX特效复用，减少GC压力
- **预制体缓存** - 避免频繁的资源加载
- **平滑移动插值** - 使用AnimationCurve实现丝滑动画

### 设计模式

- **单例模式** - 全局管理器（GameManager, VFXManager等）
- **组件化设计** - 高内聚低耦合的脚本架构
- **事件驱动** - 协程驱动的游戏流程
- **状态机** - 清晰的游戏状态管理

---

## 系统要求

- **Unity** 2021.3 LTS 或更高版本
- **TextMeshPro** 包（已内置）
- **Layer Lab GUI Pro** 粒子特效素材包

---

## 快速开始

### 1. 克隆仓库
```bash
git clone https://github.com/NewbieAudioKid/U3D_Shuffle_Snake.git
cd U3D_Shuffle_Snake
```

### 2. 在 Unity 中打开
1. 打开 Unity Hub
2. 选择 "Add" → 选择项目文件夹
3. 使用 Unity 2021.3+ 打开

### 3. 运行游戏
1. 打开 `Assets/Scenes/SplashScene.unity`
2. 点击 Play 按钮
3. 开始游戏！

---

## 开发指南

### Phase 1-7 完整开发流程

项目采用 **Bottom-Up** 开发模式，分7个阶段完成：

1. **Phase 1**: 网格系统和基础框架
2. **Phase 2**: 贪吃蛇移动逻辑
3. **Phase 3**: 触摸输入系统
4. **Phase 4**: 测试场景和碰撞检测
5. **Phase 5**: 扑克牌管理系统
6. **Phase 6**: 扑克牌→地图联动
7. **Phase 7**: 计时器和游戏结束

详见各阶段的 `Phase*_Setup_Guide.md` 文档。

### 新功能文档

- `NewFeatures_Setup_Guide.md` - 动态速度、弹性动画、特效系统
- `SnakeHeadVFX_Setup_Guide.md` - 蛇头拖尾特效
- `ScoreBall_2x2_Implementation.md` - 2x2得分球实现
- `VFX_Troubleshooting_Guide.md` - 特效问题排查

---

## 美术资源

### 扑克牌
- 来源：`Assets/Resources/Poke/`
- 格式：PNG透明背景
- 规格：54张标准扑克牌

### 粒子特效
- 来源：Layer Lab GUI Pro - CasualGame
- 包含：30+ 种粒子效果预制体
- 风格：2D卡通风格

---

## 配置说明

### Unity Inspector 关键设置

#### GameManager
- Game Time: 20秒
- 自动场景切换支持

#### SnakeGridManager
- Grid Width: 20
- Grid Height: 35  
- Cell Size: 0.4f
- Grid Offset: (-4, -7) - 对齐红色区域

#### TouchInputManager
- Min Speed Multiplier: 0.5x
- Max Speed Multiplier: 2.0x
- Poker Zone Height Ratio: 0.2 (屏幕下方20%)

#### VFXManager
- Enable Snake Head VFX: 是
- Min/Max Trail Emission: 10/50
- Object Pooling: 是

---

## 已知问题

### 特效不显示
**解决方案**：设置粒子预制体的 `Order in Layer = 999`  
详见：`VFX_QuickFix.md`

### 扑克牌显示白牌
**解决方案**：确保PNG导入设置为 Sprite (2D and UI)  
详见：`Phase5_PokerCard_Visual_Fix.md`

---

## 更新日志

### v1.0.0 (2026-01-12)
- 完整的贪吃蛇游戏逻辑
- 扑克牌洗牌和组合识别系统
- 动态速度控制（拖拽距离）
- 完整的粒子特效系统
- 2x2大型得分球
- 20秒倒计时
- 完整的UI和动画系统

---

## 贡献

欢迎提交 Issue 和 Pull Request！

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

---

## 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

---

## 作者

**NewbieAudioKid**  
GitHub: [@NewbieAudioKid](https://github.com/NewbieAudioKid)

---

<a name="english"></a>

## Game Overview

Poker Card Snake Game is an innovative snake game variant that combines poker card mechanics with classic snake gameplay. Players draw poker card combinations to generate map elements while controlling a snake to eat score balls and achieve high scores within 20 seconds!

### Core Gameplay

#### Poker Card System
- **Shuffle Mechanic** - Click poker area to randomly draw 6 cards (0.1s cooldown)
- **Combo Detection** - Auto-detect Dou Dizhu poker patterns (Pair, Three, Straight, etc.)
- **Dynamic Map** - Different combos generate different map elements

#### Snake System
- **Grid System** - 20×35 large game area
- **8-Direction Movement** - Cardinal + diagonal directions
- **Drag Control** - Drag distance controls speed (0.5x - 2.0x)
- **Screen Wrapping** - Four-way boundary wrapping
- **2x2 Score Balls** - Larger targets, easier to catch!

#### Game Limits
- **20-Second Countdown** - Victory on time-up, displays final score
- **Instant Failure** - Hit obstacles or self to end game

### VFX System

#### Particle Effects (Layer Lab Assets)
- **Collect Ball VFX** - Star explosion effect
- **Shuffle Cards VFX** - Card flying effect
- **Victory VFX** - Firework celebration effect
- **Game Over VFX** - Smoke dissipation effect
- **Snake Head Trail** - More particles at higher speeds

---

## Requirements

- Unity 2021.3 LTS or higher
- TextMeshPro package
- Layer Lab GUI Pro particle effects

---

## Quick Start

```bash
git clone https://github.com/NewbieAudioKid/U3D_Shuffle_Snake.git
```

1. Open in Unity 2021.3+
2. Open `Assets/Scenes/SplashScene.unity`
3. Press Play

---

<a name="日本語"></a>

## ゲーム概要

Poker Card Snake Game は、ポーカーカードメカニクスとクラシックなスネークゲームプレイを組み合わせた革新的なスネークゲームです。

### コアゲームプレイ

- **シャッフルメカニズム** - ポーカーエリアをクリックして6枚のカードをランダムに引く
- **コンボ検出** - 闘地主のポーカーパターンを自動検出
- **8方向移動** - 上下左右+4つの斜め方向をサポート
- **ドラッグコントロール** - ドラッグ距離で速度を制御
- **20秒カウントダウン** - タイムアップで勝利

---

<div align="center">

**Built with Unity**

Made by NewbieAudioKid

</div>
