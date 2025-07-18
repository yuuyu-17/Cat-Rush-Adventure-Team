using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float selfMoveSpeed = 1.0f; // 敵自身の速度

    [Header("エフェクト設定")]
    [SerializeField] private GameObject slashEffectPrefab; // 斬撃エフェクトのプレハブをInspectorから割り当てる
    [SerializeField] private Vector3 effectOffset = new Vector3(0, 0, -0.1f); // エフェクトの位置調整（Zを少し手前にすると重なりやすい）
    private Transform _effectParentTransform; //この変数は外部（EnemySpawner）から設定されるため、privateにする

    [Header("報酬設定")]
    [SerializeField] private int coinsOnDefeat = 10; // 倒した時に獲得するコインの数
    [SerializeField] private ItemType itemOnDefeat = ItemType.Coin; // 倒した時に獲得するアイテムの種類
    [SerializeField] private int itemCountOnDefeat = 1; // 獲得するアイテムの数

    //エフェクトの親Transformを設定するための公開メソッド
    public void SetEffectParent(Transform parent)
    {
        _effectParentTransform = parent;
    }

    private void Update()
    {
        // InGameManagerから現在のゲーム全体のスクロール速度を取得
        float gameScrollSpeed = 0f;
        if (InGameManager.Instance != null)
        {
            gameScrollSpeed = InGameManager.Instance.CurrentGameScrollSpeed;
        }
        else
        {
            Debug.LogWarning("InGameManagerがシーンに見つかりません。EnemyMoverはデフォルトの速度で動作します。");
            gameScrollSpeed = 1.0f; // デフォルトの速度
        }

        // 敵の合計移動速度を計算し、左に移動
        float totalMovement = (gameScrollSpeed + selfMoveSpeed) * Time.deltaTime;
        transform.Translate(Vector3.left * totalMovement);

        // 画面外に出たら消滅させる
        Vector3 screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, Camera.main.nearClipPlane)); // Yは中心で判定
        if (transform.position.x < screenLeft.x - 2f)
        {
            Destroy(gameObject);
        }
    }

    //プレイヤーに当たったら
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("敵がプレイヤーに当たった！");

            //報酬獲得処理
            if (InGameManager.Instance != null)
            {
                InGameManager.Instance.AddCoins(coinsOnDefeat); // コインを追加
                if (itemOnDefeat != ItemType.None)
                {
                    InGameManager.Instance.AddItem(itemOnDefeat, itemCountOnDefeat); // アイテムを追加
                }
            }
            else
            {
                Debug.LogWarning("InGameManagerが見つかりません。報酬を獲得できませんでした。");
            }

            // 斬撃エフェクトを生成
            if (slashEffectPrefab != null)
            {
                Vector3 effectSpawnPosition = transform.position + effectOffset;
                GameObject newEffect = Instantiate(slashEffectPrefab, effectSpawnPosition, Quaternion.identity);

                //_effectParentTransformが設定されていれば親にする
                if (_effectParentTransform != null)
                {
                    newEffect.transform.SetParent(_effectParentTransform, true);
                }
                else
                {
                    Debug.LogWarning("エフェクトの親Transformが設定されていません。エフェクトはルートに生成されます。", this);
                }
            }

            // 敵自身を消滅させる
            Destroy(gameObject);
        }
    }
}
