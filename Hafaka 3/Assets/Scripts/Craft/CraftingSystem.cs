using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private GameObject _craft;
    [SerializeField] private GameObject _inventoryGameObject;

    [SerializeField] private Transform _craftingListParent; // Parent container for crafting list UI
    [SerializeField] private GameObject _craftingBlueprintPrefab; // UI prefab for a blueprint
    [SerializeField] private Inventory _inventory; // Reference to the inventory system
    [SerializeField] private List<BluePrintData> _blueprints; // List of all available blueprints

    private Dictionary<BluePrintData, GameObject> _blueprintUIElements = new Dictionary<BluePrintData, GameObject>();

    private void Start()
    {
        InitializeCraftingUI();
    }

    public void SetCraft()
    {
        _craft.SetActive(!_craft.activeInHierarchy);

        if (_craft.activeInHierarchy)
        {
            UpdateCraftingList();
        }
    }

    private void InitializeCraftingUI()
    {
        foreach (var blueprint in _blueprints)
        {
            var blueprintUI = Instantiate(_craftingBlueprintPrefab, _craftingListParent);
            var craftingUIComponent = blueprintUI.GetComponent<CraftingBlueprintUI>();
            craftingUIComponent.SetBlueprint(blueprint, this);
            _blueprintUIElements.Add(blueprint, blueprintUI);
        }

        UpdateCraftingList();
    }

    public void UpdateCraftingList()
    {
        foreach (var blueprint in _blueprints)
        {
            if (CanCraftBlueprint(blueprint))
            {
                _blueprintUIElements[blueprint].transform.SetAsFirstSibling(); // Move to the top of the list
                _blueprintUIElements[blueprint].GetComponent<CraftingBlueprintUI>().SetCraftable(true);
            }
            else
            {
                _blueprintUIElements[blueprint].GetComponent<CraftingBlueprintUI>().SetCraftable(false);
            }
        }
    }

    public bool CanCraftBlueprint(BluePrintData blueprint)
    {
        int ingCount = 0;
        int required = 0;
       
        foreach (var ingredient in blueprint.RequiredIngerdients)
        {
            ingCount = blueprint.RequiredIngerdients.Count;

            foreach (var item in _inventory.ingerdients)
            {
                if (item.ingerdientType == ingredient.ingerdientType && item.amount >= ingredient.amount)
                {
                    required++;
                }
            }
        }

        if (required == ingCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int CountItemInInventory(string itemName)
    {
        int count = 0;
        foreach (var slot in _inventoryGameObject.GetComponentsInChildren<Slot>())
        {
            if (slot.HasItem && slot.IsSameItem(itemName))
            {
                count += slot.Item.Amount;
            }
        }
        return count;
    }

    public void CraftBlueprint(BluePrintData blueprint)
    {
        if (CanCraftBlueprint(blueprint))
        {
            // Deduct materials from inventory
            RemoveItemFromInventory("Rock", blueprint.RockNeeded);
            RemoveItemFromInventory("Branch", blueprint.BranchNeeded);
            RemoveItemFromInventory("Fiber", blueprint.FiberNeeded);

            // Add the crafted item to the inventory
            _inventory.TryAddItem(new Pickup { ItemData = blueprint.ItemData, AmountInStack = blueprint.AmountInStack });

            Debug.Log($"Crafted {blueprint.BluePrintName}!");
            UpdateCraftingList();
        }
        else
        {
            Debug.Log("Not enough materials to craft this blueprint.");
        }
    }

    private void RemoveItemFromInventory(string itemName, int amountToRemove)
    {
        foreach (var slot in _inventoryGameObject.GetComponentsInChildren<Slot>())
        {
            if (slot.HasItem && slot.IsSameItem(itemName))
            {
                int amountToRemoveFromSlot = Mathf.Min(amountToRemove, slot.Item.Amount);
                slot.Item.RemoveAmount(amountToRemoveFromSlot);
                amountToRemove -= amountToRemoveFromSlot;
                slot.UpdateSlotUI();
                /*if (slot.Item.Amount <= 0)
                {
                    slot.UpdateSlotUI();
                }*/

                if (amountToRemove <= 0)
                    break;
            }
        }
    }
}
