using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAlarmController : MonoBehaviour
{
    public List<Light> alarmLights;
    public AudioSource alarmSound; 

    public float blinkSpeed = 2.0f; 
    public float alarmVolume = 0.02f; 

    private bool isAlarmOn = false;

    void Start()
    {
        foreach (var light in alarmLights)
        {
            light.intensity = 1; 
            light.color = Color.white;
        }
        alarmSound.volume = 0; 
    }

    void Update()
    {
       
    }

    public void StartAlarm()
    {
        isAlarmOn = true;
        alarmSound.volume = 0.02f;
        alarmSound.Play();
        StartCoroutine(BlinkAlarmLight());
    }

    public void StopAlarm()
    {
        isAlarmOn = false;
        alarmSound.Stop();
        alarmSound.volume = 0;
        foreach (var light in alarmLights)
        {
            light.intensity = 1;
            light.color = Color.white;
        }
    }

    IEnumerator BlinkAlarmLight()
    {
        while (isAlarmOn)
        {
            foreach (var light in alarmLights)
            {
                light.intensity = 3;
                light.color = Color.red;
            }
            yield return new WaitForSeconds(1 / blinkSpeed);
            foreach (var light in alarmLights)
            {
                light.intensity = 0; 
            }
            yield return new WaitForSeconds(1 / blinkSpeed);
        }
    }
}
