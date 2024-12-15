using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject _dropConfirmationPanel;

    public bool DropItem = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void OpenConfirmDropPanel()
    {
        _dropConfirmationPanel.SetActive(true);
    }
    public void ConfirmDropItem()
    {
        DropItem = true;
        _dropConfirmationPanel.SetActive(false);
    }
    public void DontDropItem()
    {
        DropItem = false;
        _dropConfirmationPanel.SetActive(false);
    }
}
