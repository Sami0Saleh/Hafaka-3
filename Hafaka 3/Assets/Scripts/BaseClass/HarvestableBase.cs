using UnityEngine;

public abstract class HarvestableBase : MonoBehaviour
{
    protected int maxHits; // Number of hits required
    protected GameObject upperPart; // Part that disappears

    protected int currentHits = 0;
    protected bool canHarvest = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canHarvest = true;
        }

        if (other.CompareTag("MultiTool"))
        {
            Harvest();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canHarvest = false;
        }
    }

    public void Harvest()
    {
        if (!canHarvest) return;

        currentHits++;
        PlayHarvestAnimation();

        if (currentHits >= maxHits)
        {
            HarvestComplete();
        }
    }

    protected abstract void HarvestComplete(); // Unique behavior per subclass

    protected virtual void PlayHarvestAnimation()
    {
        // Implement animation logic (can be overridden)
    }

    protected void AddResourcesToInventory(ItemData resourceType)
    {
        Inventory.Instance.TryAddHarvestItem(resourceType);
    }
}
