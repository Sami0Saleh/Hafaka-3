using StarterAssets;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour,IInteractable
{
    [SerializeField] TextMeshPro m_TextMeshProUGUI;
    PlayerHealth playerHealth = null;
    
    private void Start()
    {
        m_TextMeshProUGUI.enabled = false;
     
    }
    public void Interact()  
    {
        throw new System.NotImplementedException();
    }
    private void Update()
    {
        if (m_TextMeshProUGUI.gameObject.activeSelf && playerHealth !=  null)
        {
            Vector3 dir = m_TextMeshProUGUI.transform.position - playerHealth.Eyes.transform.position;
            // make sure the direction points at the EYES of the player rather then his genitals

            m_TextMeshProUGUI.transform.rotation= Quaternion.LookRotation(dir);
        }
           
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
           other.TryGetComponent(out playerHealth);
            m_TextMeshProUGUI.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            m_TextMeshProUGUI.enabled = false;

        }
    }
}
