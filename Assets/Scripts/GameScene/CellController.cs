// ================================================================================
// TL;DR:
// 贪吃蛇游戏中的格子对象控制器（障碍物、得分球）
// 简化版本，只负责基础的碰撞和销毁逻辑
//
// 目标:
// - 标识格子类型（障碍物/得分球）
// - 提供碰撞回调接口
// - 被吃掉时通知网格管理器注销
//
// 非目标：
// - 不处理复杂的游戏逻辑（由各Manager负责）
// - 不处理特效和音效（应由专门的特效系统负责）
// ================================================================================
using UnityEngine;

public class CellController : MonoBehaviour
{
    public CellType cellType = CellType.Empty;
    public Vector2Int gridPosition;
    public bool isDestroyed = false;

    /// <summary>
    /// 初始化格子对象
    /// </summary>
    public void Init(CellType type, Vector2Int gridPos, Material mat = null)
    {
        this.cellType = type;
        this.gridPosition = gridPos;
        this.isDestroyed = false;
        
        if (mat != null && GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().material = mat;
        }
        
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 被吃掉/碰撞时调用
    /// </summary>
    public void OnCollected()
    {
        if (isDestroyed) return;
        
        isDestroyed = true;
        
        // 通知网格管理器注销此格子
        if (SnakeGridManager.Instance != null)
        {
            SnakeGridManager.Instance.UnregisterCell(gridPosition);
        }
        
        // 销毁GameObject
        Destroy(gameObject);
    }
}
