using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class BoxPickup : MonoBehaviour, IInteractable
{
    public static event System.Action<BoxPickup> OnBoxPickup;

    [SerializeField] TextMeshPro m_TextMeshProUGUI;
    [SerializeField] GameObject _boxDes;
    [SerializeField] Image _boxIcon;
    [SerializeField] TMP_Text _boxText;
    [SerializeField] Vector3 _offset;

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
        _boxDes.transform.position = transform.position + _offset;
        _boxIcon.sprite = ItemData.ItemSprite;
        _boxText.text = $"Item: {ItemData.ItemName}\n" +
            $"Amount: {AmountInStack}";
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
        if (m_TextMeshProUGUI.gameObject.activeSelf && _boxDes.gameObject.activeSelf && player != null)
        {

            Vector3 dir = m_TextMeshProUGUI.transform.position - player.Eyes.transform.position;
            Vector3 boxDir = _boxDes.transform.position - player.Eyes.transform.position;
            // make sure the direction points at the EYES of the player rather then his genitals

            m_TextMeshProUGUI.transform.rotation = Quaternion.LookRotation(dir);
            _boxDes.transform.rotation = Quaternion.LookRotation(boxDir);
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
                _boxDes.SetActive(true);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            isPlayerNearby = false;
            m_TextMeshProUGUI.enabled = false;
            _boxDes.SetActive(false);
        }
    }
    public bool IsPlayerNearby() => isPlayerNearby;
}
