using System.Collections;
using UnityEngine;

public class DestroyBox : MonoBehaviour
{
    [SerializeField] private float _time;
    void Start()
    {
        StartCoroutine(DestroyDelay(_time));
    }

    public IEnumerator DestroyDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Destroy(gameObject);
    }
}
