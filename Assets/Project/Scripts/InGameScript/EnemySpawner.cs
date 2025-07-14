using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成する敵のプレハブ")]
    [Tooltip("生成する敵のプレハブのリストをInspectorから割り当ててください。")]
    [SerializeField] private List<GameObject> enemyPrefabs;

    [Header("敵ごとの出現Y座標")]
    [Tooltip("enemyPrefabsの各要素に対応する出現Y座標を設定してください。")]
    [SerializeField] private List<float> enemySpawnYPositions;

    public float spawnInterval = 3.0f; // 敵を生成する間隔（秒）

    [Header("生成された敵の親オブジェクト")]
    [Tooltip("敵を生成する親となるTransformを割り当ててください。")]
    [SerializeField] private Transform parentTransform;

    private float timer;

    private void Start()
    {
        timer = spawnInterval;
        
        if (parentTransform == null)
        {
            Debug.LogWarning("EnemySpawnerに親Transformが設定されていません。このオブジェクト自身が親になります。", this);
            parentTransform = this.transform;
        }

        // プレハブリストが空でないことを確認
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("EnemySpawner: 敵のプレハブが割り当てられていません！", this);
            enabled = false; // スポナーを無効にする
            return;
        }

        if (enemyPrefabs.Count != enemySpawnYPositions.Count)
        {
            Debug.LogError("EnemySpawner: enemyPrefabs と enemySpawnYPositions のリストの数が一致しません！正しく設定してください。", this);
            enabled = false; // スポナーを無効にする
            return; // 処理を終了
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        // リストが空または数が一致しない場合は何もしない
        if (enemyPrefabs == null || enemyPrefabs.Count == 0 || enemyPrefabs.Count != enemySpawnYPositions.Count) return;

        //ランダムなインデックスを選択
        int randomIndex = Random.Range(0, enemyPrefabs.Count);

        GameObject selectedEnemyPrefab = enemyPrefabs[randomIndex];
        if (selectedEnemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: リストにnullの敵プレハブが含まれています。スキップします。", this);
            return;
        }

        //選択された敵に対応するY座標を取得
        float targetSpawnY = enemySpawnYPositions[randomIndex];

        // 画面右端のワールド座標を取得
        Vector3 screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, Camera.main.nearClipPlane));

        //取得したY座標をそのまま使用（敵の中心がそのY座標になる
        // EnemySpawnerオブジェクトのY座標は0のままでOK
        Vector3 spawnPosition = new Vector3(screenRight.x + 2f, targetSpawnY, 0);
        
        GameObject newEnemy = Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);

        // 敵自身をinGameObjectParentの子として設定
        newEnemy.transform.SetParent(parentTransform, true);

        // 生成された敵のEnemyMoverコンポーネントを取得し、エフェクトの親を設定
        EnemyMover enemyMover = newEnemy.GetComponent<EnemyMover>();
        if (enemyMover != null)
        {
            enemyMover.SetEffectParent(parentTransform);
        }
        else
        {
            Debug.LogWarning("生成された敵プレハブにEnemyMoverコンポーネントが見つかりません。", newEnemy);
        }
    }
}
