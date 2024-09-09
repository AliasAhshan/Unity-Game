using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image faderImage;            // Reference to the Image component (the black screen)
    public float fadeDuration = 1f;     // Time it takes to fade in/out

    private void Start()
    {
        // Start with a fade-in
        StartCoroutine(FadeIn());
    }

    // Call this function to fade out and load a new scene
    public void FadeTo(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    // Coroutine to fade the screen in
    IEnumerator FadeIn()
    {
        float timeElapsed = 0f;
        Color color = faderImage.color;
        color.a = 1f;  // Start fully black
        faderImage.color = color;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            color.a = 1f - (timeElapsed / fadeDuration);  // Gradually reduce alpha
            faderImage.color = color;
            yield return null;
        }

        color.a = 0f;
        faderImage.color = color;  // Make sure it's fully transparent at the end
    }

    // Coroutine to fade the screen out and load a new scene
    IEnumerator FadeOut(string sceneName)
    {
        float timeElapsed = 0f;
        Color color = faderImage.color;
        color.a = 0f;  // Start fully transparent
        faderImage.color = color;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            color.a = timeElapsed / fadeDuration;  // Gradually increase alpha
            faderImage.color = color;
            yield return null;
        }

        // Load the new scene when fade-out is complete
        SceneManager.LoadScene(sceneName);
    }
}
