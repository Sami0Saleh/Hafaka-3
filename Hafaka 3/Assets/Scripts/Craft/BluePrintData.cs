using UnityEngine;

[CreateAssetMenu(fileName = "NewBluePrint", menuName = "Craft/Blueprint")]
public class BluePrintData : ScriptableObject
{
    public string BluePrintName;
    public Sprite BluePrintSprite;
    public int RockNeeded;
    public int BranchNeeded;
    public int FiberNeeded;
    public int AmountInStack = 1;
    public int MaxStackSize = 100;
    public ItemData ItemData;
    public GameObject ItemPrefab;
}
