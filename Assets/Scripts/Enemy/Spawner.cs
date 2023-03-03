using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();  // ������ ������ ��� ����Ʈ    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();  // ������ �����۵��� ��� ����Ʈ

    [SerializeField] private GameObject enemyPrefab;     // ������ �� AI
    [SerializeField] private Transform[] spawnPoints;   // �� AI�� ��ȯ�� ��ġ��

    // spawnCount��ŭ ���� ����
    public void SpawnEnemy(int spawnCount)
    {
        // spawnCount ��ŭ ���� ����
        for (int i=0;i< spawnCount; i++)
        {
            // �� ���� �� ��ġ
            SetEnemyPos(CreateEnemy(enemyPrefab));
        }
    }

    // �� ����
    private GameObject CreateEnemy(GameObject enemyPrefab)
    {
        // Ǯ �ȿ� ��Ȱ��ȭ�� ���� ������ ��Ȱ��
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

    // �� ��ġ
    private void SetEnemyPos(GameObject enemyPrefab)
    {
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        enemyPrefab.transform.position = spawnPoint.position;
    }
}
