using UnityEngine;

public class HarvestableRock : HarvestableBase
{
    [SerializeField] private GameObject _upperPart;
    [SerializeField] private ItemData _resourceType;

    [SerializeField] private int _maxHits = 5;
    
    private void Start()
    {
        upperPart = _upperPart;
        maxHits = _maxHits;
    }

    protected override void HarvestComplete()
    {
        if (upperPart != null) upperPart.SetActive(false);

        AddResourcesToInventory(_resourceType);
        Destroy(gameObject, 2f);
    }
}
