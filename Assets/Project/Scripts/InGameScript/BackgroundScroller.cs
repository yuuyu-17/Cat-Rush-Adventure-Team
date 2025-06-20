using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 1.0f; // 背景のスクロール速度
    public float backgroundWidth; // 背景画像の幅

    private void Update()
    {
        BackGroundScroll();
    }

    private void BackGroundScroll()
    {
        // 背景を左に移動させる
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // 背景が画面の左端より外に出たら、右端に移動してループさせる
        if (transform.position.x < -backgroundWidth)
        {
            transform.Translate(Vector3.right * backgroundWidth * 2);
        }
    }
}