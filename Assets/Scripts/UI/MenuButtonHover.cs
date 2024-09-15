using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioClip hoverSound;  // Sound for hovering over the button
    [SerializeField] private float soundVolume = 1f;  // Volume of the hover sound
    private AudioSource audioSource;  // Local AudioSource

    private void Start()
    {
        // Attempt to find an AudioSource attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        // If no AudioSource is found, log a warning
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on " + gameObject.name + ". Please attach one.");
        }
    }

    // This method is triggered when the mouse hovers over a button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, soundVolume);
        }
    }
}
