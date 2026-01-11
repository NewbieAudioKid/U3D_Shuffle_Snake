// ================================================================================
// TL;DR:
// 贪吃蛇核心控制器，管理蛇的移动、碰撞检测、增长和视觉表现。
// 采用 List<Vector2Int> 存储蛇身坐标，支持边界穿越和斜角移动。
//
// 目标：
// - 管理蛇身数据结构（头、身体、尾巴）
// - 实现自动移动（协程驱动，可调速度）
// - 支持8方向移动（上下左右 + 4个斜角）
// - 碰撞检测（撞自己、撞障碍物、吃得分球）
// - 支持左右边界穿越
// - 增长机制（吃球后增加一节身体）
//
// 非目标：
// - 不处理输入（由 InputManager 负责）
// - 不处理得分UI（由 GameManager 负责）
// - 不处理扑克牌系统（由 PokerManager 负责）
// ================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour
{
    // ==================== 单例模式 ====================
    public static SnakeController Instance;

    // ==================== 蛇的状态 ====================
    [Header("蛇的属性")]
    public float moveSpeed = 0.2f;          // 移动间隔时间（秒）
    public int initialLength = 3;           // 初始长度
    
    [Header("动态速度设置")]
    private float speedMultiplier = 1.0f;   // 速度倍数（由拖拽距离控制）
    private float currentMoveSpeed = 0.2f;  // 当前实际移动速度
    
    [Header("平滑移动设置")]
    public bool useSmoothMovement = true;   // 是否使用平滑移动
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 移动曲线
    
    [Header("移动方向")]
    public Vector2Int currentDirection = Vector2Int.up; // 当前移动方向（默认向上）
    private Vector2Int nextDirection = Vector2Int.up;    // 下一次移动方向（缓冲输入）

    // ==================== 蛇身数据 ====================
    private List<Vector2Int> snakeBody = new List<Vector2Int>(); // 蛇身坐标列表（[0]是头）
    private bool isGrowing = false;                               // 是否正在增长
    
    // ==================== 视觉对象 ====================
    private List<GameObject> snakeVisuals = new List<GameObject>(); // 蛇的视觉对象列表
    
    // ==================== 平滑移动数据 ====================
    private List<Vector3> visualTargetPositions = new List<Vector3>(); // 视觉对象的目标位置
    private List<Vector3> visualStartPositions = new List<Vector3>();  // 视觉对象的起始位置
    private float smoothMoveProgress = 0f;                              // 平滑移动进度 (0-1)
    
    // ==================== 游戏状态 ====================
    private bool isAlive = true;            // 蛇是否存活
    private Coroutine moveCoroutine;        // 移动协程引用

    // ==================== 生命周期 ====================
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeSnake();
        currentMoveSpeed = moveSpeed; // 初始化当前速度
        StartMoving();
    }

    // ==================== 初始化 ====================
    /// <summary>
    /// 初始化贪吃蛇（创建初始蛇身）
    /// </summary>
    void InitializeSnake()
    {
        if (SnakeGridManager.Instance == null)
        {
            Debug.LogError("❌ SnakeGridManager 未找到！");
            return;
        }

        // 清空数据
        snakeBody.Clear();
        ClearVisuals();

        // 在网格中心生成蛇
        int centerX = SnakeGridManager.Instance.gridWidth / 2;
        int centerY = SnakeGridManager.Instance.gridHeight / 2;

        // 创建初始蛇身（从头到尾）
        for (int i = 0; i < initialLength; i++)
        {
            Vector2Int pos = new Vector2Int(centerX, centerY - i); // 头在上，尾在下
            snakeBody.Add(pos);
            
            // 在网格中注册
            SnakeGridManager.Instance.RegisterCell(pos, i == 0 ? CellType.SnakeHead : CellType.SnakeBody, null);
        }

        // 创建视觉对象
        UpdateVisuals();
        
        // 附加蛇头特效
        if (VFXManager.Instance != null && snakeVisuals.Count > 0)
        {
            VFXManager.Instance.AttachSnakeHeadVFX(snakeVisuals[0].transform);
        }

        Debug.Log($"🐍 贪吃蛇初始化完成！起始位置：({centerX}, {centerY})，长度：{initialLength}");
    }

    // ==================== 移动控制 ====================
    /// <summary>
    /// 开始自动移动
    /// </summary>
    public void StartMoving()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    public void StopMoving()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    /// <summary>
    /// 移动协程（核心移动逻辑 - 平滑版）
    /// </summary>
    IEnumerator MoveRoutine()
    {
        while (isAlive)
        {
            // 更新方向（处理输入缓冲）
            currentDirection = nextDirection;

            // 记录起始位置（用于平滑移动）
            if (useSmoothMovement)
            {
                visualStartPositions.Clear();
                foreach (GameObject visual in snakeVisuals)
                {
                    if (visual != null)
                        visualStartPositions.Add(visual.transform.position);
                }
            }

            // 移动一步（更新逻辑坐标）
            MoveOneStep();

            // 平滑移动动画
            if (useSmoothMovement)
            {
                // 获取目标位置
                visualTargetPositions.Clear();
                for (int i = 0; i < snakeBody.Count && i < snakeVisuals.Count; i++)
                {
                    Vector3 targetPos = SnakeGridManager.Instance.GridToWorld(snakeBody[i]);
                    visualTargetPositions.Add(targetPos);
                }

                // 执行平滑移动（使用动态速度）
                float timer = 0f;
                while (timer < currentMoveSpeed)
                {
                    timer += Time.deltaTime;
                    smoothMoveProgress = Mathf.Clamp01(timer / currentMoveSpeed);
                    
                    // 使用曲线插值
                    float curveValue = moveCurve.Evaluate(smoothMoveProgress);
                    
                    // 更新所有蛇身视觉位置
                    for (int i = 0; i < snakeVisuals.Count; i++)
                    {
                        if (snakeVisuals[i] != null && i < visualStartPositions.Count && i < visualTargetPositions.Count)
                        {
                            snakeVisuals[i].transform.position = Vector3.Lerp(
                                visualStartPositions[i],
                                visualTargetPositions[i],
                                curveValue
                            );
                        }
                    }
                    
                    yield return null;
                }
                
                // 确保到达精确位置
                for (int i = 0; i < snakeVisuals.Count && i < visualTargetPositions.Count; i++)
                {
                    if (snakeVisuals[i] != null)
                        snakeVisuals[i].transform.position = visualTargetPositions[i];
                }
            }
            else
            {
                // 不使用平滑移动，直接等待（使用动态速度）
                yield return new WaitForSeconds(currentMoveSpeed);
            }
        }
    }

    /// <summary>
    /// 移动一步
    /// </summary>
    void MoveOneStep()
    {
        if (snakeBody.Count == 0) return;

        // 1. 计算新的头部位置
        Vector2Int currentHead = snakeBody[0];
        Vector2Int newHead = currentHead + currentDirection;

        // 2. 边界穿越处理
        newHead = SnakeGridManager.Instance.NormalizeGridPosition(newHead);

        // 3. 碰撞检测
        if (!CheckCollision(newHead))
        {
            // 游戏结束
            Die();
            return;
        }

        // 4. 移动蛇身
        snakeBody.Insert(0, newHead); // 在头部插入新位置

        // 5. 处理尾巴
        if (!isGrowing)
        {
            // 不增长：移除尾巴
            Vector2Int tail = snakeBody[snakeBody.Count - 1];
            snakeBody.RemoveAt(snakeBody.Count - 1);
            SnakeGridManager.Instance.UnregisterCell(tail);
        }
        else
        {
            // 增长：保留尾巴，重置增长标记
            isGrowing = false;
        }

        // 6. 更新网格注册
        SnakeGridManager.Instance.RegisterCell(newHead, CellType.SnakeHead, null);
        if (snakeBody.Count > 1)
        {
            // 旧的头变成身体
            SnakeGridManager.Instance.RegisterCell(snakeBody[1], CellType.SnakeBody, null);
        }

        // 7. 更新视觉
        UpdateVisuals();
    }

    // ==================== 碰撞检测 ====================
    /// <summary>
    /// 检测碰撞（返回false表示游戏结束）
    /// </summary>
    bool CheckCollision(Vector2Int newPos)
    {
        CellType cellType = SnakeGridManager.Instance.GetCellType(newPos);

        switch (cellType)
        {
            case CellType.Empty:
                // 空格，可以移动
                return true;

            case CellType.ScoreBall:
                // 吃到得分球
                EatScoreBall(newPos);
                return true;

            case CellType.Obstacle:
                // 撞到障碍物
                Debug.Log("💥 撞到障碍物！游戏结束！");
                return false;

            case CellType.SnakeBody:
                // 撞到自己（检查是否是尾巴，如果即将移走的尾巴则不算碰撞）
                if (!isGrowing && snakeBody.Count > 0 && newPos == snakeBody[snakeBody.Count - 1])
                {
                    // 撞到的是即将消失的尾巴，允许通过
                    return true;
                }
                Debug.Log("💥 撞到自己！游戏结束！");
                return false;

            case CellType.SnakeHead:
                // 不应该发生（头不会撞到头）
                return false;

            default:
                return true;
        }
    }

    /// <summary>
    /// 吃到得分球（2x2大小）
    /// </summary>
    void EatScoreBall(Vector2Int pos)
    {
        Debug.Log("🎯 吃到得分球（2x2）！");

        // 播放特效
        if (VFXManager.Instance != null)
        {
            Vector3 worldPos = SnakeGridManager.Instance.GridToWorld(pos);
            VFXManager.Instance.PlayCollectBallVFX(worldPos);
        }

        // 标记增长
        isGrowing = true;

        // 清除2x2得分球（会自动清除4个格子）
        SnakeGridManager.Instance.Clear2x2ScoreBall(pos);

        // 增加分数
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(10);
        }
    }

    // ==================== 死亡处理 ====================
    /// <summary>
    /// 蛇死亡
    /// </summary>
    void Die()
    {
        isAlive = false;
        StopMoving();
        
        // 移除蛇头特效
        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.RemoveSnakeHeadVFX();
        }

        Debug.Log("☠️ 游戏结束！");

        // 触发游戏结束
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame(false);
        }
    }

    // ==================== 方向控制 ====================
    /// <summary>
    /// 设置移动方向（外部调用，由InputManager调用）
    /// </summary>
    public void SetDirection(Vector2 inputDirection)
    {
        // 将输入向量转换为网格方向
        Vector2Int newDirection = Vector2Int.zero;

        // 归一化输入
        if (inputDirection.magnitude > 0.1f)
        {
            // 转换为8方向
            float angle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;
            
            // 根据角度确定方向
            if (angle >= -22.5f && angle < 22.5f)
                newDirection = Vector2Int.right;
            else if (angle >= 22.5f && angle < 67.5f)
                newDirection = new Vector2Int(1, 1);  // 右上
            else if (angle >= 67.5f && angle < 112.5f)
                newDirection = Vector2Int.up;
            else if (angle >= 112.5f && angle < 157.5f)
                newDirection = new Vector2Int(-1, 1); // 左上
            else if (angle >= 157.5f || angle < -157.5f)
                newDirection = Vector2Int.left;
            else if (angle >= -157.5f && angle < -112.5f)
                newDirection = new Vector2Int(-1, -1); // 左下
            else if (angle >= -112.5f && angle < -67.5f)
                newDirection = Vector2Int.down;
            else if (angle >= -67.5f && angle < -22.5f)
                newDirection = new Vector2Int(1, -1);  // 右下
        }

        // 防止反向移动（不能180度掉头）
        if (newDirection != Vector2Int.zero && newDirection != -currentDirection)
        {
            nextDirection = newDirection;
        }
    }

    /// <summary>
    /// 设置速度倍数（由拖拽距离控制）
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 0.1f, 5.0f); // 限制范围
        currentMoveSpeed = moveSpeed / speedMultiplier; // 速度倍数越大，移动间隔越小
        
        // 更新蛇头特效强度
        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.UpdateSnakeHeadVFXIntensity(speedMultiplier);
        }
    }

    // ==================== 视觉更新 ====================
    /// <summary>
    /// 更新蛇的视觉对象
    /// </summary>
    void UpdateVisuals()
    {
        if (SnakeGridManager.Instance == null) return;

        // 如果视觉对象数量与蛇身长度不匹配，重新创建
        if (snakeVisuals.Count != snakeBody.Count)
        {
            ClearVisuals();
            
            // 创建新的视觉对象
            for (int i = 0; i < snakeBody.Count; i++)
            {
                Vector2Int gridPos = snakeBody[i];
                Vector3 worldPos = SnakeGridManager.Instance.GridToWorld(gridPos);

                GameObject visual;
                if (i == 0)
                {
                    // 蛇头
                    visual = Instantiate(SnakeGridManager.Instance.snakeHeadPrefab, worldPos, Quaternion.identity, transform);
                }
                else
                {
                    // 蛇身
                    visual = Instantiate(SnakeGridManager.Instance.snakeBodyPrefab, worldPos, Quaternion.identity, transform);
                }

                // 设置缩放
                visual.transform.localScale = Vector3.one * SnakeGridManager.Instance.cellSize * 0.9f;

                // 应用材质
                if (SnakeGridManager.Instance.snakeMaterial != null && visual.GetComponent<Renderer>() != null)
                {
                    visual.GetComponent<Renderer>().material = SnakeGridManager.Instance.snakeMaterial;
                }

                snakeVisuals.Add(visual);
            }
            
            // 重新附加蛇头特效（因为蛇头对象重新创建了）
            if (VFXManager.Instance != null && snakeVisuals.Count > 0)
            {
                VFXManager.Instance.AttachSnakeHeadVFX(snakeVisuals[0].transform);
            }
        }
        else if (!useSmoothMovement)
        {
            // 不使用平滑移动时，直接更新位置
            for (int i = 0; i < snakeBody.Count && i < snakeVisuals.Count; i++)
            {
                if (snakeVisuals[i] != null)
                {
                    Vector3 worldPos = SnakeGridManager.Instance.GridToWorld(snakeBody[i]);
                    snakeVisuals[i].transform.position = worldPos;
                }
            }
        }
        // 如果使用平滑移动，位置更新由协程处理，这里不做处理
    }

    /// <summary>
    /// 清除所有视觉对象
    /// </summary>
    void ClearVisuals()
    {
        foreach (GameObject obj in snakeVisuals)
        {
            if (obj != null)
                Destroy(obj);
        }
        snakeVisuals.Clear();
    }

    // ==================== 调试可视化 ====================
    void OnDrawGizmos()
    {
        if (!Application.isPlaying || snakeBody.Count == 0) return;

        // 绘制蛇头方向
        if (SnakeGridManager.Instance != null && snakeBody.Count > 0)
        {
            Vector3 headPos = SnakeGridManager.Instance.GridToWorld(snakeBody[0]);
            Vector3 direction = new Vector3(currentDirection.x, currentDirection.y, 0).normalized;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(headPos, headPos + direction * 0.5f);
        }
    }
}

