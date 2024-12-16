using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoxPickup : MonoBehaviour, IInteractable
{
    public static event System.Action<BoxPickup> OnBoxPickup;

    [SerializeField] TextMeshPro m_TextMeshProUGUI;

    private PlayerHealth player;
    private Inventory inventory;

    private bool isPlayerNearby;

    public ItemData ItemData;
    public int AmountInStack;

    private void Start()
    {
        m_TextMeshProUGUI.enabled = false;
        m_TextMeshProUGUI.text = $"Press E to pick up the {ItemData.name}";
        m_TextMeshProUGUI.fontSize = 1f;
    }
    public void Interact()
    {
        if (inventory != null && inventory.TryAddBox(this))
        {
            OnBoxPickup?.Invoke(this);
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (m_TextMeshProUGUI.gameObject.activeSelf && player != null)
        {

            Vector3 dir = m_TextMeshProUGUI.transform.position - player.Eyes.transform.position;
            // make sure the direction points at the EYES of the player rather then his genitals

            m_TextMeshProUGUI.transform.rotation = Quaternion.LookRotation(dir);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (other.TryGetComponent(out player))
            {
                inventory = other.GetComponent<Inventory>();
                isPlayerNearby = true;
                m_TextMeshProUGUI.enabled = true;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            isPlayerNearby = false;
            m_TextMeshProUGUI.enabled = false;
        }
    }
    public bool IsPlayerNearby() => isPlayerNearby;
}
