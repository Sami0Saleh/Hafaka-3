using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle instance;
    public Light directionalLight;
    public ParticleSystem fogEffect;
    [SerializeField] private Material DaySkybox;
    [SerializeField] private Material NightSkybox;

    [Header("Transition Settings")]
    [SerializeField] private GameObject twirlEffect; // Full-screen UI Image with Twirl Shader
    [SerializeField] private float transitionDuration = 2f;

    public float dayDuration = 180f;
    private float nightDuration = 60f;
    private float maxNightDuration = 120f;
    private int cycleCount = 0;

    private float timeCounter = 0f;
    private bool isDay = true;
    public event System.Action OnDayStart;
    public event System.Action OnNightStart;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        OnDayStart?.Invoke();
        ApplyDaySettings();
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;

        if (isDay && timeCounter >= dayDuration)
        {
            StartCoroutine(TransitionToNight());
        }
        else if (!isDay && timeCounter >= nightDuration)
        {
            StartCoroutine(TransitionToDay());
            cycleCount++;
            nightDuration = Mathf.Min(nightDuration + 20f, maxNightDuration);
        }
    }

    private IEnumerator TransitionToNight()
    {
        isDay = false;
        timeCounter = 0f;
        Debug.Log("Starting transition to night...");

        yield return StartCoroutine(PlayTwirlEffect());
        ApplyNightSettings();
    }

    private IEnumerator TransitionToDay()
    {
        isDay = true;
        timeCounter = 0f;
        Debug.Log("Starting transition to day...");

        yield return StartCoroutine(PlayTwirlEffect());
        ApplyDaySettings();
    }

    private IEnumerator PlayTwirlEffect()
    {
        twirlEffect.SetActive(true);
        float elapsedTime = 0f;

        // Store initial and target values
        Color initialLightColor = directionalLight.color;
        float initialLightIntensity = directionalLight.intensity;
        Color targetLightColor = isDay ? new Color(0.929f, 0.972f, 1.000f) : Color.black;
        float targetLightIntensity = isDay ? 1.5f : 1f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            directionalLight.color = Color.Lerp(initialLightColor, targetLightColor, t);
            directionalLight.intensity = Mathf.Lerp(initialLightIntensity, targetLightIntensity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set
        directionalLight.color = targetLightColor;
        directionalLight.intensity = targetLightIntensity;

        twirlEffect.SetActive(false);
    }

    private void ApplyDaySettings()
    {
        Debug.Log("Day settings applied.");
        fogEffect.Stop();
        ChangeSkybox(DaySkybox);
    }

    private void ApplyNightSettings()
    {
        Debug.Log("Night settings applied.");
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
