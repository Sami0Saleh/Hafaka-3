using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float GetTime { get { return time; } set { time = value; } }
    [SerializeField]float time;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(instance.gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.time;
      
       
    }
}
