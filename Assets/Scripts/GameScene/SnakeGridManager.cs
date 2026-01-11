// ================================================================================
// TL;DR:
// 贪吃蛇游戏的网格系统管理器，负责 50x100 网格的管理和碰撞检测。
// 使用 HashSet 优化占用格子的查询性能，支持快速碰撞检测和随机空位生成。
//
// 目标：
// - 管理 50(宽) x 100(高) 的游戏网格
// - 使用 HashSet 存储已占用格子（蛇身、障碍物、得分球）
// - 提供 O(1) 的碰撞检测性能
// - 支持随机生成空位（用于生成障碍物和得分球）
// - 支持屏幕边界穿越（左右边界互通）
//
// 非目标：
// - 不处理贪吃蛇移动逻辑（由 SnakeController 负责）
// - 不处理扑克牌系统（由 PokerManager 负责）
// - 不处理输入控制（由 InputManager 负责）
// ================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 格子类型枚举
/// </summary>
public enum CellType
{
    Empty,          // 空格
    SnakeHead,      // 蛇头
    SnakeBody,      // 蛇身
    Obstacle,       // 障碍物
    ScoreBall       // 得分球
}

/// <summary>
/// 贪吃蛇网格管理器
/// </summary>
public class SnakeGridManager : MonoBehaviour
{
    // ==================== 单例模式 ====================
    public static SnakeGridManager Instance;

    // ==================== 网格配置 ====================
    [Header("网格设置")]
    public int gridWidth = 20;              // 网格宽度
    public int gridHeight = 35;             // 网格高度
    public float cellSize = 0.4f;           // 单格世界坐标尺寸（调整以适应背景板）
    
    [Header("网格位置偏移")]
    [Tooltip("网格左下角相对于Gameboard的偏移")]
    public Vector2 gridOffset = new Vector2(-4f, -7f); // 手动调整以对齐红色区域
    
    private Vector2 gridOrigin;             // 网格左下角 (0,0) 的世界坐标

    // ==================== 资源引用 ====================
    [Header("预制体资源")]
    public GameObject snakeHeadPrefab;      // 蛇头预制体（Sphere）
    public GameObject snakeBodyPrefab;      // 蛇身预制体（Cube）
    public GameObject obstaclePrefab;       // 障碍物预制体（Cube）
    public GameObject scoreBallPrefab;      // 得分球预制体（Sphere）

    [Header("材质资源")]
    public Material snakeMaterial;          // 蛇的材质
    public Material obstacleMaterial;       // 障碍物材质
    public Material scoreBallMaterial;      // 得分球材质

    // ==================== 核心数据结构 ====================
    // 使用 HashSet 存储已占用的格子坐标，O(1) 查询性能
    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
    
    // 使用 Dictionary 存储格子上的 GameObject 引用（用于销毁）
    private Dictionary<Vector2Int, GameObject> cellObjects = new Dictionary<Vector2Int, GameObject>();
    
    // 使用 Dictionary 存储格子类型（用于碰撞判断）
    private Dictionary<Vector2Int, CellType> cellTypes = new Dictionary<Vector2Int, CellType>();

    // ==================== 生命周期 ====================
    void Awake() 
    { 
        Instance = this; 
    }

void Start() 
{ 
    InitializeGrid();
    
    // 测试：生成一些障碍物和得分球
    // StartCoroutine(TestGeneration());
}

IEnumerator TestGeneration()
{
    yield return new WaitForSeconds(1f);
    
    // 生成2个小障碍物（适应新的网格大小）
    GenerateRectangleObstacle(3, 3);
    GenerateRectangleObstacle(4, 4);
    
    // 生成15个得分球
    GenerateScoreBalls(15);
    
    Debug.Log("✅ 测试生成完成！");
}

    // ==================== 网格初始化 ====================
    /// <summary>
    /// 初始化网格系统，计算网格原点
    /// </summary>
    void InitializeGrid()
    {
        // 使用偏移量设置网格原点（不再居中）
        gridOrigin = new Vector2(
            transform.position.x + gridOffset.x,
            transform.position.y + gridOffset.y
        );

        Debug.Log($"SnakeGridManager 初始化完成：{gridWidth}x{gridHeight}");
        Debug.Log($"网格尺寸：{gridWidth * cellSize} x {gridHeight * cellSize} 单位");
        Debug.Log($"网格原点（左下角）：{gridOrigin}");
    }

