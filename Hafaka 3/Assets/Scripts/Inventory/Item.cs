using NUnit.Framework.Interfaces;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
    public static event System.Action<Item> OnItemPickup;

    [SerializeField] private ItemData _itemData;

    // Local stack amount for this instance of the item
    [SerializeField] private int _amountInStack;

    public ItemData ItemData => _itemData;
    public int AmountInStack => _amountInStack;

    private void Awake()
    {
        // Initialize the local stack amount from the scriptable object
        _amountInStack = _itemData.AmountInStack;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && inventory.TryAddItem(this))
            {
                OnItemPickup?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
