using UnityEngine;

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

    void Awake()
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

    void Update()
    {
        CurrentGameScrollSpeed = baseScrollSpeed + (_playerCurrentMoveSpeed * playerSpeedInfluence);
    }

    public void SetPlayerCurrentMoveSpeed(float speed)
    {
        _playerCurrentMoveSpeed = speed;
    }
}
