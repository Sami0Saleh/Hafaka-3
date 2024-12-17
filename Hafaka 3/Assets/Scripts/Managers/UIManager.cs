using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Inventory _inventory;
    [SerializeField] CraftingSystem _craftingSystem;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    
    public void OpenInventoryAndCraft()
    {
        _inventory.SetInventory();
        _craftingSystem.SetCraft();
    }
}
