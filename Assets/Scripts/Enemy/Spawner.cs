using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();  // 생성된 적들을 담는 리스트    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();  // 생성할 아이템들을 담는 리스트

    [SerializeField] private GameObject enemyPrefab;     // 생성할 적 AI
    [SerializeField] private Transform[] spawnPoints;   // 적 AI를 소환할 위치들

    // spawnCount만큼 적을 생성
    public void SpawnEnemy(int spawnCount)
    {
        // spawnCount 만큼 적을 생성
        for (int i=0;i< spawnCount; i++)
        {
            // 적 생성 및 배치
            SetEnemyPos(CreateEnemy(enemyPrefab));
        }
    }

    // 적 생성
    private GameObject CreateEnemy(GameObject enemyPrefab)
    {
        // 풀 안에 비활성화된 적이 있으면 재활용
        foreach (GameObject enemy in enemies)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }
        // If all objects in the pool are active, create a new one
        var newEnemy = Instantiate(enemyPrefab);
        enemies.Add(newEnemy);
        return newEnemy;
    }

    // 적 배치
    private void SetEnemyPos(GameObject enemyPrefab)
    {
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        enemyPrefab.transform.position = spawnPoint.position;
    }
}
