using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
   public float lifetime = 0.5f; // エフェクトが表示される時間（秒）

    void Start()
    {
        // 指定した時間後にGameObjectを破棄する
        Destroy(gameObject, lifetime);
    }
}
