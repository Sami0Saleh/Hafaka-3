using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingBlueprintUI : MonoBehaviour
{
    [SerializeField] private Image _bluePrint;
    [SerializeField] private TMP_Text _blueprintText;
    [SerializeField] private Image _blueprintImage;
    [SerializeField] private Button _craftButton;

    private BluePrintData _blueprint;
    private CraftingSystem _craftingSystem;

    public void SetBlueprint(BluePrintData blueprint, CraftingSystem craftingSystem)
    {
        _blueprint = blueprint;
        _craftingSystem = craftingSystem;
        _blueprintImage.sprite = blueprint.BluePrintSprite;

        string requiredItems = "";
        foreach (var item in blueprint.RequiredIngerdients)
        {
            requiredItems += $"{item.ingerdientType.ToString()}: {item.amount}\n";
        }

        _blueprintText.text = $"Name: {blueprint.BluePrintName}\nRequired:\n{requiredItems}";
        _craftButton.onClick.AddListener(() => _craftingSystem.CraftBlueprint(_blueprint));
    }

    public void SetCraftable(bool craftable)
    {
        _bluePrint.color = craftable ? Color.yellow : Color.red;
        _blueprintText.fontStyle = (FontStyles)(craftable ? FontStyle.Bold : FontStyle.Normal);
        _craftButton.interactable = craftable;
    }
}
