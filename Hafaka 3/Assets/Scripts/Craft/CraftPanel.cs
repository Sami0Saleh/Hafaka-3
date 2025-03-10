using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftPanel : MonoBehaviour
{
    public static CraftPanel Instance { get; private set; }


    [SerializeField] private TMP_Text _blueprintNameText;
    [SerializeField] private TMP_Text _blueprintText;
    [SerializeField] private Button _craftButton;
    [SerializeField] private Button _starButton;
    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private Sprite _fullStar;

    private CraftingBlueprintUI _craftingBlueprintUI;
    private BluePrintData _blueprint;
    private CraftingSystem _craftingSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }


    public void SetCraftPanel(CraftingBlueprintUI craftingBlueprintUI)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        if (_blueprint != craftingBlueprintUI.Blueprint)
            gameObject.SetActive(true);
        
        _craftingBlueprintUI = craftingBlueprintUI;
        _blueprint = craftingBlueprintUI.Blueprint;
        _craftingSystem = craftingBlueprintUI.CraftingSystem;

        string requiredItems = "";
        foreach (var item in craftingBlueprintUI.Blueprint.RequiredIngerdients)
        {
            requiredItems += $"{item.ingerdientType.ToString()} ----- {item.amount}\n";
        }

        if (craftingBlueprintUI.Blueprint.IsFavourite)
        {
            _starButton.image.sprite = _fullStar;
        }
        else
        {
            _starButton.image.sprite = _emptyStar;
        }

        _blueprintNameText.text = $"Name: {craftingBlueprintUI.Blueprint.BluePrintName}";
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
        _craftingBlueprintUI.Blueprint.IsFavourite = !_craftingBlueprintUI.Blueprint.IsFavourite;
        if (_blueprint.IsFavourite)
        {
            _starButton.image.sprite = _fullStar;
            _craftingBlueprintUI.SetFavourite(true);
        }
        else
        {
            _starButton.image.sprite = _emptyStar;
            _craftingBlueprintUI.SetFavourite(false);
        }
        
    }
}
