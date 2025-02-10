using UnityEngine;

public class SurvivalManager : MonoBehaviour
{
    public static SurvivalManager Instance;
    private void Awake()
    {
        if (Instance!=null)
        {
            Destroy(this.gameObject);

        }
        else
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
