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
    public float playerSpeedInfluence = 0f;

    private float _playerCurrentMoveSpeed = 0f;

    public float CurrentGameScrollSpeed { get; private set; }

    public bool IsGameOver { get; private set; } = false;

    public int totalCoinsCollected { get; private set; } // 獲得した総コイン数

    public Dictionary<ItemType, int> collectedItems { get; private set; } = new Dictionary<ItemType, int>(); // 獲得したアイテムとその数を管理するDictionary

    public float currentRunDistanceTraveled { get; private set; } // 現在のゲームプレイでの移動距離

    //インゲームUI表示用
    [Header("インゲームUI表示設定")]
    [Tooltip("現在の移動距離を表示するTextMeshProUGUIコンポーネントを割り当ててください。")]
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
        // シーンアンロード時に購読を解除 (メモリリーク防止)
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
        // "InGameScene" は実際のインゲームシーン名に置き換えてください
        if (scene.name == "InGameSene" || scene.name == "YourMainGameSceneName") 
        {
            InitializeForNewGame();
            FindAndAssignGameUI(); // 新しいシーンのUI要素を探して割り当てる

            // ゲーム開始イベントを発火 (他のスクリプトがゲーム開始を検知できるように)
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
        currentRunDistanceTraveled = 0f; // 距離は新しいゲームごとにリセット
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
            //現在のゲームプレイでの移動距離を更新
            currentRunDistanceTraveled += CurrentGameScrollSpeed * Time.deltaTime;
            UpdateDistanceUI();
        }
        else
        {
            CurrentGameScrollSpeed = 0f;
            // ゲーム終了時にUIを最終更新 (距離が固定されるため)
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

    // ゲームオーバー処理（時間切れ以外でゲームを終了する場合）
    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log("ゲームオーバー！");
    }
}
