// ================================================================================
// TL;DR:
// 特效管理器，使用 Layer Lab 的粒子特效素材
// 管理游戏中的各种视觉特效：吃球、洗牌、游戏结束等
//
// 目标：
// - 吃得分球时播放星星/闪光特效
// - 洗牌时播放卡牌刷新特效
// - 游戏胜利时播放庆祝特效
// - 对象池管理粒子系统，提高性能
//
// 非目标：
// - 不处理UI动画（由各自的UI脚本负责）
// - 不处理音效（如需要，由 AudioManager 负责）
// ================================================================================
using UnityEngine;
using System.Collections.Generic;

public class VFXManager : MonoBehaviour
{
    // ==================== 单例模式 ====================
    public static VFXManager Instance;

    // ==================== 粒子特效预制体引用 ====================
    [Header("粒子特效预制体（Layer Lab）")]
    [Tooltip("从 Layer Lab/GUI Pro-CasualGame/Prefabs/Prefabs_DemoScene_Particle/ 中选择")]
    public GameObject collectBallVFX;       // 吃球特效（星星/闪光）
    public GameObject shuffleCardsVFX;      // 洗牌特效
    public GameObject victoryVFX;           // 胜利特效（烟花/庆祝）
    public GameObject gameOverVFX;          // 失败特效（可选）
    public GameObject snakeHeadTrailVFX;    // 蛇头拖尾特效（新增）

    [Header("特效设置")]
    public float vfxLifetime = 2f;          // 特效生存时间（秒）
    public bool useObjectPooling = true;    // 是否使用对象池

    [Header("蛇头特效设置")]
    public bool enableSnakeHeadVFX = true;  // 是否启用蛇头特效
    public float minTrailEmission = 10f;    // 最小粒子发射速率（慢速时）
    public float maxTrailEmission = 50f;    // 最大粒子发射速率（快速时）

    // ==================== 对象池 ====================
    private Dictionary<string, Queue<GameObject>> vfxPool = new Dictionary<string, Queue<GameObject>>();
    private Transform vfxContainer;
    
    // 蛇头特效实例（持续存在）
    private GameObject snakeHeadVFXInstance;
    private ParticleSystem snakeHeadParticleSystem;

    // ==================== 生命周期 ====================
    void Awake()
    {
        Instance = this;
        InitializeVFXContainer();
    }

    void InitializeVFXContainer()
    {
        // 创建特效容器（用于组织场景）
        GameObject container = new GameObject("VFX_Container");
        vfxContainer = container.transform;
        vfxContainer.SetParent(transform);
    }

    // ==================== 公共接口 ====================

