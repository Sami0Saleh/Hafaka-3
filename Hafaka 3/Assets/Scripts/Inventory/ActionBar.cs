using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ActionBar : MonoBehaviour
{
    [SerializeField] private Slot[] _actionBarSlots;  // Reference to the 9 action bar slots
    [SerializeField] private KeyCode[] _keyBindings = 
        {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
        KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
        };

    private int _currentSlotIndex = -1;

    [SerializeField] private Image[] actionBarImage; // UI images for the action bar slots
    [SerializeField] private TMP_Text[] _amountText;

    private void Update()
    {
        for (int i = 0; i < _keyBindings.Length; i++)
        {
            if (Input.GetKeyDown(_keyBindings[i]))
            {
                EquipItemInSlot(i);
                break;
            }
        }
    }

    public void AssignItemToActionBar(Slot inventorySlot, int actionBarIndex)
    {
        if (inventorySlot.HasItem && actionBarIndex >= 0 && actionBarIndex < _actionBarSlots.Length)
        {
            Slot actionSlot = _actionBarSlots[actionBarIndex];
            if (!actionSlot.HasItem)
            {
                actionSlot.AssignItem(inventorySlot.Item.Data, inventorySlot.Item.Amount, inventorySlot.Item.IngerdientType);
                _amountText[actionBarIndex].text = inventorySlot.Item.Amount.ToString();
            }
        }
    }

    private void EquipItemInSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _actionBarSlots.Length)
        {
            Slot slot = _actionBarSlots[slotIndex];
            if (slot.HasItem)
            {
                Debug.Log($"Equipped {slot.Item.Data.ItemName}");
                _currentSlotIndex = slotIndex;
            }
        }
    }

    /*public Slot GetCurrentSlot() => _currentSlotIndex >= 0 ? _actionBarSlots[_currentSlotIndex] : null;*/


    // Updates a specific slot in the action bar
    public void UpdateSlot(Slot slot, int index, Item item)
    {
        if (item != null)
        {
            AssignItemToActionBar(slot, index);
            actionBarImage[index].sprite = item.Data.ItemSprite;
        }
    }

    public void ClearSlot(Slot slot, int index, Item item)
    {
        if (item != null)
        {
            Destroy(_actionBarSlots[index].Item.gameObject);
            item = null;
            actionBarImage[index].sprite = slot.DefaultSprite;
            _amountText[index].text = string.Empty;
        }
    }
}
