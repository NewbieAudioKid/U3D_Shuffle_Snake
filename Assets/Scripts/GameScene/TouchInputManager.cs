// ================================================================================
// TL;DR:
// 触摸输入管理器，实现拖拽控制贪吃蛇方向，并支持屏幕区域分离。
// 采用首次触摸点判断区域，支持跨区域拖拽，兼容PC鼠标和移动触摸。
//
// 目标：
// - 屏幕上方80%：贪吃蛇控制区
// - 屏幕下方20%：扑克牌区（不响应蛇控制）
// - 拖拽方向控制：首次触摸点 → 当前触摸点的矢量方向
// - 支持斜角移动（8方向）
// - 兼容PC鼠标和移动触摸输入
//
// 非目标：
// - 不处理扑克牌点击（由 PokerManager 负责）
// - 不处理游戏暂停等UI交互
// ================================================================================
using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
    // ==================== 单例模式 ====================
    public static TouchInputManager Instance;

    // ==================== 屏幕区域分割 ====================
    [Header("屏幕区域设置")]
    [Range(0f, 1f)]
    [Tooltip("扑克牌区域占屏幕的百分比（从底部算起）")]
    public float pokerZoneHeightRatio = 0.2f; // 下方20%为扑克区

    [Header("输入灵敏度")]
    [Tooltip("最小拖拽距离（像素），小于此距离不响应")]
    public float minDragDistance = 20f;
    
    [Header("速度调节设置")]
    [Tooltip("拖拽距离影响速度的缩放范围")]
    public float minSpeedMultiplier = 0.5f;  // 最小速度倍数（短距离拖拽）
    public float maxSpeedMultiplier = 2.0f;  // 最大速度倍数（长距离拖拽）
    public float maxDragDistanceForSpeed = 300f; // 达到最大速度所需的拖拽距离（像素）

    [Header("调试可视化")]
    public bool showDebugInfo = true;

    // ==================== 输入状态 ====================
    private bool isTouching = false;            // 是否正在触摸
    private Vector2 touchStartPosition;         // 触摸起始位置（屏幕坐标）
    private Vector2 currentTouchPosition;       // 当前触摸位置（屏幕坐标）
    private bool touchStartInSnakeZone = false; // 触摸是否开始于贪吃蛇区域

    // ==================== 生命周期 ====================
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 同时支持鼠标和触摸输入
        HandleInput();
    }

    // ==================== 输入处理 ====================
    /// <summary>
    /// 统一处理鼠标和触摸输入
    /// </summary>
    void HandleInput()
    {
        // PC鼠标输入
        if (Input.GetMouseButtonDown(0))
        {
            OnTouchStart(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isTouching)
        {
            OnTouchDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnTouchEnd();
        }

        // 移动端触摸输入（如果有触摸输入，优先使用）
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchStart(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isTouching)
                        OnTouchDrag(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnTouchEnd();
                    break;
            }
        }
    }

    /// <summary>
    /// 触摸开始
    /// </summary>
    void OnTouchStart(Vector2 screenPosition)
    {
        isTouching = true;
        touchStartPosition = screenPosition;
        currentTouchPosition = screenPosition;

        // 判断触摸起始位置是否在贪吃蛇区域
        touchStartInSnakeZone = IsInSnakeZone(screenPosition);

        if (showDebugInfo)
        {
            string zone = touchStartInSnakeZone ? "蛇区" : "扑克区";
            Debug.Log($"🖱️ 触摸开始：{screenPosition}，区域：{zone}");
        }
    }

    /// <summary>
    /// 触摸拖拽中
    /// </summary>
    void OnTouchDrag(Vector2 screenPosition)
    {
        currentTouchPosition = screenPosition;

        // 只有在贪吃蛇区域开始的触摸才响应
        if (!touchStartInSnakeZone) return;

        // 计算拖拽向量
        Vector2 dragVector = currentTouchPosition - touchStartPosition;

        // 检查是否超过最小拖拽距离
        if (dragVector.magnitude >= minDragDistance)
        {
            // 将屏幕拖拽方向传递给蛇控制器
            UpdateSnakeDirection(dragVector);
        }
    }

    /// <summary>
    /// 触摸结束
    /// </summary>
    void OnTouchEnd()
    {
        isTouching = false;
        touchStartInSnakeZone = false;

        // 恢复正常速度
        if (SnakeController.Instance != null)
        {
            SnakeController.Instance.SetSpeedMultiplier(1.0f);
        }

        if (showDebugInfo)
        {
            Debug.Log("🖱️ 触摸结束");
        }
    }

    // ==================== 区域判断 ====================
    /// <summary>
    /// 判断屏幕坐标是否在贪吃蛇区域（上方80%）
    /// </summary>
    bool IsInSnakeZone(Vector2 screenPosition)
    {
        float pokerZoneHeight = Screen.height * pokerZoneHeightRatio;
        return screenPosition.y > pokerZoneHeight;
    }

    /// <summary>
    /// 判断屏幕坐标是否在扑克牌区域（下方20%）
    /// </summary>
    public bool IsInPokerZone(Vector2 screenPosition)
    {
        float pokerZoneHeight = Screen.height * pokerZoneHeightRatio;
        return screenPosition.y <= pokerZoneHeight;
    }

    // ==================== 方向更新 ====================
    /// <summary>
    /// 根据拖拽向量更新蛇的方向和速度
    /// </summary>
    void UpdateSnakeDirection(Vector2 dragVector)
    {
        if (SnakeController.Instance == null) return;

        // 计算拖拽距离
        float dragDistance = dragVector.magnitude;

        // 归一化拖拽向量（屏幕坐标 Y 向上为正，与 Unity 一致）
        Vector2 direction = dragVector.normalized;

        // 根据拖拽距离计算速度倍数（线性插值）
        float speedMultiplier = Mathf.Lerp(
            minSpeedMultiplier, 
            maxSpeedMultiplier, 
            Mathf.Clamp01(dragDistance / maxDragDistanceForSpeed)
        );

        // 传递方向和速度倍数给蛇控制器
        SnakeController.Instance.SetDirection(direction);
        SnakeController.Instance.SetSpeedMultiplier(speedMultiplier);

        if (showDebugInfo)
        {
            Debug.Log($"🐍 方向更新：{direction}，拖拽距离：{dragDistance:F0}px，速度倍数：{speedMultiplier:F2}x");
        }
    }

    // ==================== 调试可视化 ====================
    void OnGUI()
    {
        if (!showDebugInfo) return;

        // // 显示区域分界线
        // float dividerY = Screen.height * (1f - pokerZoneHeightRatio);
        // GUI.color = Color.red;
        // GUI.Box(new Rect(0, dividerY - 2, Screen.width, 4), "");

        // 绘制触摸拖拽箭头（不显示文字）
        if (isTouching)
        {
            DrawArrow(touchStartPosition, currentTouchPosition);
        }
    }

    /// <summary>
    /// 绘制拖拽箭头（调试用）
    /// </summary>
    void DrawArrow(Vector2 start, Vector2 end)
    {
        // 转换为GUI坐标（Y轴翻转）
        Vector2 guiStart = new Vector2(start.x, Screen.height - start.y);
        Vector2 guiEnd = new Vector2(end.x, Screen.height - end.y);

        // 绘制线条（使用Box模拟）
        Vector2 direction = guiEnd - guiStart;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float distance = direction.magnitude;

        // 绘制主线
        GUIUtility.RotateAroundPivot(angle, guiStart);
        GUI.color = Color.yellow;
        GUI.Box(new Rect(guiStart.x, guiStart.y - 2, distance, 4), "");
        GUIUtility.RotateAroundPivot(-angle, guiStart);

        // 绘制起点
        GUI.color = Color.green;
        GUI.Box(new Rect(guiStart.x - 10, guiStart.y - 10, 20, 20), "");

        // 绘制终点
        GUI.color = Color.red;
        GUI.Box(new Rect(guiEnd.x - 10, guiEnd.y - 10, 20, 20), "");
    }

    // ==================== 公共接口 ====================
    /// <summary>
    /// 启用/禁用输入
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    /// <summary>
    /// 获取当前触摸状态
    /// </summary>
    public bool IsTouching()
    {
        return isTouching;
    }

    /// <summary>
    /// 获取触摸起始位置
    /// </summary>
    public Vector2 GetTouchStartPosition()
    {
        return touchStartPosition;
    }
}

