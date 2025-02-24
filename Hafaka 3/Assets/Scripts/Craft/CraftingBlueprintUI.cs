using TMPro;
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
        //CraftPanel.OnCraftPanel += SetButton;
    }

    public void SetCraftable(bool craftable)
    {
        _craftable = craftable;
    }

    public void SetButton()
    {

        //CraftPanel.Instance.SetCraftPanel(this, _blueprint, _craftingSystem);
        //CraftPanel.Instance.SetCraftable(_craftable);
    }
}
