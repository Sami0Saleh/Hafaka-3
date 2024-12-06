using StarterAssets;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour,IInteractable
{
    [SerializeField] TextMeshPro m_TextMeshProUGUI;
    private GameObject player;
    
    private void Start()
    {
        m_TextMeshProUGUI.gameObject.SetActive(false);
      
    }
    public void Interact()  
    {
        throw new System.NotImplementedException();
    }
    private void Update()
    {
        if (m_TextMeshProUGUI.gameObject.activeSelf && player !=  null)
        {
            Vector3 dir = m_TextMeshProUGUI.transform.position - player.transform.position;

            m_TextMeshProUGUI.transform.rotation= Quaternion.LookRotation(dir);
        }
           
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            player = other.gameObject;
            m_TextMeshProUGUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            m_TextMeshProUGUI.gameObject.SetActive(false);
        }
    }
}
