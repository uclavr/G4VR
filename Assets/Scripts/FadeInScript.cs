using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float fadeDuration = 0.5f;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; 
    }

    void OnEnable()
    {
        canvasGroup.alpha = 0f;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = time / fadeDuration;
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f; 
    }
}

