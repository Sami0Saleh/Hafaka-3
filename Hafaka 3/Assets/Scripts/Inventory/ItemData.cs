using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;
    public int AmountInStack = 1;
    public int MaxStackSize = 100;
    public int ItemID;
    public GameObject ItemPrefab;
    public IngerdientType ingerdientType;
}
