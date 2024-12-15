using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData Data { get; private set; }
    public int Amount { get; private set; }

    public void Initialize(ItemData data, int amount)
    {
        Data = data;
        Amount = amount;
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
