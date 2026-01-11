// ================================================================================
// TL;DR:
// 扑克牌组合识别器，实现斗地主规则的牌型判断
// 支持：单张、对子、三张、顺子、连对、飞机、四带二等
// ================================================================================
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PokerComboDetector : MonoBehaviour
{
    /// <summary>
    /// 识别6张牌的组合类型
    /// </summary>
    public PokerComboResult DetectCombo(List<PokerCard> cards)
    {
        if (cards == null || cards.Count != 6)
            return CreateResult(PokerComboType.None);

        // 统计每个点数的数量
        Dictionary<int, int> valueCount = new Dictionary<int, int>();
        foreach (var card in cards)
        {
            int value = card.GetValue();
            if (value > 0)
            {
                if (!valueCount.ContainsKey(value))
                    valueCount[value] = 0;
                valueCount[value]++;
            }
        }

        // 按优先级检测组合（从复杂到简单）
        PokerComboResult result;

        // 1. 检测顺子（6张连续）
        result = CheckStraight(valueCount, cards);
        if (result.comboType != PokerComboType.None) return result;

        // 2. 检测连对（3对连续）
        result = CheckDoubleStraight(valueCount);
        if (result.comboType != PokerComboType.None) return result;

        // 3. 检测飞机（2个三张）
        result = CheckPlane(valueCount);
        if (result.comboType != PokerComboType.None) return result;

        // 4. 检测四带二
        result = CheckFourWithTwo(valueCount);
        if (result.comboType != PokerComboType.None) return result;

        // 5. 检测三带一/三带二
        result = CheckThreeWith(valueCount);
        if (result.comboType != PokerComboType.None) return result;

        // 6. 检测基础牌型（对子、三张）
        result = CheckBasicCombos(valueCount);
        return result;
    }

    // ==================== 检测方法 ====================

    /// <summary>
    /// 检测顺子（5张及以上连续，不含2和王）
    /// </summary>
    PokerComboResult CheckStraight(Dictionary<int, int> valueCount, List<PokerCard> cards)
    {
        // 找出所有单张的点数
        List<int> singleValues = valueCount.Where(kv => kv.Value == 1).Select(kv => kv.Key).ToList();
        
        if (singleValues.Count < 5) return CreateResult(PokerComboType.None);

        // 排除2和王（15以上）
        singleValues = singleValues.Where(v => v < 15).OrderBy(v => v).ToList();
        
        if (singleValues.Count < 5) return CreateResult(PokerComboType.None);

        // 检查是否连续
        int consecutiveCount = 1;
        for (int i = 1; i < singleValues.Count; i++)
        {
            if (singleValues[i] == singleValues[i-1] + 1)
                consecutiveCount++;
            else
                consecutiveCount = 1;

            if (consecutiveCount >= 5)
            {
                // 找到顺子
                return CreateResult(PokerComboType.Straight, "STRAIGHT", 20);
            }
        }

        return CreateResult(PokerComboType.None);
    }

    /// <summary>
    /// 检测连对（3对及以上连续）
    /// </summary>
    PokerComboResult CheckDoubleStraight(Dictionary<int, int> valueCount)
    {
        // 找出所有对子的点数
        List<int> pairValues = valueCount.Where(kv => kv.Value == 2).Select(kv => kv.Key).ToList();
        
        if (pairValues.Count < 3) return CreateResult(PokerComboType.None);

        // 排序并检查连续性
        pairValues = pairValues.OrderBy(v => v).ToList();
        
        int consecutiveCount = 1;
        for (int i = 1; i < pairValues.Count; i++)
        {
            if (pairValues[i] == pairValues[i-1] + 1)
                consecutiveCount++;
            else
                consecutiveCount = 1;

            if (consecutiveCount >= 3)
            {
                return CreateResult(PokerComboType.DoubleStraight, "DOUBLE STRAIGHT", 20);
            }
        }

        return CreateResult(PokerComboType.None);
    }

    /// <summary>
    /// 检测飞机（2个及以上三张）
    /// </summary>
    PokerComboResult CheckPlane(Dictionary<int, int> valueCount)
    {
        // 找出所有三张的点数
        List<int> threeValues = valueCount.Where(kv => kv.Value == 3).Select(kv => kv.Key).ToList();
        
        if (threeValues.Count < 2) return CreateResult(PokerComboType.None);

        // 排序并检查连续性
        threeValues = threeValues.OrderBy(v => v).ToList();
        
        for (int i = 1; i < threeValues.Count; i++)
        {
            if (threeValues[i] == threeValues[i-1] + 1)
            {
                // 找到飞机
                return CreateResult(PokerComboType.Plane, "PLANE", 40);
            }
        }

        return CreateResult(PokerComboType.None);
    }

    /// <summary>
    /// 检测四带二（四张+两单或两对）
    /// </summary>
    PokerComboResult CheckFourWithTwo(Dictionary<int, int> valueCount)
    {
        bool hasFour = valueCount.Values.Any(c => c == 4);
        if (!hasFour) return CreateResult(PokerComboType.None);

        int remainingCards = valueCount.Values.Where(c => c != 4).Sum();
        if (remainingCards == 2)
        {
            return CreateResult(PokerComboType.FourWithTwo, "FOUR WITH TWO", 40);
        }

        return CreateResult(PokerComboType.None);
    }

    /// <summary>
    /// 检测三带一/三带二
    /// </summary>
    PokerComboResult CheckThreeWith(Dictionary<int, int> valueCount)
    {
        bool hasThree = valueCount.Values.Any(c => c == 3);
        if (!hasThree) return CreateResult(PokerComboType.None);

        int remainingCards = valueCount.Values.Where(c => c != 3).Sum();
        
        if (remainingCards == 1)
        {
            return CreateResult(PokerComboType.ThreeWithOne, "THREE WITH ONE", 10);
        }
        else if (remainingCards == 2)
        {
            bool hasPair = valueCount.Values.Where(c => c != 3).Any(c => c == 2);
            if (hasPair)
            {
                return CreateResult(PokerComboType.ThreeWithPair, "THREE WITH PAIR", 10);
            }
        }

        return CreateResult(PokerComboType.None);
    }

    /// <summary>
    /// 检测基础组合（对子、三张）
    /// </summary>
    PokerComboResult CheckBasicCombos(Dictionary<int, int> valueCount)
    {
        // 统计不同数量的出现次数
        int pairCount = valueCount.Values.Count(c => c == 2);
        int threeCount = valueCount.Values.Count(c => c == 3);

        // 三对
        if (pairCount == 3)
        {
            return CreateResultWithObstacle(PokerComboType.Pair, "THREE PAIRS");
        }

        // 两个三张
        if (threeCount == 2)
        {
            return CreateResultWithObstacle(PokerComboType.Three, "TWO THREES");
        }

        // 一对 + 一个三张
        if (pairCount == 1 && threeCount == 1)
        {
            return CreateResultWithObstacle(PokerComboType.Pair, "PAIR + THREE");
        }

        // 单个三张
        if (threeCount == 1)
        {
            return CreateResultWithObstacle(PokerComboType.Three, "THREE");
        }

        // 单个对子
        if (pairCount == 1)
        {
            return CreateResultWithObstacle(PokerComboType.Pair, "PAIR");
        }

        return CreateResult(PokerComboType.None);
    }

    // ==================== 结果创建 ====================

    /// <summary>
    /// 创建基础结果（用于高级组合）
    /// </summary>
    PokerComboResult CreateResult(PokerComboType type, string name = "NONE", int score = 0)
    {
        return new PokerComboResult
        {
            comboType = type,
            comboName = name,
            scoreReward = score,
            hasObstacle = false,
            obstacleSize = Vector2Int.zero
        };
    }

    /// <summary>
    /// 创建带障碍物的结果（用于对子和三张）
    /// </summary>
    PokerComboResult CreateResultWithObstacle(PokerComboType type, string name)
    {
        // 20%概率生成障碍物，80%生成得分球
        bool generateObstacle = Random.value < 0.2f;
        
        if (generateObstacle)
        {
            // 生成 5-15 随机大小的障碍物
            int width = Random.Range(5, 16);
            int height = Random.Range(5, 16);
            
            return new PokerComboResult
            {
                comboType = type,
                comboName = name,
                scoreReward = 0,
                hasObstacle = true,
                obstacleSize = new Vector2Int(width, height)
            };
        }
        else
        {
            // 生成1个得分球
            return new PokerComboResult
            {
                comboType = type,
                comboName = name,
                scoreReward = 1,
                hasObstacle = false,
                obstacleSize = Vector2Int.zero
            };
        }
    }
}