    // ==================== 坐标转换 ====================
    /// <summary>
    /// 网格坐标转世界坐标
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float worldX = gridOrigin.x + (gridPos.x + 0.5f) * cellSize;
        float worldY = gridOrigin.y + (gridPos.y + 0.5f) * cellSize;
        return new Vector3(worldX, worldY, 0);
    }

    /// <summary>
    /// 世界坐标转网格坐标
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - gridOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    // ==================== 边界处理 ====================
    /// <summary>
    /// 规范化网格坐标（支持上下左右边界穿越）
    /// </summary>
    public Vector2Int NormalizeGridPosition(Vector2Int gridPos)
    {
        int x = gridPos.x;
        int y = gridPos.y;

        // X轴循环（左右穿越）
        if (x < 0) x = gridWidth + x;
        if (x >= gridWidth) x = x - gridWidth;

        // Y轴循环（上下穿越）
        if (y < 0) y = gridHeight + y;
        if (y >= gridHeight) y = y - gridHeight;

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 检查坐标是否在有效范围内
    /// </summary>
    public bool IsValidGridPosition(Vector2Int gridPos)
    {
        return gridPos.y >= 0 && gridPos.y < gridHeight;
        // X轴不检查，因为支持穿越
    }

    // ==================== 占用检测 ====================
    /// <summary>
    /// 检查格子是否被占用
    /// </summary>
    public bool IsCellOccupied(Vector2Int gridPos)
    {
        Vector2Int normalized = NormalizeGridPosition(gridPos);
        return occupiedCells.Contains(normalized);
    }

    /// <summary>
    /// 获取格子类型
    /// </summary>
    public CellType GetCellType(Vector2Int gridPos)
    {
        Vector2Int normalized = NormalizeGridPosition(gridPos);
        if (cellTypes.ContainsKey(normalized))
            return cellTypes[normalized];
        return CellType.Empty;
    }

    // ==================== 格子注册/注销 ====================
    /// <summary>
    /// 注册占用格子
    /// </summary>
    public void RegisterCell(Vector2Int gridPos, CellType type, GameObject obj = null)
    {
        Vector2Int normalized = NormalizeGridPosition(gridPos);
        occupiedCells.Add(normalized);
        cellTypes[normalized] = type;
        
        if (obj != null)
            cellObjects[normalized] = obj;
    }

    /// <summary>
    /// 注销占用格子
    /// </summary>
    public void UnregisterCell(Vector2Int gridPos)
    {
        Vector2Int normalized = NormalizeGridPosition(gridPos);
        occupiedCells.Remove(normalized);
        cellTypes.Remove(normalized);
        cellObjects.Remove(normalized);
    }

    /// <summary>
    /// 清除格子（销毁GameObject并注销）
    /// </summary>
    public void ClearCell(Vector2Int gridPos)
    {
        Vector2Int normalized = NormalizeGridPosition(gridPos);
        
        if (cellObjects.ContainsKey(normalized))
        {
            GameObject obj = cellObjects[normalized];
            if (obj != null)
                Destroy(obj);
        }
        
        UnregisterCell(normalized);
    }

    // ==================== 随机生成 ====================
    /// <summary>
    /// 获取随机空位
    /// </summary>
    public Vector2Int GetRandomEmptyCell()
    {
        int maxAttempts = 100; // 最多尝试100次
        
        for (int i = 0; i < maxAttempts; i++)
        {
            int x = Random.Range(0, gridWidth);
            int y = Random.Range(0, gridHeight);
            Vector2Int pos = new Vector2Int(x, y);
            
            if (!IsCellOccupied(pos))
                return pos;
        }
        
        Debug.LogWarning("未找到空位！网格可能已满。");
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// 获取指定区域内的随机空位列表
    /// </summary>
    public List<Vector2Int> GetRandomEmptyCells(int count)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int maxAttempts = count * 10; // 防止死循环
        int attempts = 0;
        
        while (result.Count < count && attempts < maxAttempts)
        {
            Vector2Int pos = GetRandomEmptyCell();
            if (pos.x >= 0 && !result.Contains(pos))
            {
                result.Add(pos);
            }
            attempts++;
        }
        
        return result;
    }

    // ==================== 障碍物生成 ====================
    /// <summary>
    /// 生成矩形障碍物
    /// </summary>
    public void GenerateRectangleObstacle(int width, int height)
    {
        // 随机选择左下角起点
        Vector2Int startPos = GetRandomEmptyCell();
        if (startPos.x < 0) return;

        // 检查是否有足够空间
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(startPos.x + x, startPos.y + y);
                if (!IsValidGridPosition(pos) || IsCellOccupied(pos))
                {
                    // 空间不够，放弃生成
                    return;
                }
                cells.Add(pos);
            }
        }

        // 生成障碍物
        foreach (Vector2Int cell in cells)
        {
            GameObject obstacle = Instantiate(obstaclePrefab, GridToWorld(cell), Quaternion.identity, transform);
            obstacle.transform.localScale = Vector3.one * cellSize * 0.9f; // 稍微小一点，避免视觉重叠
            
            if (obstacleMaterial != null && obstacle.GetComponent<Renderer>() != null)
                obstacle.GetComponent<Renderer>().material = obstacleMaterial;
            
            RegisterCell(cell, CellType.Obstacle, obstacle);
        }

        Debug.Log($"生成矩形障碍物：{width}x{height}，起点：{startPos}");
    }

    // ==================== 得分球生成 ====================
    /// <summary>
    /// 生成指定数量的得分球（2x2大小）
    /// </summary>
    public void GenerateScoreBalls(int count)
    {
        int successCount = 0;
        int attempts = 0;
        int maxAttempts = count * 10; // 最多尝试次数
        
        while (successCount < count && attempts < maxAttempts)
        {
            attempts++;
            
            // 随机选择一个空位作为左下角
            Vector2Int startPos = GetRandomEmptyCell();
            if (startPos.x < 0) break; // 没有空位了
            
            // 检查2x2区域是否都为空，且不超出边界
            if (CanPlace2x2ScoreBall(startPos))
            {
                // 创建2x2得分球
                Create2x2ScoreBall(startPos);
                successCount++;
            }
        }

        Debug.Log($"生成 {successCount}/{count} 个得分球（2x2）");
    }
    
    /// <summary>
    /// 检查能否在指定位置放置2x2得分球
    /// </summary>
    bool CanPlace2x2ScoreBall(Vector2Int bottomLeft)
    {
        // 检查边界（确保右上角不超出网格）
        if (bottomLeft.x + 1 >= gridWidth || bottomLeft.y + 1 >= gridHeight)
            return false;
        
        // 检查2x2区域内所有格子是否为空
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector2Int checkPos = bottomLeft + new Vector2Int(x, y);
                if (IsCellOccupied(checkPos))
                    return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 创建2x2得分球
    /// </summary>
    void Create2x2ScoreBall(Vector2Int bottomLeft)
    {
        // 计算2x2区域的中心位置（世界坐标）
        Vector3 centerWorld = GridToWorld(bottomLeft) + new Vector3(cellSize / 2f, cellSize / 2f, 0f);
        
        // 创建得分球对象（只创建一个，但占据4个格子）
        GameObject ball = Instantiate(scoreBallPrefab, centerWorld, Quaternion.identity, transform);
        
        // 设置大小为2x2（原本是1x1的0.8倍，现在改成2倍）
        ball.transform.localScale = Vector3.one * cellSize * 2f * 0.9f; // 稍微小一点避免视觉重叠
        
        if (scoreBallMaterial != null && ball.GetComponent<Renderer>() != null)
            ball.GetComponent<Renderer>().material = scoreBallMaterial;
        
        // 注册4个格子都被占用（都指向同一个GameObject）
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector2Int cellPos = bottomLeft + new Vector2Int(x, y);
                RegisterCell(cellPos, CellType.ScoreBall, ball);
            }
        }
    }
    
    /// <summary>
    /// 清除2x2得分球（当蛇吃到时调用）
    /// </summary>
    public void Clear2x2ScoreBall(Vector2Int hitPos)
    {
        Vector2Int normalized = NormalizeGridPosition(hitPos);
        
        // 获取得分球对象
        if (!cellObjects.ContainsKey(normalized))
            return;
            
        GameObject ballObj = cellObjects[normalized];
        if (ballObj == null)
            return;
        
        // 找到并清除所有指向这个对象的格子（2x2区域）
        List<Vector2Int> cellsToClear = new List<Vector2Int>();
        foreach (var kvp in cellObjects)
        {
            if (kvp.Value == ballObj)
            {
                cellsToClear.Add(kvp.Key);
            }
        }
        
        // 清除所有相关格子
        foreach (var cell in cellsToClear)
        {
            occupiedCells.Remove(cell);
            cellTypes.Remove(cell);
            cellObjects.Remove(cell);
        }
        
        // 销毁对象
        Destroy(ballObj);
        
        Debug.Log($"清除2x2得分球，位置：{hitPos}，清除了 {cellsToClear.Count} 个格子");
    }

    // ==================== 调试可视化 ====================
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 绘制网格边界
        Gizmos.color = Color.cyan;
        Vector3 bottomLeft = new Vector3(gridOrigin.x, gridOrigin.y, 0);
        Vector3 bottomRight = new Vector3(gridOrigin.x + gridWidth * cellSize, gridOrigin.y, 0);
        Vector3 topLeft = new Vector3(gridOrigin.x, gridOrigin.y + gridHeight * cellSize, 0);
        Vector3 topRight = new Vector3(gridOrigin.x + gridWidth * cellSize, gridOrigin.y + gridHeight * cellSize, 0);
        
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}

