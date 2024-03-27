using Meta.WitAi;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FireManager : MonoBehaviour
{
    [SerializeField] private Transform[] fireSpawnPoints;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private int maxFires = 5;
    [SerializeField, Min(0.1f)] private float spawnRadius = 2;
    public int totalFires = 0;
    Dictionary<int, GameObject> spawnedFires = new();

    public UnityEvent firesExtinguishedEvent = new UnityEvent();

    private bool firesExtinguished = true;

    public FireAlarmController alarmController = null;
    public WinScreenManager winScreenManager = null;

    private float timeSinceGameStart, timeToStartGame;

    public float initialFireDelay = 3f; 
    public float spawnInterval = 10f;

    private float timeSinceLastSpawn;
    private bool initialSpawnComplete = false;
    public bool gameIsStarting = false;

    void Start()
    {
        if (fireSpawnPoints.Length <= 0)
            Debug.LogError($"At Least one SpawnPoint must be set in {nameof(fireSpawnPoints)}"); 
        if( firePrefab == null)
            Debug.LogError($"Prefab must be set for {nameof(firePrefab)}");

        timeSinceLastSpawn = initialFireDelay;
        timeToStartGame = Random.Range(5.0f, 10.0f);
        timeSinceGameStart = 0;

    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Y) || Input.GetKeyDown(KeyCode.T))
        {
            timeSinceGameStart = 0;
            gameIsStarting = true;
        }
        timeSinceGameStart += Time.deltaTime;

        if (!initialSpawnComplete && (timeSinceGameStart >= timeToStartGame) && gameIsStarting)
        {
            if (timeSinceLastSpawn >= initialFireDelay)
            {
                timeSinceGameStart += Time.deltaTime;
                SpawnFires();
                initialSpawnComplete = true;
                gameIsStarting = false;
            }
        }
        else
        {
            if (!firesExtinguished && (timeSinceLastSpawn >= spawnInterval))
            {
                timeSinceGameStart += Time.deltaTime;
                SpawnFires();
            }
        }
        if(initialSpawnComplete)
            timeSinceLastSpawn += Time.deltaTime;
    }

    private void SpawnFires()
    {
        totalFires++;
        if (totalFires > 10)
        {
            firesExtinguished = true;
            firesExtinguishedEvent.Invoke();
            alarmController.StopAlarm();
            winScreenManager.ShowLoseScreen();
            return;
        }
        var spawnpoint = fireSpawnPoints[Random.Range(0,fireSpawnPoints.Length)];
        var amountOfFires = 1;

        for (int i = 0; i < amountOfFires; i++)
        {
            Vector3 pos = RandomCircle(spawnpoint.position, spawnRadius);

            var go = Instantiate(firePrefab, pos, spawnpoint.rotation);
            
            var fireScript = go.GetComponent<Fire>();
            fireScript.fireExtinguishedEvent.AddListener(OnFireExtinguished);
            fireScript.startLifeTime += Random.Range(-0.5f, +1f);
            
            spawnedFires.Add(go.GetInstanceID(), go);
        }
        if(firesExtinguished == true)
            alarmController.StartAlarm();
        spawnInterval = Random.Range(15.0f, 30.0f);
        firesExtinguished = false;
        timeSinceLastSpawn = 0; 
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
        //Debug.Log($"{this.spawnedFires.Serialize()}");
        if (spawnedFires.TryGetValue(GameObjectId, out GameObject go) && go.TryGetComponent(out Fire fire)){
            //Debug.LogWarning("FIRE");
            spawnedFires.Remove(GameObjectId);
            fire.fireExtinguishedEvent.RemoveAllListeners();
            for (int i = go.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(go.transform.GetChild(i).gameObject);
            };
            Object.Destroy(go);
        }

        if(spawnedFires.Count <= 0)
        {
            Debug.LogError("FIRES GONE");
            firesExtinguished = true;
            firesExtinguishedEvent.Invoke();
            alarmController.StopAlarm();
            winScreenManager.ShowWinScreen();
            winScreenManager.winScreen.GetComponentInChildren<TextMeshProUGUI>().text += "\nYour time was: <b>" + timeSinceGameStart.ToString("0.00") + "sec</b>\n\nPress START to restart.";
        }
    }
}