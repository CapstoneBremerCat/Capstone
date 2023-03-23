using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemies = new List<GameObject>();  // ������ ������ ��� ����Ʈ    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();  // ������ �����۵��� ��� ����Ʈ

        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();  // �پ��� �� �������� ��� ����Ʈ
        [SerializeField] private Transform[] spawnPoints;   // �� AI�� ��ȯ�� ��ġ��

        // spawnCount��ŭ ���� ����
        public void SpawnEnemy(int spawnCount, int wave)
        {
            // spawnCount ��ŭ ���� ����
            for (int i = 0; i < spawnCount; i++)
            {
                // �� ���� �� ��ġ
                SetEnemyPos(CreateEnemy(wave));
            }
        }

        // �� ����
        private GameObject CreateEnemy(int wave)
        {
            GameObject enemyPrefab = enemyPrefabs[0];
            int random = Random.Range(0, 100);
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                if (random < 100 - ((wave * 10 / (wave * 10 + 100)) * 100))
                {
                    Debug.LogWarning(100 - ((wave * 10 / (wave * 10 + 100)) * 100));
                    enemyPrefab = enemyPrefabs[i];
                    break;
                }
                wave *= 2;  // wave ���� 2�辿 ����
            }
            // Ǯ �ȿ� ��Ȱ��ȭ�� ���� ������ ��Ȱ��
            foreach (GameObject enemy in enemies)
            {
                if (enemy.activeInHierarchy == false && enemy.name.Equals(enemyPrefab.name + "(Clone)"))
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
}