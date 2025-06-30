using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    // シングルトンインスタンス。どこからでも GameTimer.Instance でアクセスできます。
    public static GameTimer Instance { get; private set; }

    [Header("ゲーム時間設定")]
    [Tooltip("ゲームが終了するまでの時間（秒）です。")]
    public float gameDuration = 60.0f; // ゲームの継続時間（デフォルトは60秒）
    private float _currentTime; // 現在の残り時間

    // ゲームが時間切れになったかどうかを示すフラグ
    public bool IsTimeUp { get; private set; } = false;

    [Header("リザルト画面設定")]
    [Tooltip("時間切れになった時に移行するリザルトシーンの名前です。")]
    public string resultSceneName = "ResultSene"; // 移行するリザルトシーン名

    [Header("UI表示設定")]
    [Tooltip("残り時間を表示するTextMeshProUGUIコンポーネントを割り当ててください。")]
    public TextMeshProUGUI timerTextUI; // 残り時間を表示するTextMeshProUGUI

    /// <summary>
    /// シングルトンの初期化。このコンポーネントがアタッチされたオブジェクトが複数ある場合、古い方を破棄します。
    /// </summary>
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
    /// フレームごとにタイマーを更新し、時間切れを判定します。
    /// </summary>
    private void Update()
    {
        // 時間切れになっていない場合のみタイマーを減らす
        if (!IsTimeUp)
        {
            _currentTime -= Time.deltaTime; // リアルタイムで時間を減らす

            // UIを毎フレーム更新
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
    /// 現在の残り時間を取得します。（UI表示などに利用）
    /// </summary>
    /// <returns>ゲームの残り時間（秒）</returns>
    public float GetRemainingTime()
    {
        return _currentTime;
    }

    /// <summary>
    /// 時間切れになった時のリザルト画面への移行処理を実行します。
    /// </summary>
    private void TransitionToResult()
    {
        if (IsTimeUp) return; // 既に時間切れ処理が呼ばれていたら何もしない

        IsTimeUp = true; // 時間切れフラグを立てる
        Debug.Log("時間切れ！リザルト画面へ移行します。");

        // リザルトシーンへの移行
        if (!string.IsNullOrEmpty(resultSceneName))
        {
            SceneManager.LoadScene(resultSceneName); // 指定されたシーン名をロード
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

            // フォーマットして表示 (例: 01:30)
            timerTextUI.text = string.Format("時間:{0:00}:{1:00}", minutes, seconds);
        }
    }
}
