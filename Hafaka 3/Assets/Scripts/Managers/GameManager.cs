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
   

    void Update()
    {
        time += Time.time;
    }
}
