// ================================================================================
// TL;DR:
// 扑克牌管理器，负责加载卡牌、洗牌、显示和组合识别
// 实现横向6张牌显示，点击洗牌，0.1秒CD，显示组合名称
// ================================================================================
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class PokerManager : MonoBehaviour
{
    // ==================== 单例模式 ====================
    public static PokerManager Instance;

    // ==================== 扑克牌资源 ====================
    [Header("扑克牌配置")]
    public TextAsset cardsCSV;                    // cards.csv文件
    private List<PokerCard> allCards = new List<PokerCard>(); // 所有卡牌
    private List<PokerCard> currentCards = new List<PokerCard>(6); // 当前6张牌

    // ==================== UI组件 ====================
    [Header("UI引用")]
    public Transform cardContainer;               // 卡牌容器（6张卡的父对象）
    public GameObject cardPrefab;                 // 单张卡牌的预制体
    public Button shuffleButton;                  // 洗牌按钮
    public TextMeshProUGUI comboNameText;         // 组合名称文本

    [Header("显示设置")]
    public float cardSpacing = 100f;              // 卡牌间距
    public float cardXOffset = 0f;                // 卡牌整体X轴偏移（向右为正）
    public float cardYOffset = 0f;                // 卡牌整体Y轴偏移（向上为正）
    public float comboTextDisplayTime = 0.5f;     // 组合名称显示时间
    
    [Header("组合文字动画设置")]
    public AnimationCurve popInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);  // 弹出曲线（Elastic效果）
    public AnimationCurve popOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); // 退出曲线
    public float popInDuration = 0.3f;            // 弹出动画时长
    public float popOutDuration = 0.2f;           // 退出动画时长

    // ==================== 洗牌CD ====================
    [Header("洗牌设置")]
    public float shuffleCooldown = 0.1f;          // 洗牌冷却时间
    private float lastShuffleTime = 0f;           // 上次洗牌时间
    private bool canShuffle = true;               // 是否可以洗牌

    // ==================== 组件引用 ====================
    private PokerComboDetector comboDetector;
    private List<GameObject> cardObjects = new List<GameObject>(); // 当前卡牌对象

    // ==================== 生命周期 ====================
    void Awake()
    {
        Instance = this;
        comboDetector = gameObject.AddComponent<PokerComboDetector>();
    }

    void Start()
    {
        // ========== 临时测试代码 ==========
        Debug.Log("🎴 开始测试扑克牌资源加载...");
        
        // 测试单张图片加载
        Sprite testSprite = Resources.Load<Sprite>("Poke/png/1");
        if (testSprite != null)
        {
            Debug.Log($"✅ 测试成功！Sprite 加载正常");
            Debug.Log($"   - Sprite 名称: {testSprite.name}");
            Debug.Log($"   - Sprite 大小: {testSprite.rect.width} x {testSprite.rect.height}");
            Debug.Log($"   - Texture 大小: {testSprite.texture.width} x {testSprite.texture.height}");
        }
        else
        {
            Debug.LogError("❌ 测试失败！无法加载 Sprite: Poke/png/1");
            Debug.LogError("   请检查：");
            Debug.LogError("   1. PNG 文件是否在 Assets/Resources/Poke/png/ 文件夹");
            Debug.LogError("   2. Texture Type 是否设置为 'Sprite (2D and UI)'");
        }
        // ========== 测试代码结束 ==========
        
        LoadCardsFromCSV();
        InitializeUI();
        ShuffleCards(); // 初始洗牌
    }

    // ==================== 加载卡牌数据 ====================
    /// <summary>
    /// 从CSV文件加载所有卡牌数据
    /// </summary>
    void LoadCardsFromCSV()
    {
        if (cardsCSV == null)
        {
            Debug.LogError("❌ cards.csv 文件未设置！");
            return;
        }

        allCards.Clear();

        // 解析CSV
        string[] lines = cardsCSV.text.Split('\n');
        
        for (int i = 1; i < lines.Length; i++) // 跳过标题行
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(',');
            if (values.Length < 4) continue;

            PokerCard card = new PokerCard
            {
                fullName = values[0].Trim(),
                suit = values[1].Trim(),
                rank = values[2].Trim(),
                pngFilename = values[3].Trim()
            };

            // 加载图片资源（从 png 子文件夹）
            string spritePath = "Poke/png/" + Path.GetFileNameWithoutExtension(card.pngFilename);
            card.sprite = Resources.Load<Sprite>(spritePath);

            if (card.sprite == null)
            {
                Debug.LogWarning($"⚠️ 无法加载图片：{spritePath}");
            }
            else
            {
                Debug.Log($"✅ 成功加载：{spritePath}");
            }

            allCards.Add(card);
        }

        Debug.Log($"🎴 成功加载 {allCards.Count} 张扑克牌");
    }

    // ==================== UI初始化 ====================
    /// <summary>
    /// 初始化UI组件
    /// </summary>
    void InitializeUI()
    {
        // 绑定洗牌按钮
        if (shuffleButton != null)
        {
            shuffleButton.onClick.AddListener(OnShuffleButtonClick);
        }

        // 隐藏组合名称文本
        if (comboNameText != null)
        {
            comboNameText.gameObject.SetActive(false);
        }
    }

    // ==================== 洗牌功能 ====================
    /// <summary>
    /// 洗牌按钮点击
    /// </summary>
    public void OnShuffleButtonClick()
    {
        if (!canShuffle)
        {
            Debug.Log("⏱️ 洗牌冷却中...");
            return;
        }

        ShuffleCards();
        StartCoroutine(ShuffleCooldownRoutine());
    }

    /// <summary>
    /// 洗牌（随机抽6张牌）
    /// </summary>
    void ShuffleCards()
    {
        if (allCards.Count < 6)
        {
            Debug.LogError("❌ 卡牌数量不足！");
            return;
        }

        // 播放洗牌特效（在屏幕下方扑克区域）
        if (VFXManager.Instance != null && cardContainer != null)
        {
            // 转换UI位置到世界坐标
            Vector3 screenPos = cardContainer.position;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
            VFXManager.Instance.PlayShuffleCardsVFX(worldPos);
        }

        // 随机抽取6张牌
        currentCards.Clear();
        List<PokerCard> tempDeck = new List<PokerCard>(allCards);

        for (int i = 0; i < 6; i++)
        {
            int randomIndex = Random.Range(0, tempDeck.Count);
            currentCards.Add(tempDeck[randomIndex]);
            tempDeck.RemoveAt(randomIndex);
        }

        // 更新显示
        UpdateCardDisplay();

        // 检测组合
        CheckCombo();

        Debug.Log($"🔄 洗牌完成！当前牌：{GetCardNames()}");
    }

    /// <summary>
    /// 洗牌冷却协程
    /// </summary>
    IEnumerator ShuffleCooldownRoutine()
    {
        canShuffle = false;
        lastShuffleTime = Time.time;

        yield return new WaitForSeconds(shuffleCooldown);

        canShuffle = true;
    }

    // ==================== 卡牌显示 ====================
    /// <summary>
    /// 更新卡牌显示
    /// </summary>
