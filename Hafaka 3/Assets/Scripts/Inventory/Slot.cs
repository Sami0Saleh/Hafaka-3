using NUnit.Framework.Interfaces;
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

    public Item Item { get => _item; set => _item = value; }

    private void Start()
    {
        UpdateSlotUI();
    }

    public void AssignItem(ItemData itemData, int amountToAssign, IngerdientType ingerdientType)
    {
        if (_item == null)
        {
            GameObject newItemObject = new GameObject("Item");
            newItemObject.transform.SetParent(transform);
            _item = newItemObject.AddComponent<Item>();
        }

        _item.Initialize(itemData, Mathf.Min(amountToAssign, itemData.MaxStackSize), ingerdientType);
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

    public void UpdateSlotUI()
    {
        Image slotImage = GetComponent<Image>();
        if (_item != null)
        {
            slotImage.sprite = _item.Data.ItemSprite;
            if (_item.Amount >= 1)
            {
                _amountText.text = _item.Amount.ToString();
            }
            else
            {
                _item = null;
                _amountText.text = string.Empty;
                slotImage.sprite = _defaultSprite;
            }
             
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
                DropItem();
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
                AssignItem(sourceSlot._item.Data, sourceSlot._item.Amount, sourceSlot._item.IngerdientType);
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
            // Find the player's position and forward direction
            GameObject player = GameObject.FindWithTag("Player"); // Ensure your player GameObject has the "Player" tag
            if (player != null)
            {
                Transform playerTransform = player.transform;
                Vector3 dropPosition = playerTransform.position + playerTransform.forward * 1.5f; // Drop 1.5 units in front of the player
                //dropPosition.y = 0; // Ensure the item is dropped on the ground

                // Instantiate item in the world
                GameObject droppedItem = Instantiate(_item.Data.ItemPrefab, dropPosition, transform.rotation.normalized);
                BoxPickup droppedBox = droppedItem.GetComponent<BoxPickup>();
                if (droppedBox != null)
                {
                    droppedBox.itemData = _item.Data;
                    droppedBox.AmountInStack = _item.Amount;
                }
                Debug.Log($"Dropped {_item.Data.ItemName} in front of the player at {dropPosition}.");

                var ingredient = Inventory.Instance.ingerdients.Find(x => x.ingerdientType == _item.IngerdientType);
                int index = Inventory.Instance.ingerdients.IndexOf(ingredient);
                Inventory.Instance.ingerdients.RemoveAt(index);

                Destroy(_item.gameObject);
                _item = null;
                UpdateSlotUI();
            }
            else
            {
                Debug.LogWarning("Player not found! Ensure the Player GameObject has the 'Player' tag.");
            }
        }
    }

    public bool IsSameItem(string itemName)
    {
        return _item != null && _item.Data.ItemName == itemName;
    }
}