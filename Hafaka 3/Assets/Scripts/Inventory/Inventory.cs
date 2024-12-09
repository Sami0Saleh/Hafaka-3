using UnityEngine;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventory;
    [SerializeField] private Slot[] _slots;

    public GameObject GetInventory { get => _inventory; }
    

    private void OnEnable()
    {
        Pickup.OnItemPickup += HandleItemPickup;
    }

    private void OnDisable()
    {
        Pickup.OnItemPickup -= HandleItemPickup;
    }


    public void SetInventory()
    {
        _inventory.SetActive(!_inventory.activeInHierarchy);
    }

    public bool TryAddItem(Pickup pickup)
    {
        int amountToAdd = pickup.AmountInStack;
        ItemData itemData = pickup.ItemData;

        // First, try to add to existing slots with the same pickup
        foreach (Slot slot in _slots)
        {
            if (slot.HasItem && slot.IsSameItem(itemData) && !slot.IsFull)
            {
                int stackableAmount = Mathf.Min(slot.RemainingCapacity, amountToAdd);
                slot.AddToStack(stackableAmount);
                amountToAdd -= stackableAmount;

                if (amountToAdd <= 0)
                    return true;
            }
        }

        // Then, try to assign the remaining itemData to a new slot
        foreach (Slot slot in _slots)
        {
            if (!slot.HasItem)
            {
                slot.AssignItem(itemData, amountToAdd);
                return true;
            }
        }

        // If no space is available, notify the player
        Debug.Log("Inventory Full! Cannot pick up pickup.");
        return false;
    }

    private void HandleItemPickup(Pickup pickup)
    {
        // Additional logic if needed when an pickup is picked up
    }
}