    /// <summary>
    /// 播放吃球特效
    /// </summary>
    public void PlayCollectBallVFX(Vector3 position)
    {
        if (collectBallVFX != null)
        {
            position.z = 0f; // 强制设置 Z 坐标，确保在摄像机视野内
            PlayVFX(collectBallVFX, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("⚠️ Collect Ball VFX 预制体未设置！");
        }
    }

    /// <summary>
    /// 播放洗牌特效
    /// </summary>
    public void PlayShuffleCardsVFX(Vector3 position)
    {
        if (shuffleCardsVFX != null)
        {
            position.z = 0f; // 强制设置 Z 坐标
            PlayVFX(shuffleCardsVFX, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("⚠️ Shuffle Cards VFX 预制体未设置！");
        }
    }

    /// <summary>
    /// 播放胜利特效
    /// </summary>
    public void PlayVictoryVFX(Vector3 position)
    {
        if (victoryVFX != null)
        {
            position.z = 0f; // 强制设置 Z 坐标
            PlayVFX(victoryVFX, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("⚠️ Victory VFX 预制体未设置！");
        }
    }

    /// <summary>
    /// 播放失败特效
    /// </summary>
    public void PlayGameOverVFX(Vector3 position)
    {
        if (gameOverVFX != null)
        {
            position.z = 0f; // 强制设置 Z 坐标
            PlayVFX(gameOverVFX, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("⚠️ Game Over VFX 预制体未设置！");
        }
    }

    /// <summary>
    /// 创建并附加蛇头特效（持续跟随蛇头）
    /// </summary>
    public void AttachSnakeHeadVFX(Transform snakeHeadTransform)
    {
        if (!enableSnakeHeadVFX || snakeHeadTrailVFX == null || snakeHeadTransform == null)
            return;

        // 如果已存在，先销毁
        if (snakeHeadVFXInstance != null)
        {
            Destroy(snakeHeadVFXInstance);
        }

        // 创建新的蛇头特效
        snakeHeadVFXInstance = Instantiate(snakeHeadTrailVFX, snakeHeadTransform);
        snakeHeadVFXInstance.transform.localPosition = Vector3.zero;
        snakeHeadVFXInstance.transform.localRotation = Quaternion.identity;
        snakeHeadVFXInstance.SetActive(true);

        // 获取粒子系统引用
        snakeHeadParticleSystem = snakeHeadVFXInstance.GetComponent<ParticleSystem>();
        if (snakeHeadParticleSystem == null)
        {
            snakeHeadParticleSystem = snakeHeadVFXInstance.GetComponentInChildren<ParticleSystem>();
        }

        Debug.Log("✨ 蛇头特效已附加");
    }

    /// <summary>
    /// 更新蛇头特效强度（根据速度）
    /// </summary>
    public void UpdateSnakeHeadVFXIntensity(float speedMultiplier)
    {
        if (!enableSnakeHeadVFX || snakeHeadParticleSystem == null)
            return;

        // 根据速度调整粒子发射速率
        var emission = snakeHeadParticleSystem.emission;
        float targetEmissionRate = Mathf.Lerp(minTrailEmission, maxTrailEmission, (speedMultiplier - 0.5f) / 1.5f);
        emission.rateOverTime = targetEmissionRate;

        // 可选：根据速度调整粒子大小
        var main = snakeHeadParticleSystem.main;
        float sizeMultiplier = Mathf.Lerp(0.8f, 1.5f, (speedMultiplier - 0.5f) / 1.5f);
        main.startSizeMultiplier = sizeMultiplier;
    }

    /// <summary>
    /// 移除蛇头特效
    /// </summary>
    public void RemoveSnakeHeadVFX()
    {
        if (snakeHeadVFXInstance != null)
        {
            Destroy(snakeHeadVFXInstance);
            snakeHeadVFXInstance = null;
            snakeHeadParticleSystem = null;
        }
    }

    // ==================== 内部逻辑 ====================

    /// <summary>
    /// 通用特效播放方法
    /// </summary>
    void PlayVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation)
    {
        if (vfxPrefab == null)
        {
            Debug.LogError("❌ VFX 预制体为空！请检查 VFXManager 的引用设置。");
            return;
        }

        GameObject vfxInstance;

        if (useObjectPooling)
        {
            vfxInstance = GetFromPool(vfxPrefab);
        }
        else
        {
            vfxInstance = Instantiate(vfxPrefab, vfxContainer);
        }

        vfxInstance.transform.position = position;
        vfxInstance.transform.rotation = rotation;
        vfxInstance.SetActive(true);

        // 调试日志
        Debug.Log($"✅ 播放特效：{vfxPrefab.name} 位置：{position}");
        
        // 检查粒子系统
        ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            ps = vfxInstance.GetComponentInChildren<ParticleSystem>();
        }
        
        if (ps != null)
        {
            Debug.Log($"   粒子系统：isPlaying={ps.isPlaying}, particleCount={ps.particleCount}, " +
                      $"emission={ps.emission.enabled}, duration={ps.main.duration}");
            
            // 强制播放粒子
            if (!ps.isPlaying)
            {
                ps.Play();
                Debug.Log("   🔄 强制播放粒子系统");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ {vfxPrefab.name} 上没有找到 ParticleSystem 组件！");
        }

        // 自动销毁或回收
        StartCoroutine(RecycleVFX(vfxInstance, vfxPrefab));
    }

    /// <summary>
    /// 从对象池获取特效
    /// </summary>
    GameObject GetFromPool(GameObject prefab)
    {
        string key = prefab.name;

        if (!vfxPool.ContainsKey(key))
        {
            vfxPool[key] = new Queue<GameObject>();
        }

        if (vfxPool[key].Count > 0)
        {
            GameObject obj = vfxPool[key].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab, vfxContainer);
            obj.name = key;
            return obj;
        }
    }

    /// <summary>
    /// 回收特效到对象池
    /// </summary>
    System.Collections.IEnumerator RecycleVFX(GameObject vfx, GameObject prefab)
    {
        yield return new WaitForSeconds(vfxLifetime);

        if (useObjectPooling)
        {
            vfx.SetActive(false);
            string key = prefab.name;
            if (!vfxPool.ContainsKey(key))
            {
                vfxPool[key] = new Queue<GameObject>();
            }
            vfxPool[key].Enqueue(vfx);
        }
        else
        {
            Destroy(vfx);
        }
    }

    // ==================== 工具方法 ====================

    /// <summary>
    /// 清理所有特效
    /// </summary>
    public void ClearAllVFX()
    {
        foreach (Transform child in vfxContainer)
        {
            Destroy(child.gameObject);
        }
        vfxPool.Clear();
    }
}