/// <summary>
/// 更新卡牌显示
/// </summary>
void UpdateCardDisplay()
{
    // 清除旧的卡牌对象
    foreach (GameObject obj in cardObjects)
    {
        if (obj != null)
            Destroy(obj);
    }
    cardObjects.Clear();

    if (cardContainer == null || cardPrefab == null)
    {
        Debug.LogWarning("⚠️ 卡牌容器或预制体未设置");
        return;
    }

    // 禁用自动布局（如果有的话）
    var layoutGroup = cardContainer.GetComponent<UnityEngine.UI.LayoutGroup>();
    if (layoutGroup != null)
    {
        layoutGroup.enabled = false;
    }

    // 创建新的卡牌对象
    for (int i = 0; i < currentCards.Count; i++)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);

        // 设置位置（横向排列 + 可调偏移）
        RectTransform rect = cardObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            float xPos = (i - 2.5f) * cardSpacing + cardXOffset; // 居中排列 + X偏移
            float yPos = cardYOffset;                             // Y偏移
            rect.anchoredPosition = new Vector2(xPos, yPos);
        }

        // 设置图片（在子对象 CardImage 上）
        Transform cardImageTransform = cardObj.transform.Find("CardImage");
        if (cardImageTransform != null)
        {
            Image cardImage = cardImageTransform.GetComponent<Image>();
            if (cardImage != null)
            {
                if (currentCards[i].sprite != null)
                {
                    cardImage.sprite = currentCards[i].sprite;
                    Debug.Log($"✅ 第{i+1}张牌设置成功：{currentCards[i].fullName} → {currentCards[i].sprite.name}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ 第{i+1}张牌 Sprite 为空：{currentCards[i].fullName}");
                }
            }
            else
            {
                Debug.LogError($"❌ CardImage 上没有 Image 组件！");
            }
        }
        else
        {
            Debug.LogError($"❌ CardPrefab 下没有找到 CardImage 子对象！");
            Debug.LogError($"   请检查 CardPrefab 的结构是否正确");
        }

        cardObjects.Add(cardObj);
    }
}
    // ==================== 组合检测 ====================
    /// <summary>
    /// 检测当前6张牌的组合
    /// </summary>
    void CheckCombo()
    {
        if (comboDetector == null) return;

        PokerComboResult result = comboDetector.DetectCombo(currentCards);

        if (result.comboType != PokerComboType.None)
        {
            Debug.Log($"🎯 检测到组合：{result.comboName}");
            
            // 显示组合名称
            ShowComboName(result.comboName);

            // 触发地图生成（Phase 6会实现）
            if (SnakeGridManager.Instance != null)
            {
                if (result.hasObstacle)
                {
                    SnakeGridManager.Instance.GenerateRectangleObstacle(
                        result.obstacleSize.x, 
                        result.obstacleSize.y
                    );
                }
                else if (result.scoreReward > 0)
                {
                    SnakeGridManager.Instance.GenerateScoreBalls(result.scoreReward);
                }
            }
        }
    }

    /// <summary>
    /// 显示组合名称（0.5秒后自动消失）
    /// </summary>
    void ShowComboName(string comboName)
    {
        if (comboNameText == null) return;

        StopAllCoroutines(); // 停止之前的显示协程
        StartCoroutine(ShowComboNameRoutine(comboName));
    }

    /// <summary>
    /// 显示组合名称协程（带 Elastic 动画效果）
    /// </summary>
    IEnumerator ShowComboNameRoutine(string comboName)
    {
        comboNameText.text = comboName;
        comboNameText.gameObject.SetActive(true);

        // 弹出动画（Elastic 效果）
        float timer = 0f;
        Vector3 originalScale = Vector3.one;
        comboNameText.transform.localScale = Vector3.zero;

        while (timer < popInDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / popInDuration;
            
            // 使用 Elastic 曲线（超过1会产生回弹效果）
            float curveValue = popInCurve.Evaluate(progress);
            
            // 添加弹性回弹效果
            float elasticScale = curveValue;
            if (progress < 0.7f)
            {
                elasticScale = Mathf.Lerp(0f, 1.2f, progress / 0.7f); // 0-70%: 放大到1.2倍
            }
            else
            {
                elasticScale = Mathf.Lerp(1.2f, 1.0f, (progress - 0.7f) / 0.3f); // 70-100%: 回弹到1倍
            }
            
            comboNameText.transform.localScale = originalScale * elasticScale;
            yield return null;
        }
        comboNameText.transform.localScale = originalScale;

        // 停留时间
        yield return new WaitForSeconds(comboTextDisplayTime);

        // 退出动画（缩小淡出）
        timer = 0f;
        while (timer < popOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / popOutDuration;
            float curveValue = popOutCurve.Evaluate(progress);
            
            comboNameText.transform.localScale = originalScale * (1f - curveValue);
            
            // 淡出效果（可选）
            Color color = comboNameText.color;
            color.a = 1f - curveValue;
            comboNameText.color = color;
            
            yield return null;
        }

        // 恢复透明度
        Color finalColor = comboNameText.color;
        finalColor.a = 1f;
        comboNameText.color = finalColor;

        comboNameText.gameObject.SetActive(false);
    }

    // ==================== 工具方法 ====================
    /// <summary>
    /// 获取当前牌的名称（调试用）
    /// </summary>
    string GetCardNames()
    {
        string names = "";
        foreach (var card in currentCards)
        {
            names += card.fullName + ", ";
        }
        return names.TrimEnd(',', ' ');
    }
}

