using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private TMP_Text _amountText;


    private Item _item;
    private Transform _originalParent;
    private GameObject _draggedItem;

    public bool HasItem => _item != null;
    public bool IsFull => _item != null && _item.Amount >= _item.Data.MaxStackSize;
    public int RemainingCapacity => _item != null ? _item.Data.MaxStackSize - _item.Amount : 0;

    private void Start()
    {
        UpdateSlotUI();
    }

    public void AssignItem(ItemData itemData, int amountToAssign)
    {
        if (_item == null)
        {
            GameObject newItemObject = new GameObject("Item");
            newItemObject.transform.SetParent(transform);
            _item = newItemObject.AddComponent<Item>();
        }

        _item.Initialize(itemData, Mathf.Min(amountToAssign, itemData.MaxStackSize));
        UpdateSlotUI();
    }

    public void AddToStack(int amount)
    {
        if (_item != null && !IsFull)
        {
            _item.AddAmount(amount);
        }
        UpdateSlotUI();
    }

    public bool IsSameItem(ItemData itemData)
    {
        return _item != null && _item.Data.ItemID == itemData.ItemID;
    }

    private void UpdateSlotUI()
    {
        Image slotImage = GetComponent<Image>();
        if (_item != null)
        {
            slotImage.sprite = _item.Data.ItemSprite;
            _amountText.text = _item.Amount >= 1 ? _item.Amount.ToString() : string.Empty;
        }
        else
        {
            slotImage.sprite = _defaultSprite;
            _amountText.text = string.Empty;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_item != null)
        {
            // Create a visual representation of the dragged item
            _draggedItem = new GameObject("DraggedItem");
            _draggedItem.transform.SetParent(transform.root);
            _draggedItem.transform.localScale = Vector3.one;

            Image draggedImage = _draggedItem.AddComponent<Image>();
            draggedImage.sprite = _item.Data.ItemSprite;
            draggedImage.raycastTarget = false;

            _originalParent = transform;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedItem != null)
        {
            _draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedItem != null)
        {
            Destroy(_draggedItem);

            // If dropped on an invalid area, return the item to its original slot
            if (eventData.pointerEnter == null || !eventData.pointerEnter.GetComponent<Slot>())
            {
                UpdateSlotUI();
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Slot sourceSlot = eventData.pointerDrag?.GetComponent<Slot>();
        if (sourceSlot != null && sourceSlot.HasItem)
        {
            if (!HasItem)
            {
                AssignItem(sourceSlot._item.Data, sourceSlot._item.Amount);
                Destroy(sourceSlot._item.gameObject);
                sourceSlot._item = null;
            }

            UpdateSlotUI();
            sourceSlot.UpdateSlotUI();
        }
    }

    public void DropItem()
    {
        if (_item != null)
        {
            if (UIManager.Instance.DropItem)
            {
                // Instantiate item in the world
                Vector3 dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dropPosition.z = 0; // Ensure item is dropped on the ground

                GameObject droppedItem = Instantiate(_item.Data.ItemPrefab, dropPosition, Quaternion.identity);
                Debug.Log($"Dropped {_item.Data.ItemName} into the world.");

                Destroy(_item.gameObject);
                _item = null;
                UpdateSlotUI();
            }
            else
            {
                // Return item to the slot
                UpdateSlotUI();
            }
        }
    }

    
}