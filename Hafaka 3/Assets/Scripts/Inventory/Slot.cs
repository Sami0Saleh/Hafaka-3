using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item SlotItem;

    [SerializeField] Sprite _defaultSprite;
    [SerializeField] TMP_Text _amountText;

    private void Start()
    {
        _amountText.text = "";
    }
    private void Update()
    {
        CheckForItem();
    }

    public void CheckForItem()
    {
        if (transform.childCount > 1)
        {
            SlotItem = transform.GetChild(1).GetComponent<Item>();
            GetComponent<Image>().sprite = SlotItem.itemSprite;
            if (SlotItem.amountInStack > 1)
                _amountText.text = SlotItem.amountInStack.ToString();
        }
        else
        {
            SlotItem = null;
            GetComponent<Image>().sprite = _defaultSprite;
            _amountText.text = "";
        }
    }
}
