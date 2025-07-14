using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Button FlyButton;

    // ★★★ 追加する変数 ★★★
    [Header("ジャンプ設定")]
    [SerializeField] private float jumpForce = 500f; // ジャンプ力
    [SerializeField] private Rigidbody2D rb; // プレイヤーのRigidbody2Dへの参照

    private void Start()
    {
        // Rigidbody2Dコンポーネントを取得（Playerオブジェクトにアタッチされているはず）
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("PlayerController: Rigidbody2DコンポーネントがPlayerオブジェクトに見つかりません！", this);
            }
        }

        // FlyButtonが設定されていれば、クリックイベントリスナーを追加
        if (FlyButton != null)
        {
            FlyButton.onClick.AddListener(Jump); // ボタンがクリックされたらJumpメソッドを呼び出す
        }
        else
        {
            Debug.LogWarning("PlayerController: FlyButtonが割り当てられていません。ボタンでのジャンプは機能しません。", this);
        }
    }

    // ★★★ ジャンプ処理のメソッド ★★★
    private void Jump()
    {
        if (rb != null)
        {
            // 現在のY軸速度をリセットして、常に同じジャンプ力を適用するようにする
            // これにより、連打してもジャンプが高くなりすぎないようにする
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // 真上（Y軸方向）に力を加える
            rb.AddForce(Vector2.up * jumpForce);
            Debug.Log("ジャンプ！");
        }
    }
}
