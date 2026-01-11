# Phase 1 完成检查清单

## ✅ 已完成

1. **创建 SnakeGridManager.cs**
   - 50×100 网格系统
   - HashSet 优化的占用检测
   - 支持左右边界穿越
   - 坐标转换函数
   - 障碍物和得分球生成接口

2. **更新 CellController.cs**
   - 简化为适合贪吃蛇游戏
   - 移除旧的射击游戏逻辑
   - 添加 CellType 和 gridPosition

3. **删除旧代码**
   - ✓ BeltPathHolder.cs
   - ✓ BeltWalker.cs
   - ✓ PigController.cs
   - ✓ ShooterTableManager.cs
   - ✓ ReadyQueueManager.cs
   - ✓ BulletController.cs

---

## 🎯 下一步：在 Unity 中设置

### 1. 创建预制体（Prefabs）

请在 Unity 中执行以下操作：

#### a) 创建蛇头预制体
```
1. Hierarchy → 右键 → 3D Object → Sphere
2. 重命名为 "SnakeHead"
3. Transform:
   - Position: (0, 0, 0)
   - Scale: (0.8, 0.8, 0.8)
4. 拖拽到 Assets/Prefabs/ 文件夹
5. 删除场景中的实例
```

#### b) 创建蛇身预制体
```
1. Hierarchy → 右键 → 3D Object → Cube
2. 重命名为 "SnakeBody"
3. Transform:
   - Position: (0, 0, 0)
   - Scale: (0.9, 0.9, 0.9)
4. 拖拽到 Assets/Prefabs/ 文件夹
5. 删除场景中的实例
```

#### c) 创建障碍物预制体
```
1. Hierarchy → 右键 → 3D Object → Cube
2. 重命名为 "Obstacle"
3. Transform:
   - Position: (0, 0, 0)
   - Scale: (0.9, 0.9, 0.9)
4. 材质设置：使用 Mat_Grey 或创建新材质
5. 拖拽到 Assets/Prefabs/ 文件夹
6. 删除场景中的实例
```

#### d) 创建得分球预制体
```
1. Hierarchy → 右键 → 3D Object → Sphere
2. 重命名为 "ScoreBall"
3. Transform:
   - Position: (0, 0, 0)
   - Scale: (0.8, 0.8, 0.8)
4. 材质设置：使用 Mat_Pink 或创建新材质
5. 拖拽到 Assets/Prefabs/ 文件夹
6. 删除场景中的实例
```

### 2. 设置 GameScene

#### a) 修改 Gameboard GameObject
```
1. 在 Hierarchy 中选中 Gameboard
2. 移除旧的 GridManager 组件（如果有）
3. Add Component → SnakeGridManager
4. 设置参数：
   - Grid Width: 50
   - Grid Height: 100
   - Cell Size: 1.0
5. 拖拽预制体到对应字段：
   - Snake Head Prefab: SnakeHead
   - Snake Body Prefab: SnakeBody
   - Obstacle Prefab: Obstacle
   - Score Ball Prefab: ScoreBall
6. 拖拽材质：
   - Snake Material: (可选)
   - Obstacle Material: Mat_Grey
   - Score Ball Material: Mat_Pink
```

### 3. 测试网格系统

#### 测试代码（临时）
在 `SnakeGridManager.cs` 的 `Start()` 方法后添加：

```csharp
void Start() 
{ 
    InitializeGrid();
    
    // 测试：生成一些障碍物和得分球
    StartCoroutine(TestGeneration());
}

IEnumerator TestGeneration()
{
    yield return new WaitForSeconds(1f);
    
    // 生成3个障碍物
    GenerateRectangleObstacle(5, 5);
    GenerateRectangleObstacle(8, 8);
    GenerateRectangleObstacle(10, 10);
    
    // 生成20个得分球
    GenerateScoreBalls(20);
    
    Debug.Log("测试生成完成！");
}
```

---

## ⚠️ 可能的错误

如果 Console 出现错误：
1. **缺少预制体引用** → 确保在 Inspector 中拖拽了所有预制体
2. **材质未找到** → 检查 Materials 文件夹是否有对应材质
3. **CellType 未定义** → 确保两个脚本都已保存并编译成功

---

## 📝 完成后告诉我

完成上述设置后，运行游戏，应该能看到：
- ✅ 网格边界（青色线框，只在 Scene 视图可见）
- ✅ 3个随机位置的矩形障碍物（灰色Cube）
- ✅ 20个随机位置的得分球（粉色Sphere）

截图或告诉我结果，然后我们进入 **Phase 2：创建贪吃蛇控制器** 🐍

