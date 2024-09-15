using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioClip hoverSound;  // Sound for hovering over the button
    [SerializeField] private float soundVolume = 1f;  // Adjust the volume of sounds

    // This method is triggered when the mouse hovers over a button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance != null && GameManager.Instance.audioSource != null && hoverSound != null)
        {
            GameManager.Instance.audioSource.PlayOneShot(hoverSound, soundVolume);
        }
    }
}
