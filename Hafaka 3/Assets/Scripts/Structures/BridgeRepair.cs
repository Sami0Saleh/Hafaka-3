using TMPro;
using UnityEngine;

public class BridgeRepair : MonoBehaviour
{
    [SerializeField] private CraftingBench craftingBench; // Reference to the Crafting Bench
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material repairedMaterial;
    [SerializeField] private TextMeshPro m_TextMeshProUGUI;
    [SerializeField] private Collider bridgeCollider;

    [SerializeField] private int _amountOfBranchesNeeded = 10;
    [SerializeField] private int _amountOfLogsNeeded = 5;

    private int _amountOfBranches = 0;
    private int _amountOfLogs = 0;
    private bool playerInRange = false;

    private PlayerHealth player;

    public bool IsRepaired => _amountOfBranches >= _amountOfBranchesNeeded && _amountOfLogs >= _amountOfLogsNeeded;

    private void Start()
    {
        UpdateUIText();
        m_TextMeshProUGUI.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsRepaired)
        {
            if (other.TryGetComponent(out player))
            {
                playerInRange = true;
                m_TextMeshProUGUI.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            m_TextMeshProUGUI.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsRepaired)
        {
            if (m_TextMeshProUGUI.gameObject.activeSelf && player != null)
            {

                Vector3 dir = m_TextMeshProUGUI.transform.position - player.Eyes.transform.position;
                // make sure the direction points at the EYES of the _playerTransform rather then his genitals

                m_TextMeshProUGUI.transform.rotation = Quaternion.LookRotation(dir);
            }

            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                TryRepair();
                UpdateUIText();
            }
        }
    }

    private void TryRepair()
    {
        // Check if the crafting bench is repaired before allowing bridge repair
        if (!craftingBench.IsRepaired)
        {
            Debug.Log("You need to repair the Crafting Bench first!");
            return;
        }

        if (IsRepaired)
        {
            Debug.Log("Bridge is already repaired!");
            return;
        }

        int branchesAvailable = Inventory.Instance.GetItemCount("Branch");
        int logsAvailable = Inventory.Instance.GetItemCount("WoodLog");

        int branchesToAdd = Mathf.Min(_amountOfBranchesNeeded - _amountOfBranches, branchesAvailable);
        int logsToAdd = Mathf.Min(_amountOfLogsNeeded - _amountOfLogs, logsAvailable);

        if (branchesToAdd > 0)
        {
            Inventory.Instance.RemoveItemFromInventory("Branch", branchesToAdd);
            _amountOfBranches += branchesToAdd;
        }

        if (logsToAdd > 0)
        {
            Inventory.Instance.RemoveItemFromInventory("WoodLog", logsToAdd);
            _amountOfLogs += logsToAdd;
        }

        if (IsRepaired)
        {
            meshRenderer.material = repairedMaterial; // Change material when fully repaired
            bridgeCollider.enabled = false;
            Debug.Log("Bridge Repaired!");
        }
    }

    private void UpdateUIText()
    {
        if (!craftingBench.IsRepaired)
        {
            m_TextMeshProUGUI.text = "You must repair the Crafting Bench first!";
        }
        else
        {
            m_TextMeshProUGUI.text = $"Press 'E' to Repair\n" +
               $"{_amountOfBranches}/{_amountOfBranchesNeeded} Branches\n" +
               $"{_amountOfLogs}/{_amountOfLogsNeeded} Wooden Logs\n";
        }
    }
}
