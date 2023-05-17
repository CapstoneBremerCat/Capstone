using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game;
namespace Game
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemies = new List<GameObject>();  // 생성된 적들을 담는 리스트    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();  // 생성할 아이템들을 담는 리스트

        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();  // 다양한 적 프리팹을 담는 리스트
        [SerializeField] private Transform[] spawnPoints;   // 적 AI를 소환할 위치들
        [SerializeField] private GameObject spawnParent;   // 적 AI를 소환할 위치들

        // spawnCount만큼 적을 생성
        public void SpawnEnemy(int spawnCount, int wave)
        {
            // spawnCount 만큼 적을 생성
            for (int i = 0; i < spawnCount; i++)
            {
                // 적 생성 및 배치
                GameObject enemyInstance = CreateEnemy(wave);
                SetEnemyPos(enemyInstance);
            }
        }

        // 적 생성
        private GameObject CreateEnemy(int wave)
        {
            GameObject enemyPrefab = enemyPrefabs[0];
            int random = Random.Range(0, 100);
            // 기본 몬스터 등장 확률
            float commonRate = (100.0f - (wave / (wave + 100.0f)) * 100.0f);
            Debug.LogWarning("wave: " + wave + " namedRate: " + commonRate);
            float namedRate = (100 - commonRate) / (enemyPrefabs.Count - 1);
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                if (random < commonRate + namedRate * i)
                {
                    enemyPrefab = enemyPrefabs[i];
                    break;
                }
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
            var newEnemy = Instantiate(enemyPrefab, spawnParent.transform);
            enemies.Add(newEnemy);
            return newEnemy;
        }

        // 적 배치
        private void SetEnemyPos(GameObject enemyInstance)
        {
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemyInstance.transform.position = spawnPoint.position;

            NavMeshAgent navMeshAgent = enemyInstance.GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = true; // NavMeshAgent 활성화

            navMeshAgent.Warp(spawnPoint.position); // NavMesh에 배치된 위치로 순간이동
        }
    }
}