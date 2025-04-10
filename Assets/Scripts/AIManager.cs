using UnityEngine;

public enum EMonsterType
{
    WalkMonster,
    FlyingMonster,
    SkeletonMonster,
    MushroomMonster,
    GoblinMonster,
    BossMonster
}

public class AIManager : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float spawnRangeX = 10.0f;
    public float spawnRangeY = 5.0f;
    public int enemyCount = 5;
    public Transform[] spawnPoints;
    private float monsterSpeed = 1.0f;
    private float monsterHp = 1.0f;
    private float monsterDamage = 1.0f;
    private EMonsterType currentMonsterType = EMonsterType.WalkMonster;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (spawnPoints.Length > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Vector2 spawnPosition = spawnPoints[randomIndex].position;
                Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                float randomX = Random.Range(-spawnRangeX, spawnRangeX);
                float randomY = Random.Range(-spawnRangeY, spawnRangeY);
                Vector2 randomPosition = new Vector2(randomX, randomY);
                Instantiate(monsterPrefab, randomPosition, Quaternion.identity);
            }
        }
    }

    private void MonsterSetState()
    {
        EnemyManager monster = monsterPrefab.GetComponent<EnemyManager>();
        float minSpeed = 1f;
        float maxSpeed = 10f;
        float minHp = 1f;
        float maxHp = 10f;
        float minDamage = 1f;
        float maxDamage = 10f;

        if (currentMonsterType == EMonsterType.WalkMonster)
        {
            minSpeed = 1;
            maxSpeed = 5;
            minHp = 1;
            maxHp = 10;
            minDamage = 1;
            maxDamage = 10;
        }
        else if (currentMonsterType == EMonsterType.SkeletonMonster)
        {
            minSpeed = 0.5f;
            maxSpeed = 3f;
            minHp = 1;
            maxHp = 10;
            minDamage = 1;
            maxDamage = 10;
        }
        else if (currentMonsterType == EMonsterType.FlyingMonster)
        {
            minSpeed = 3.0f;
            maxSpeed = 7.0f;
            minHp = 1;
            maxHp = 10;
            minDamage = 1;
            maxDamage = 10;
        }
        monsterSpeed = Random.Range(minSpeed, maxSpeed);
        monsterHp = Random.Range(minHp, maxHp);
        monsterDamage = Random.Range(minDamage, maxDamage);
        monster.speed = monsterSpeed;
        monster.enemyHp = monsterHp;
        monster.damage = monsterDamage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector2.zero, new Vector2(spawnRangeX * 2, spawnRangeY * 2));
        Gizmos.color = Color.blue;
        if (spawnPoints.Length > 0)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            }
        }
    }
}
