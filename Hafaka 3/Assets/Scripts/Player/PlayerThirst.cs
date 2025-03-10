using Unity.VisualScripting;
using UnityEngine;

public class PlayerThirst : MonoBehaviour
{
    [SerializeField]float MaxThirst;
    [SerializeField]float currentThirst;
    [SerializeField] float ThirstDrainTimer;
    [SerializeField] float WaterLossPrecentage;
    private float lastDrainTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaxThirst = 100;
        currentThirst = MaxThirst;
        ThirstDrainTimer = 10f;
        WaterLossPrecentage = 0.95f;
    }

    // Update is called once per frame
    void Update()
    {   
        if (GameManager.Instance.GetTime - lastDrainTime >= 90f)
        {
            DrainThirst();
            lastDrainTime = GameManager.Instance.GetTime;
        }
    }
    public void DrainThirst()
    {
        currentThirst *= WaterLossPrecentage;
        currentThirst = Mathf.Max(currentThirst, 0);
        Debug.Log("Thirst drained. Current thirst: " + currentThirst);
    }
}
