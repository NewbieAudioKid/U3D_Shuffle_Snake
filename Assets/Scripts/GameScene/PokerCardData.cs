// ================================================================================
// TL;DR:
// 扑克牌数据结构定义
// 包含单张卡牌信息和组合类型枚举
// ================================================================================
using UnityEngine;

/// <summary>
/// 单张扑克牌数据
/// </summary>
[System.Serializable]
public class PokerCard
{
    public string fullName;      // 完整名称（如 "Spades_Ace"）
    public string suit;          // 花色（Spades/Hearts/Clubs/Diamonds/Joker）
    public string rank;          // 点数（Ace/2-10/Jack/Queen/King/Color/Black）
    public string pngFilename;   // PNG文件名
    public string psdFilename;   // PSD文件名
    public Sprite sprite;        // 加载的图片资源
    
    /// <summary>
    /// 获取数值（用于组合判断）
    /// </summary>
    public int GetValue()
    {
        switch (rank)
        {
            case "3": return 3;
            case "4": return 4;
            case "5": return 5;
            case "6": return 6;
            case "7": return 7;
            case "8": return 8;
            case "9": return 9;
            case "10": return 10;
            case "Jack": return 11;
            case "Queen": return 12;
            case "King": return 13;
            case "Ace": return 14;
            case "2": return 15;
            case "Color": return 16;  // 小王
            case "Black": return 17;  // 大王
            default: return 0;
        }
    }
}

/// <summary>
/// 扑克组合类型枚举
/// </summary>
public enum PokerComboType
{
    None,               // 无组合
    Single,             // 单张
    Pair,               // 对子
    Three,              // 三张
    ThreeWithOne,       // 三带一
    ThreeWithPair,      // 三带二
    Straight,           // 顺子（5张及以上连续）
    DoubleStraight,     // 连对（3对及以上）
    Plane,              // 飞机（2个及以上三张）
    PlaneWithSingles,   // 飞机带单张
    PlaneWithPairs,     // 飞机带对子
    FourWithTwo,        // 四带二（四张+两单或两对）
    Bomb,               // 炸弹（四张）
    RocketBomb          // 王炸（大小王）
}

/// <summary>
/// 扑克组合结果
/// </summary>
public class PokerComboResult
{
    public PokerComboType comboType;
    public string comboName;
    public int scoreReward;     // 得分球数量
    public bool hasObstacle;    // 是否生成障碍物
    public Vector2Int obstacleSize; // 障碍物尺寸
}

