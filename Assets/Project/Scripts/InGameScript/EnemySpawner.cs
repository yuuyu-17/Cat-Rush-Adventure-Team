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
        Vector3 screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, Camera.main.nearClipPlane)); 
        
        // 敵のY座標は、スポーナーのY座標+オフセット
        Vector3 spawnPosition = new Vector3(screenRight.x + 2f, transform.position.y + spawnYOffset, 0);
        
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // 敵自身をinGameObjectParentの子として設定
        newEnemy.transform.SetParent(parentTransform, true);

        // 生成された敵のEnemyMoverコンポーネントを取得し、エフェクトの親を設定
        EnemyMover enemyMover = newEnemy.GetComponent<EnemyMover>();
        if (enemyMover != null)
        {
            enemyMover.SetEffectParent(parentTransform); // EnemyMoverに親Transformを渡す
        }
        else
        {
            Debug.LogWarning("生成された敵プレハブにEnemyMoverコンポーネントが見つかりません。", newEnemy);
        }
    }
}
