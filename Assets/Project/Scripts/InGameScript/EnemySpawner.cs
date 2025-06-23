using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 生成する敵のプレハブをInspectorから割り当てる
    public float spawnInterval = 3.0f; // 敵を生成する間隔（秒）
    public float spawnYOffset = 0f; // 敵を生成するY座標のオフセット

    [Header("生成された敵の親オブジェクト")]
    [Tooltip("敵を生成する親となるTransformを割り当ててください。(例: Canvasの子であるInGameObject)")]
    public Transform parentTransform; // ここにInGameObjectのTransformを割り当てる

    private float timer;

    void Start()
    {
        timer = spawnInterval;
        
        if (parentTransform == null)
        {
            Debug.LogWarning("EnemySpawnerに親Transformが設定されていません。このオブジェクト自身が親になります。", this);
            parentTransform = this.transform; 
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        // 画面右端のワールド座標を取得
        Vector3 screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, Camera.main.nearClipPlane)); // Yは画面中央に
        
        // 敵のY座標は、スポーナーのY座標+オフセット
        Vector3 spawnPosition = new Vector3(screenRight.x + 2f, transform.position.y + spawnYOffset, 0); 
        
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Canvasの子として生成する場合、位置をワールド座標で設定した後、親を設定
        newEnemy.transform.SetParent(parentTransform, true); // ★世界座標を維持したまま親を設定する★
                                                             // 'true' は worldPositionStays の引数
    }
}
