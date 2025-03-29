using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoatInteractionHandler : MonoBehaviour
{
    [SerializeField] private GameObject boatMotorPart; 
    private string requiredItemName = "BoatMotor"; // Name of the item in inventory
    public GameObject interactionUI; 

    private bool playerInRange = false;

    private void Start()
    {

        if (interactionUI)
            interactionUI.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionUI)
                interactionUI.SetActive(true); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionUI)
                interactionUI.SetActive(false); 
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryPlaceBoatMotor();
        }
    }

    private void TryPlaceBoatMotor()
    {
        if (Inventory.Instance.HasItem(requiredItemName)) // Check if player has the motor
        {
            Inventory.Instance.RemoveItemFromInventory(requiredItemName, 1); // Remove from inventory
            boatMotorPart.SetActive(true); // Enable the motor on the boat
            Destroy(interactionUI); // Remove the interaction UI
            Debug.Log("Boat Motor placed! You win!");

            // Transition to credits scene after a short delay
            //Invoke("LoadCreditsScene", 2f);
        }
        else
        {
            Debug.Log("You don't have the Boat Motor yet!");
            // You can show a UI message here if needed
        }
    }

    private void LoadCreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }
}
