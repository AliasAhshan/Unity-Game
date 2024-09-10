using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/* Manages and updates the HUD, which contains your health bar, coins, etc */

public class HUD : MonoBehaviour
{
    [Header ("Reference")]
    public Animator animator;
    [SerializeField] private GameObject ammoBar;
    public TextMeshProUGUI coinsMesh;
    [SerializeField] private Image healthBar;  // Image for the health bar
    [SerializeField] private Image inventoryItemGraphic;
    [SerializeField] private GameObject startUp;

    private float ammoBarWidth;
    private float ammoBarWidthEased;
    [System.NonSerialized] public Sprite blankUI;
    private float coins;
    private float coinsEased;
    [System.NonSerialized] public string loadSceneName;
    [System.NonSerialized] public bool resetPlayer;

    // Health Bar colors
    public Color aliveColor = Color.green;   // Color when player is alive
    public Color deadColor = Color.black;    // Color when player is dead

    private bool playerIsDead = false;       // Track if the player has died

    // Reference to PlayerMovement script
    private PlayerMovement playerMovementScript;

    void Start()
    {
        // Find the PlayerMovement script
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerMovementScript = playerObj.GetComponent<PlayerMovement>();
        }

        // Set the health bar to full and the color to the alive color
        healthBar.color = aliveColor;

        // Set initial coins value if you are tracking that
        coins = 0;  // Initialize based on your actual coin setup
        coinsEased = coins;
        blankUI = inventoryItemGraphic.GetComponent<Image>().sprite;
    }

    void Update()
    {
        // Update coins text mesh to reflect how many coins the player has
        coinsMesh.text = Mathf.Round(coinsEased).ToString();
        coinsEased += (coins - coinsEased) * Time.deltaTime * 5f;

        if (coinsEased >= coins)
        {
            animator.SetTrigger("getGem");
            coins = coinsEased + 1;
        }

        // Check if the player has died
        if (playerMovementScript != null && playerMovementScript.currentHealth <= 0 && !playerIsDead)
        {
            // Trigger death behavior
            playerIsDead = true;
            StartCoroutine(FadeOutHealthBar());
        }
    }

    public void PlayerDied()
    {
        // Change health bar color to deathColor (black)
        healthBar.color = deadColor;
        Debug.Log("Player has died, health bar updated.");
    }

    IEnumerator FadeOutHealthBar()
    {
        float duration = 0.5f;  // Duration of the fade out
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Gradually change the health bar color to the dead color
            healthBar.color = Color.Lerp(aliveColor, deadColor, elapsedTime / duration);

            yield return null;  // Wait for the next frame
        }

        // Ensure the health bar is fully faded out
        healthBar.color = deadColor;
    }

    public void HealthBarHurt()
    {
        animator.SetTrigger("hurt");
    }

    public void SetInventoryImage(Sprite image)
    {
        inventoryItemGraphic.sprite = image;
    }

    // void ResetScene()
    // {
    //     if (GameManager.Instance.inventory.ContainsKey("reachedCheckpoint"))
    //     {
    //         // Send player back to the checkpoint
    //         NewPlayer.ResetLevel();
    //     }
    //     else
    //     {
    //         // Reload entire scene
    //         SceneManager.LoadScene(loadSceneName);
    //     }
    // }
}
