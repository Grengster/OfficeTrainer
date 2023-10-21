using Meta.WitAi;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FireManager : MonoBehaviour
{
    [SerializeField] private Transform[] fireSpawnPoints;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private int maxFires = 5;
    [SerializeField, Min(0.1f)] private float spawnRadius = 2;

    Dictionary<int, GameObject> spawnedFires = new();

    public UnityEvent firesExtinguishedEvent = new UnityEvent();

    private bool firesExtinguished = true;

    void Start()
    {
        if (fireSpawnPoints.Length <= 0)
            Debug.LogError($"At Least one SpawnPoint must be set in {nameof(fireSpawnPoints)}"); 
        if( firePrefab == null)
            Debug.LogError($"Prefab must be set for {nameof(firePrefab)}");

    }

    void Update()
    {
        if (firesExtinguished && Input.GetKeyDown(KeyCode.Space)){
            SpawnFires();
        }
    }

    private void SpawnFires()
    {
        var spawnpoint = fireSpawnPoints[Random.Range(0,fireSpawnPoints.Length)];
        var amountOfFires = Random.Range(1, maxFires);

        for (int i = 0; i < amountOfFires; i++)
        {
            Vector3 pos = RandomCircle(spawnpoint.position, spawnRadius);

            var go = Instantiate(firePrefab, pos, spawnpoint.rotation);
            
            var fireScript = go.GetComponent<Fire>();
            fireScript.fireExtinguishedEvent.AddListener(OnFireExtinguished);
            fireScript.startLifeTime += Random.Range(-0.5f, +1f);
            
            spawnedFires.Add(go.GetInstanceID(), go);
        }
        firesExtinguished = false;
    }

    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y ;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    private void OnFireExtinguished(int GameObjectId)
    {
        Debug.Log($"Fire {GameObjectId} extinguished!");
        Debug.Log($"{this.spawnedFires.Serialize()}");
        if (spawnedFires.TryGetValue(GameObjectId, out GameObject go) && go.TryGetComponent(out Fire fire)){
            Debug.LogWarning("FIRE");
            spawnedFires.Remove(GameObjectId);
            fire.fireExtinguishedEvent.RemoveAllListeners();
            go.DestroySafely();
        }

        if(spawnedFires.Count <= 0)
        {
            Debug.LogError("FIRES GONE");
            firesExtinguished = true;
            firesExtinguishedEvent.Invoke();
        }
    }
}