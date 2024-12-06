using UnityEngine;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject _inventory;
    [SerializeField] Slot[] _slots;

    private void Update()
    {
        
    }

    public void SetInventory()
    {
        _inventory.SetActive(!_inventory.activeInHierarchy);
    }

    public void AddItem(Item itemToAdd, Item startingItem = null)
    {
        foreach (Slot i in _slots)
        {
            if (!i.SlotItem)
            {
                itemToAdd.transform.parent = i.transform;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Item>())
            AddItem(col.GetComponent<Item>());
        
    }
}
