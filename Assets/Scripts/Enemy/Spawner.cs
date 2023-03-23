using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemies = new List<GameObject>();  // 생성된 적들을 담는 리스트    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();  // 생성할 아이템들을 담는 리스트

        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();  // 다양한 적 프리팹을 담는 리스트
        [SerializeField] private Transform[] spawnPoints;   // 적 AI를 소환할 위치들

        // spawnCount만큼 적을 생성
        public void SpawnEnemy(int spawnCount, int wave)
        {
            // spawnCount 만큼 적을 생성
            for (int i = 0; i < spawnCount; i++)
            {
                // 적 생성 및 배치
                SetEnemyPos(CreateEnemy(wave));
            }
        }

        // 적 생성
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
                wave *= 2;  // wave 값을 2배씩 높임
            }
            // 풀 안에 비활성화된 적이 있으면 재활용
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

        // 적 배치
        private void SetEnemyPos(GameObject enemyPrefab)
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemyPrefab.transform.position = spawnPoint.position;
        }
    }
}