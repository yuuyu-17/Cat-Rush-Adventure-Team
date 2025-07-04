using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    [Header("ゲーム全体のスクロール速度設定")]
    [Tooltip("背景や敵の基本的なスクロール速度")]
    public float baseScrollSpeed = 1.0f;

    [Tooltip("プレイヤーの移動速度がスクロール速度に与える影響の係数")]
    public float playerSpeedInfluence = 1f;

    private float _playerCurrentMoveSpeed = 1f;

    public float CurrentGameScrollSpeed { get; private set; }

    public bool IsGameOver { get; private set; } = false;

    public int totalCoinsCollected { get; private set; } // 獲得した総コイン数

    public Dictionary<ItemType, int> collectedItems { get; private set; } = new Dictionary<ItemType, int>(); // 獲得したアイテムとその数を管理

    [Header("プレイヤー能力値")]
    public float basePlayerMoveSpeed = 1.0f; // プレイヤーの基本移動速度
    public int playerMoveSpeedLevel { get; private set; } // 移動速度の現在の強化レベル
    public float moveSpeedPerLevel = 0.5f; // レベルアップごとの移動速度上昇量
    public int initialUpgradeCost = 10; // 最初の強化に必要なコイン数
    public int upgradeCostIncreasePerLevel = 5; // レベルアップごとにコストが増加する量

    public float currentRunDistanceTraveled { get; private set; } // 現在のゲームプレイでの移動距離

    //インゲームUI表示用
    [Header("インゲームUI表示設定")]
    private TextMeshProUGUI _distanceTextUI;

    // ゲーム開始時とシーン切り替え時に実行されるイベント
    public delegate void OnGameStartDelegate();
    public static event OnGameStartDelegate OnGameStart;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 初回起動時のみ能力値を初期化
            if (playerMoveSpeedLevel == 0) // まだ初期化されていない場合
            {
                playerMoveSpeedLevel = 1; // 初期レベルを1とする
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // シーンがロードされるたびにOnSceneLoadedメソッドを購読
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // シーンアンロード時に購読を解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// シーンがロードされたときに呼び出されるメソッド
    /// </summary>
    /// <param name="scene">ロードされたシーン</param>
    /// <param name="mode">ロードモード</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // インゲームシーンがロードされたときにのみ処理を実行
        if (scene.name == "InGameSene" || scene.name == "YourMainGameSceneName")
        {
            InitializeForNewGame();
            FindAndAssignGameUI(); // 新しいシーンのUI要素を探して割り当てる

            // ゲーム開始イベントを発火
            if (OnGameStart != null)
            {
                OnGameStart();
            }
        }
    }

    /// <summary>
    /// ゲーム開始時や新しいゲームが始まった時に呼ばれる初期化処理
    /// </summary>
    private void InitializeForNewGame()
    {
        IsGameOver = false;
        currentRunDistanceTraveled = 0f;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "InGameScene" || SceneManager.GetActiveScene().name == "YourMainGameSceneName")
        {
             InitializeForNewGame();
             FindAndAssignGameUI();
             if (OnGameStart != null)
             {
                 OnGameStart();
             }
        }
    }


    private void Update()
    {
        // ゲームがインゲームシーンにいるかどうかをチェック（UI更新のため）
        if (_distanceTextUI == null && (SceneManager.GetActiveScene().name == "InGameScene" || SceneManager.GetActiveScene().name == "YourMainGameSceneName"))
        {
            FindAndAssignGameUI(); // UI参照が失われた場合に再度検索
        }

        bool isTimeUp = (GameTimer.Instance != null && GameTimer.Instance.IsTimeUp);

        if (!IsGameOver && !isTimeUp)
        {
            CurrentGameScrollSpeed = baseScrollSpeed + (_playerCurrentMoveSpeed * playerSpeedInfluence);

            // プレイヤーの実際の移動速度は、基本速度 + (レベル * レベルごとの上昇量)
            float actualPlayerSpeed = basePlayerMoveSpeed + (playerMoveSpeedLevel - 1) * moveSpeedPerLevel;
            SetPlayerCurrentMoveSpeed(actualPlayerSpeed); // プレイヤーの移動速度を更新
            
            currentRunDistanceTraveled += CurrentGameScrollSpeed * Time.deltaTime;
            UpdateDistanceUI();
        }
        else
        {
            CurrentGameScrollSpeed = 0f;
            UpdateDistanceUI();
        }
    }

    public void SetPlayerCurrentMoveSpeed(float speed)
    {
        _playerCurrentMoveSpeed = speed;
    }

    //報酬獲得時に呼び出されるメソッド
    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            totalCoinsCollected += amount;
            Debug.Log($"コインを {amount} 枚獲得！合計: {totalCoinsCollected} 枚");
        }
    }

    public void AddItem(ItemType type, int count = 1)
    {
        if (type == ItemType.None || count <= 0) return;

        if (collectedItems.ContainsKey(type))
        {
            collectedItems[type] += count;
        }
        else
        {
            // Dictionaryにまだ存在しないアイテムタイプの場合
            collectedItems.Add(type, count);
        }
        Debug.Log($"{type.ToString()} を {count} 個獲得！現在: {collectedItems[type]} 個");
    }

    /// <summary>
    /// インゲームシーンのUI要素を探して割り当てます。
    /// </summary>
    private void FindAndAssignGameUI()
    {
        // シーン内の"DistanceText"という名前のTextMeshProUGUIを探す
        _distanceTextUI = GameObject.Find("DistanceText")?.GetComponent<TextMeshProUGUI>();
        if (_distanceTextUI == null)
        {
            Debug.LogWarning("インゲームシーンで 'DistanceText' という名前のTextMeshProUGUIが見つかりません。距離表示UIは機能しません。", this);
        }
        UpdateDistanceUI(); // UI参照が設定されたら一度更新
    }

    /// <summary>
    /// 現在のゲームプレイでの移動距離をUIテキストに表示します。
    /// </summary>
    private void UpdateDistanceUI()
    {
        if (_distanceTextUI != null)
        {
            _distanceTextUI.text = $"距離: {currentRunDistanceTraveled:F2} m";
        }
    }

    /// <summary>
    /// プレイヤーの移動速度を強化します。
    /// </summary>
    /// <returns>強化が成功した場合はtrue、コイン不足などで失敗した場合はfalseを返します。</returns>
    public bool UpgradePlayerMoveSpeed()
    {
        int cost = GetMoveSpeedUpgradeCost();
        if (totalCoinsCollected >= cost)
        {
            totalCoinsCollected -= cost;
            playerMoveSpeedLevel++;
            Debug.Log($"移動速度を強化！レベル {playerMoveSpeedLevel} になりました。残りコイン: {totalCoinsCollected}");
            return true;
        }
        else
        {
            Debug.Log("コインが足りません！");
            return false;
        }
    }

    /// <summary>
    /// 次の移動速度強化に必要なコイン数を取得します。
    /// </summary>
    public int GetMoveSpeedUpgradeCost()
    {
        // レベル1の時はinitialUpgradeCost、その後はレベルごとに増加
        return initialUpgradeCost + (playerMoveSpeedLevel - 1) * upgradeCostIncreasePerLevel;
    }

    /// <summary>
    /// 現在のプレイヤーの実際の移動速度（基本速度 + 強化分）を取得します。
    /// </summary>
    public float GetActualPlayerMoveSpeed()
    {
        return basePlayerMoveSpeed + (playerMoveSpeedLevel - 1) * moveSpeedPerLevel;
    }

    // ゲームオーバー処理（時間切れ以外でゲームを終了する場合）
    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log("ゲームオーバー！");
    }
}
