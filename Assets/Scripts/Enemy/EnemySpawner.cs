using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private readonly List<Enemy> enemies = new List<Enemy>();  // 생성된 적들을 담는 리스트

    [SerializeField] private Enemy enemyPrefab;     // 생성할 적 AI

    [SerializeField] private Transform[] spawnPoints;   // 적 AI를 소환할 위치들

    private int wave;   // 현재 웨이브

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUI()
    {

    }

    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave()
    {
        // 웨이브 1 증가
        wave++;

        // 적 생성 수 (임시)
        var spawnCount = Mathf.RoundToInt(wave * 5f);

        // spawnCount 만큼 적을 생성
        for (int i=0;i< spawnCount;i++)
        {
            // 적 생성 처리
            CreateEnemy();
        }
    }

    // 적을 생성
    private void CreateEnemy()
    {
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        enemies.Add(enemy);

/*        // 사망한 적을 리스트에서 제거
        enemy.OnDeath += () => enemies.Remove(enemy);
        // 사망한 적을 10 초 뒤에 파괴
        enemy.OnDeath += () => Destroy(enemy.gameObject, 10f);
        enemy.OnDeath += () => enemies.Remove(enemy);*/
    }
}
