using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;

public class Fire : MonoBehaviour
{
    [SerializeField, Range(0,1f)] private float currentIntensity = 1f;
    [SerializeField] private ParticleSystem firePS = null;

    public float startIntensity = 5f;
    public float startLifeTime = 5f;

    private float timeLastWatered = 0f;
    [SerializeField] private float regenDelay = 1f;
    [SerializeField] private float regenRate = 1f;
    private bool fireExtinguished = false;


    public UnityEvent<int> fireExtinguishedEvent = new();

    private void Start()
    {
        Debug.Log($"Instanciated {this.gameObject.GetInstanceID()}");
        startIntensity = firePS.emission.rateOverTime.constant;
        startLifeTime = firePS.main.startLifetime.constant;
    }

    private void Update()
    {
        if (currentIntensity > 0 && currentIntensity < 1.0f && Time.time - timeLastWatered >= regenDelay)
        {
            currentIntensity += regenRate * Time.deltaTime;
            ChangeIntensity();
        }

    }

    private void ChangeIntensity()
    {
        var emission = firePS.emission;
        var psMain = firePS.main;
        psMain.startLifetime = Mathf.Max(0.2f,currentIntensity) * startLifeTime;
        emission.rateOverTime = currentIntensity * startIntensity;
    }

    /// <summary>
    /// Changes the Current Fire Intensity by the amount, and returns True if fire is extinguished
    /// </summary>
    /// <param name="amountPerSecond"></param>
    /// <returns></returns>
    public void TryExtinguish(float amountPerSecond)
    {
        timeLastWatered = Time.time;
        currentIntensity -= amountPerSecond;
        ChangeIntensity();

        if (currentIntensity <= 0 && fireExtinguished == false)
        {
            Debug.Log("FireExtinguishedInvoke");
            this.fireExtinguishedEvent.Invoke(this.gameObject.GetInstanceID());
            fireExtinguished = true;
        }
    }
}
