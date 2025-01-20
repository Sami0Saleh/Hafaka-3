using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData Data { get; private set; }
    public int Amount { get; private set; }

    public IngerdientType IngerdientType { get; private set; }

    public void Initialize(ItemData data, int amount, IngerdientType ingerdientType)
    {
        Data = data;
        Amount = amount;
        IngerdientType = ingerdientType;
    }

    public void AddAmount(int amount)
    {
        Amount += amount;
    }

    public void RemoveAmount(int amount)
    {
        Amount = Mathf.Max(0, Amount - amount);
    }
}
