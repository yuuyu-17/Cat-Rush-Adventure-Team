using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Dictionaryを使うために必要

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

    // 獲得したアイテムとその数を管理するDictionary
    public Dictionary<ItemType, int> collectedItems { get; private set; } = new Dictionary<ItemType, int>();

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

    private void Start()
    {
        IsGameOver = false;
        totalCoinsCollected = 0; // ゲーム開始時にコインをリセット
        collectedItems.Clear(); // ゲーム開始時にアイテムをリセット
        // 各アイテムタイプをDictionaryに初期化（こうすると後で追加が楽）
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            if (type != ItemType.None) // Noneは除外
            {
                collectedItems[type] = 0;
            }
        }
    }

    private void Update()
    {
        // GameTimerからIsTimeUpの状態を取得してゲーム進行を判断
        bool isTimeUp = (GameTimer.Instance != null && GameTimer.Instance.IsTimeUp);

        if (!IsGameOver && !isTimeUp) // ゲームが終了しておらず、時間切れでもない場合のみ
        {
            CurrentGameScrollSpeed = baseScrollSpeed + (_playerCurrentMoveSpeed * playerSpeedInfluence);
        }
        else
        {
            CurrentGameScrollSpeed = 0f; // ゲーム終了時または時間切れ時はスクロール停止
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

    // ゲームオーバー処理（時間切れ以外でゲームを終了する場合）
    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log("ゲームオーバー！");
    }
}
