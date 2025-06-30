using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{

    private void Update()
    {
        ScrollBackground();
    }

    private void ScrollBackground()
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
    }
}