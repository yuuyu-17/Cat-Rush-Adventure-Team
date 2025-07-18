using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [Header("ゲーム時間設定")]
    [Tooltip("ゲームが終了するまでの時間（秒）です。")]
    [SerializeField] private float gameDuration = 60.0f; // ゲームの継続時間（デフォルトは60秒）
    private float _currentTime; // 現在の残り時間

    public bool IsTimeUp { get; private set; } = false;// ゲームが時間切れになったかどうかを示すフラグ

    [Header("リザルト画面設定")]
    [Tooltip("時間切れになった時に移行するリザルトシーンの名前です。")]
    [SerializeField] private string resultSceneName = "ResultSene"; // 移行するリザルトシーン名

    [Header("UI表示設定")]
    [Tooltip("残り時間を表示するTextMeshProUGUIコンポーネントを割り当ててください。")]
    [SerializeField] private TextMeshProUGUI timerTextUI; // 残り時間を表示するText

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // このタイマーがインゲーム部分だけで使うなら不要
        }
        else
        {
            // シーン内に既にGameTimerのインスタンスが存在する場合、重複するこのオブジェクトを破棄
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ゲーム開始時にタイマーを初期化します。
    /// </summary>
    private void Start()
    {
        _currentTime = gameDuration; // 設定されたゲーム継続時間で残り時間を初期化
        IsTimeUp = false; // 時間切れフラグをリセット
        Debug.Log("ゲームタイマー開始！残り時間: " + _currentTime + "秒");
        UpdateTimerUI(); // ゲーム開始時に一度UIを更新
    }

    /// <summary>
    /// フレームごとにタイマーを更新し、時間切れを判定
    /// </summary>
    private void Update()
    {
        // 時間切れになっていない場合のみタイマーを減らす
        if (!IsTimeUp)
        {
            _currentTime -= Time.deltaTime;
            UpdateTimerUI();

            // 時間が0以下になったらリザルト画面への移行処理を呼び出す
            if (_currentTime <= 0)
            {
                _currentTime = 0; // 時間がマイナスにならないように固定
                UpdateTimerUI(); // 最後に00:00と表示されるように更新
                TransitionToResult();
            }
        }
    }

    /// <summary>
    /// 現在の残り時間を取得
    /// </summary>
    /// <returns>ゲームの残り時間（秒）</returns>
    public float GetRemainingTime()
    {
        return _currentTime;
    }

    /// <summary>
    /// 時間切れになった時のリザルト画面への移行処理。
    /// </summary>
    private void TransitionToResult()
    {
        if (IsTimeUp) return; // 既に時間切れ処理が呼ばれていたら何もしない

        IsTimeUp = true; // 時間切れフラグを立てる
        Debug.Log("時間切れ！リザルト画面へ移行します。");

        // リザルトシーンへの移行
        if (!string.IsNullOrEmpty(resultSceneName))
        {
            SceneManager.LoadScene(resultSceneName);
        }
        else
        {
            Debug.LogError("リザルトシーン名が設定されていません！Inspectorで 'Result Scene Name' を設定してください。", this);
        }
    }

    /// <summary>
    /// 残り時間をUIテキストに表示します。
    /// </summary>
    private void UpdateTimerUI()
    {
        if (timerTextUI != null)
        {
            // 残り時間を分:秒の形式に変換
            int minutes = Mathf.FloorToInt(_currentTime / 60);
            int seconds = Mathf.FloorToInt(_currentTime % 60);

            // フォーマットして表示
            timerTextUI.text = string.Format("時間:{0:00}:{1:00}", minutes, seconds);
        }
    }
}
