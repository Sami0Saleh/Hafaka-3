using UnityEngine;
using UnityEngine.UI;

public class CraftingBlueprintUI : MonoBehaviour
{
    
    [SerializeField] private Image _bluePrint;
    [SerializeField] private Image _blueprintImage;
    [SerializeField] private Image _star;
    [SerializeField] private Sprite _emptyStar;
    [SerializeField] private Sprite _fullStar;
    [SerializeField] private Button _button;


    private BluePrintData _blueprint;
    private CraftingSystem _craftingSystem;
    private bool _craftable;

    public BluePrintData Blueprint { get => _blueprint; set => _blueprint = value; }
    public CraftingSystem CraftingSystem { get => _craftingSystem; set => _craftingSystem = value; }


    public void SetBlueprint(BluePrintData blueprint, CraftingSystem craftingSystem)
    {
        _blueprint = blueprint;
        _craftingSystem = craftingSystem;
        _blueprintImage.sprite = blueprint.BluePrintSprite;

        if (blueprint.IsFavourite)
        {
            _star.sprite = _fullStar;
        }
        else
        {
            _star.sprite = _emptyStar;
        }
    }

    public void SetCraftable(bool craftable)
    {
        _craftable = craftable;
        CraftPanel.Instance.SetCraftable(_craftable);
    }

    public void SetFavourite(bool fav)
    {
        _star.sprite = fav ? _fullStar : _emptyStar;
    }

    public void SetButton()
    {
        CraftPanel.Instance.SetCraftPanel(this);
        CraftPanel.Instance.SetCraftable(_craftable);
    }
}
