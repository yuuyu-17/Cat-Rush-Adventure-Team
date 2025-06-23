using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float backgroundWidth; // 背景画像の幅

    private void Update()
    {
        BackGroundScroll();
    }

    private void BackGroundScroll()
    {
        // InGameManagerから現在のスクロール速度を取得
        float currentScrollSpeed = 0f;
        if (InGameManager.Instance != null)
        {
            currentScrollSpeed = InGameManager.Instance.CurrentGameScrollSpeed;
        }
        else
        {
            Debug.LogWarning("InGameManagerがシーンに見つかりません。BackgroundScrollerはデフォルトの速度で動作します。");
            currentScrollSpeed = 1.0f; // InGameManagerが見つからない場合、デフォルトの速度を設定
        }

        // 背景を左に移動させる
        transform.Translate(Vector3.left * currentScrollSpeed * Time.deltaTime);

        // 背景が画面の左端より外に出たら、右端に移動してループさせる
        if (transform.position.x < -backgroundWidth)
        {
            transform.Translate(Vector3.right * backgroundWidth * 2);
        }
    }
}