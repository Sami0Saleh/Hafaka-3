using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class CraftPanel : MonoBehaviour
{

    [SerializeField] private TMP_Text _blueprintNameText;
    [SerializeField] private TMP_Text _blueprintText;
    [SerializeField] private Button _craftButton;
    [SerializeField] private Button _starButton;
    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private Sprite _fullStar;

    private CraftingBlueprintUI _craftingBlueprintUI;
    private BluePrintData _blueprint;
    private CraftingSystem _craftingSystem;

    //public static event Action<CraftingBlueprintUI, BluePrintData, CraftingSystem> OnCraftPanel;

    public void SetCraftPanel(CraftingBlueprintUI craftingBlueprintUI, BluePrintData bluePrintData, CraftingSystem craftingSystem)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        _craftingBlueprintUI = craftingBlueprintUI;
        _blueprint = bluePrintData;
        _craftingSystem = craftingSystem;

        string requiredItems = "";
        foreach (var item in bluePrintData.RequiredIngerdients)
        {
            requiredItems += $"{item.ingerdientType.ToString()} ----------- {item.amount}\n";
        }

        if (bluePrintData.IsFavourite)
        {
            _starButton.image.sprite = _fullStar;
        }
        else
        {
            _starButton.image.sprite = _emptyStar;
        }

        _blueprintNameText.text = $"Name: {bluePrintData.BluePrintName}";
        _blueprintText.text = $"{requiredItems}";
        _craftButton.onClick.AddListener(() => _craftingSystem.CraftBlueprint(_blueprint));
    }

    public void SetCraftable(bool craftable)
    {
        //_blueprintText.fontStyle = (FontStyles)(craftable ? FontStyle.Bold : FontStyle.Normal);
        _craftButton.interactable = craftable;
    }

    public void SetFavourite()
    {
        _blueprint.IsFavourite = !_blueprint.IsFavourite;
        if (_blueprint.IsFavourite)
        {
            _starButton.image.sprite = _fullStar;
        }
        else
        {
            _starButton.image.sprite = _emptyStar;
        }
    }
}
