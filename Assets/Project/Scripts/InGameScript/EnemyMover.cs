using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float selfMoveSpeed = 1.0f; // 敵自身の速度（プレイヤーに向かう追加速度）

    void Update()
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
        // Camera.main.ViewportToWorldPoint はワールド座標を返すため、CanvasがWorld Spaceであれば機能します。
        Vector3 screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, Camera.main.nearClipPlane)); // Yは中心で判定
        if (transform.position.x < screenLeft.x - 2f) // 少し画面外に出てから消す
        {
            Destroy(gameObject);
        }
    }

    // Is TriggerがONのCollider2Dと接触したときに呼ばれる
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("敵がプレイヤーに当たった！");
            Destroy(gameObject);
        }
    }
}
