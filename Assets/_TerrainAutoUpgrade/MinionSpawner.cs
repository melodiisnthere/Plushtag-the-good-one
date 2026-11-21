using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class MinionSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject minionPrefab;
    public Transform player;
    public float spawnInterval = 2f;
    public int maxAliveMinions = 5;
    public int totalSpawnLimit = 20;

    [Header("Kill Settings")]
    public string killTag = "Bullet";

    private List<GameObject> activeMinions = new List<GameObject>();
    private int totalSpawned = 0;
    private bool spawning = true;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned in MinionSpawner!");
            enabled = false;
            return;
        }
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Clean up any null (destroyed) minions
            activeMinions.RemoveAll(m => m == null);

            if (activeMinions.Count < maxAliveMinions && totalSpawned < totalSpawnLimit)
            {
                SpawnMinion();
            }

            if (totalSpawned >= totalSpawnLimit && activeMinions.Count == 0)
            {
                spawning = false; // Stop after all minions are gone
            }
        }
    }

    void SpawnMinion()
    {
        GameObject minion = Instantiate(minionPrefab, transform.position, Quaternion.identity);
        activeMinions.Add(minion);
        totalSpawned++;

        MinionAI ai = minion.AddComponent<MinionAI>();
        ai.Initialize(player, killTag);
    }
}

public class MinionAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private string killTag;

    public void Initialize(Transform playerTarget, string killTagName)
    {
        player = playerTarget;
        killTag = killTagName;
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("Minion prefab missing NavMeshAgent!");
        }
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(killTag))
        {
            Destroy(gameObject);
        }
    }
}
