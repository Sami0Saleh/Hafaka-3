using UnityEngine;

public class CycleBehavior : MonoBehaviour
{
    [SerializeField] Material DayMat;
    [SerializeField] Material NightMat;
    MeshRenderer meshRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Start()
    {
        if (DayNightCycle.instance == null)
        {
            Debug.LogError("DayNightCycle instance is missing!");
            return;
        }
        
        DayNightCycle.instance.OnDayStart += On_DayChange;
        DayNightCycle.instance.OnNightStart += On_NightChange;
    }
    
    private void OnDestroy()
    {
        if (DayNightCycle.instance != null)
        {
            DayNightCycle.instance.OnDayStart -= On_DayChange;
            DayNightCycle.instance.OnNightStart -= On_NightChange;
        }
    }
    
    void On_DayChange()
    {
        if (DayMat != null)
        {
            meshRenderer.material = new Material(DayMat);
            // Debug.Log($"{gameObject.name} switched to Day Material.");
        }
    }
    void On_NightChange()
    {
        if (NightMat != null)
        {
            meshRenderer.material = new Material(NightMat);
            // Debug.Log($"{gameObject.name} switched to Night Material.");
        }
    }
}
