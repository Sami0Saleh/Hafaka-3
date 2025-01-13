using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private GameObject _inventory;
    [SerializeField] private Slot[] _slots;

    public GameObject GetInventory { get => _inventory; }

    public List<IngerdientSerializable> ingerdients;
    private IngerdientSerializable _ingerdientType;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        Pickup.OnItemPickup += HandleItemPickup;
        BoxPickup.OnBoxPickup += HandleBoxPickup;
    }

    private void OnDisable()
    {
        Pickup.OnItemPickup -= HandleItemPickup;
        BoxPickup.OnBoxPickup -= HandleBoxPickup;
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

                _ingerdientType.amount += itemData.AmountInStack;


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
                
                _ingerdientType.ingerdientType = itemData.ingerdientType;
                _ingerdientType.amount = itemData.AmountInStack;
                ingerdients.Add(_ingerdientType);

                return true;
            }
        }

        // If no space is available, notify the player
        Debug.Log("Inventory Full! Cannot pick up pickup.");
        return false;
    }

    public bool TryAddBox(BoxPickup boxPickup)
    {
        int amountToAdd = boxPickup.AmountInStack;
        ItemData itemData = boxPickup.itemData;

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
    
    private void HandleBoxPickup(BoxPickup boxPickup)
    {
        // Additional logic if needed when an pickup is picked up
    }
}
