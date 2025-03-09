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
        DayNightCycle.instance.OnDayStart += On_DayChange;
        DayNightCycle.instance.OnNightStart += On_NightChange;
    }
    void On_DayChange()
    {
        if (DayMat == null)
        {
            return;
        }
        meshRenderer.material = DayMat;
    }
    void On_NightChange()
    {
        if (NightMat == null)
        {
            return;
        }
        meshRenderer.material = NightMat;
    }
}
