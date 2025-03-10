using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;
    public Light directionalLight;
    public ParticleSystem fogEffect;
    [SerializeField] private Material DaySkybox;
    [SerializeField] private Material NightSkybox;


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
        ApplyDaySettings();
        // OnNightStart?.Invoke();
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
        Debug.Log("Night has begun!");
        OnNightStart?.Invoke();
        ApplyNightSettings();
    }

    private void SwitchToDay()
    {
        isDay = true;
        timeCounter = 0f;
        Debug.Log("Day has started!");
        OnDayStart?.Invoke();
        ApplyDaySettings();
    }

    private void ApplyDaySettings()
    {
        directionalLight.color = new Color(0.929f, 0.972f, 1.000f); // 6700K light color
        directionalLight.intensity = 1.5f;
        fogEffect.Stop();
        ChangeSkybox(DaySkybox);
    }

    private void ApplyNightSettings()
    {
        directionalLight.color = Color.black;
        directionalLight.intensity = 1f;
        fogEffect.Play();
        ChangeSkybox(NightSkybox);
    }

    private void ChangeSkybox(Material newSkybox)
    {
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment(); // Refresh global illumination
            Debug.Log($"Skybox changed to: {newSkybox.name}");
        }
        else
        {
            Debug.LogWarning("Skybox material is missing!");
        }
    }
}


