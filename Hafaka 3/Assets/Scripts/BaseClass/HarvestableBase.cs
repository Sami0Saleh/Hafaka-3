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
            PlayerController.Instance.IsHarvisting = true;
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
            PlayerController.Instance.IsHarvisting = false;
        }
    }

    public void Harvest()
    {
        if (!canHarvest) return;

        if (PlayerController.Instance.IsHarvisting)
        {
            currentHits++;
            PlayHarvestAnimation();

            if (currentHits >= maxHits)
            {
                HarvestComplete();
            }
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
