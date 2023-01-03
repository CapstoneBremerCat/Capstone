using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private readonly List<Enemy> enemies = new List<Enemy>();  // ������ ������ ��� ����Ʈ

    [SerializeField] private Enemy enemyPrefab;     // ������ �� AI

    [SerializeField] private Transform[] spawnPoints;   // �� AI�� ��ȯ�� ��ġ��

    private int wave;   // ���� ���̺�

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUI()
    {

    }

    // ���� ���̺꿡 ���� ���� ����
    private void SpawnWave()
    {
        // ���̺� 1 ����
        wave++;

        // �� ���� �� (�ӽ�)
        var spawnCount = Mathf.RoundToInt(wave * 5f);

        // spawnCount ��ŭ ���� ����
        for (int i=0;i< spawnCount;i++)
        {
            // �� ���� ó��
            CreateEnemy();
        }
    }

    // ���� ����
    private void CreateEnemy()
    {
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        enemies.Add(enemy);

/*        // ����� ���� ����Ʈ���� ����
        enemy.OnDeath += () => enemies.Remove(enemy);
        // ����� ���� 10 �� �ڿ� �ı�
        enemy.OnDeath += () => Destroy(enemy.gameObject, 10f);
        enemy.OnDeath += () => enemies.Remove(enemy);*/
    }
}
