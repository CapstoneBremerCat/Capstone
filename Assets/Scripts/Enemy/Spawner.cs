using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();  // ������ ������ ��� ����Ʈ

    [SerializeField] private Enemy enemyPrefab;     // ������ �� AI

    [SerializeField] private Transform[] spawnPoints;   // �� AI�� ��ȯ�� ��ġ��

    // spawnCount��ŭ ���� ����
    public void SpawnEnemy(int spawnCount)
    {
        // spawnCount ��ŭ ���� ����
        for (int i=0;i< spawnCount; i++)
        {
            // �� ���� ó��
            CreateEnemy(enemyPrefab);
        }
    }

    // ���� ����
    private void CreateEnemy(Enemy enemyPrefab)
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
