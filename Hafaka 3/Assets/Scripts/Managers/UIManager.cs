using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Inventory _inventory;
    [SerializeField] CraftingSystem _craftingSystem;
    [SerializeField] GameObject _sortArrowsPanel;
    [SerializeField] GameObject _toolsPanel;
    [SerializeField] GameObject _weaponsPanel;
    [SerializeField] GameObject _clothesPanel;
    [SerializeField] GameObject _foodPanel;
    [SerializeField] GameObject _buildingsPanel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    
    public void OpenInventory()
    {
        _inventory.SetInventory();
        _craftingSystem.SetCraft();
        CraftPanel.Instance.gameObject.SetActive(false);
    }
    public void OpenCraft()
    {
        _craftingSystem.SetCraft();
        _inventory.SetInventory();
    }

    public void SortArrows()
    {
        _sortArrowsPanel.SetActive(!_sortArrowsPanel.activeInHierarchy);
    }

    public void Tools()
    {
        _toolsPanel.SetActive(true);
        _weaponsPanel.SetActive(false);
        _clothesPanel.SetActive(false);
        _foodPanel.SetActive(false);
        _buildingsPanel.SetActive(false);
        CraftPanel.Instance.gameObject.SetActive(false);
    }

    public void Weapons()
    {
        _weaponsPanel.SetActive(true);
        _toolsPanel.SetActive(false);
        _clothesPanel.SetActive(false);
        _foodPanel.SetActive(false);
        _buildingsPanel.SetActive(false);
        CraftPanel.Instance.gameObject.SetActive(false);
    }

    public void Clothes()
    {
        _clothesPanel.SetActive(true);
        _toolsPanel.SetActive(false);
        _weaponsPanel.SetActive(false);
        _foodPanel.SetActive(false);
        _buildingsPanel.SetActive(false);
        CraftPanel.Instance.gameObject.SetActive(false);
    }

    public void Food()
    {
        _foodPanel.SetActive(true);
        _toolsPanel.SetActive(false);
        _weaponsPanel.SetActive(false);
        _clothesPanel.SetActive(false);
        _buildingsPanel.SetActive(false);
        CraftPanel.Instance.gameObject.SetActive(false);
    }

    public void Buildings()
    {
        _buildingsPanel.SetActive(true);
        _toolsPanel.SetActive(false);
        _weaponsPanel.SetActive(false);
        _clothesPanel.SetActive(false);
        _foodPanel.SetActive(false);
        CraftPanel.Instance.gameObject.SetActive(false);
    }
}
