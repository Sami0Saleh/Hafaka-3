using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;
    public Light directionalLight;
    public ParticleSystem fogEffect;

    public float dayDuration = 180f;
    private float nightDuration = 60f;
    private float maxNightDuration = 120f;
    private int cycleCount = 0;

    private float timeCounter = 0f;
    private bool isDay = true;
    public event Action OnDayStart;
    public event Action OnNightStart;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnDayStart?.Invoke();
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;

        if (isDay && timeCounter >= dayDuration)
        {
            SwitchToNight();
        }
        else if (!isDay && timeCounter >= nightDuration)
        {
            SwitchToDay();
            cycleCount++;
            nightDuration = Mathf.Min(nightDuration + 20f, maxNightDuration);
        }
    }

    private void SwitchToNight()
    {
        isDay = false;
        timeCounter = 0f;
        directionalLight.color = Color.blue;
        directionalLight.intensity = 0.3f;
        fogEffect.Play();
        Debug.Log("Night has begun!");
        OnNightStart?.Invoke();
    }

    private void SwitchToDay()
    {
        isDay = true;
        timeCounter = 0f;
        directionalLight.color = Color.yellow;
        directionalLight.intensity = 1f;
        fogEffect.Stop();
        Debug.Log("Day has started!");
        OnDayStart?.Invoke();
    }
}


