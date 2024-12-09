using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private TMP_Text _amountText;

    private ItemData _slotItemData;
    private int _currentStackAmount;

    public bool HasItem => _slotItemData != null;
    public bool IsFull => _currentStackAmount >= _slotItemData.MaxStackSize;
    public int RemainingCapacity => _slotItemData.MaxStackSize - _currentStackAmount;

    public static event Action<Slot> OnSlotPressed;

    private void Start()
    {
        UpdateSlotUI();
    }

    public void AssignItem(ItemData itemData, int amountToAssign)
    {
        _slotItemData = itemData;
        _currentStackAmount = Mathf.Min(amountToAssign, itemData.MaxStackSize);
        UpdateSlotUI();
    }

    public void AddToStack(int amount)
    {
        if (_slotItemData != null && !IsFull)
        {
            int stackableAmount = Mathf.Min(amount, RemainingCapacity);
            _currentStackAmount += stackableAmount;
        }
        UpdateSlotUI();
    }

    public bool IsSameItem(ItemData itemData)
    {
        return _slotItemData != null && _slotItemData.ItemID == itemData.ItemID;
    }

    private void UpdateSlotUI()
    {
        Image slotImage = GetComponent<Image>();
        if (_slotItemData != null)
        {
            slotImage.sprite = _slotItemData.ItemSprite;
            _amountText.text = _currentStackAmount >= 1
                ? _currentStackAmount.ToString()
                : string.Empty;
        }
        else
        {
            slotImage.sprite = _defaultSprite;
            _amountText.text = string.Empty;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSlotPressed?.Invoke(this);
        Debug.Log("Pressed");
    }
}
