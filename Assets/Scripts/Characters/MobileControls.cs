using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour
{
    public Button jumpButton;
    public Button leftButton;
    public Button rightButton;
    public Button slideButton;
    public Button pauseButton;  // Reference to PauseButton
    public GameObject buttonPanel;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("No PlayerMovement script found!");
        }

        // Add listener for the PauseButton
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonPress); // Add listener to pause button
        }
    }

    void Update()
    {
        // Ensure the mobile controls stay visible while moving
        if (buttonPanel != null && !buttonPanel.activeSelf)
        {
            buttonPanel.SetActive(true);  // Ensure buttons are always enabled during gameplay
        }
    }

    public void DisableMobileControls()
    {
        // Disable the buttons by making them non-interactable
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            button.interactable = false;
        }
    }

    public void EnableMobileControls(bool enable)
    {
        jumpButton.gameObject.SetActive(enable);
        leftButton.gameObject.SetActive(enable);
        rightButton.gameObject.SetActive(enable);
        slideButton.gameObject.SetActive(enable);
        pauseButton.gameObject.SetActive(enable);  // Enable/disable PauseButton

        if (enable)
        {
            buttonPanel.SetActive(true);  // Ensure the button panel is active when re-enabled
        }
    }

    public void OnLeftButtonPress(BaseEventData eventData)
    {
        Debug.Log("Left Button Pressed");
        playerMovement.SetMobileHorizontalInput(-1f);  // Move left
    }

    public void OnLeftButtonRelease(BaseEventData eventData)
    {
        Debug.Log("Left Button Released");
        playerMovement.SetMobileHorizontalInput(0f);  // Stop moving left
    }

    public void OnRightButtonPress(BaseEventData eventData)
    {
        Debug.Log("Right Button Pressed");
        playerMovement.SetMobileHorizontalInput(1f);  // Move right
    }

    public void OnRightButtonRelease(BaseEventData eventData)
    {
        Debug.Log("Right Button Released");
        playerMovement.SetMobileHorizontalInput(0f);  // Stop moving right
    }

    public void OnJumpButtonPress(BaseEventData eventData)
    {
        playerMovement.Jump();  // Trigger jump action
    }

    public void OnSlideButtonPress(BaseEventData eventData)
    {
        playerMovement.SlideMobile();  // Trigger slide action
    }

    // Function for PauseButton press
    public void OnPauseButtonPress()
    {
        Debug.Log("Pause Button Pressed");
        playerMovement.TogglePauseMenu();  // Call the function in PlayerMovement to toggle the pause menu
    }
}
