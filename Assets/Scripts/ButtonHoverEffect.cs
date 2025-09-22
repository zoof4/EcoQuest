using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private Color originalImageColor;
    private Color originalTextColor;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalImageColor = buttonImage.color;
        
        if (buttonText != null)
        {
            originalTextColor = buttonText.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        // 버튼 배경색의 불투명도를 100%로
        Color targetImageColor = new Color(originalImageColor.r, originalImageColor.g, originalImageColor.b, 1f);
        fadeCoroutine = StartCoroutine(FadeColor(buttonImage, buttonImage.color, targetImageColor, 0.2f));

        // 텍스트 색상도 밝게
        if (buttonText != null)
        {
            Color targetTextColor = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, 1f);
            StartCoroutine(FadeTextColor(buttonText, buttonText.color, targetTextColor, 0.2f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        // 버튼 배경색을 원래 불투명도로
        fadeCoroutine = StartCoroutine(FadeColor(buttonImage, buttonImage.color, originalImageColor, 0.2f));

        // 텍스트 색상도 원래대로
        if (buttonText != null)
        {
            StartCoroutine(FadeTextColor(buttonText, buttonText.color, originalTextColor, 0.2f));
        }
    }

    private IEnumerator FadeColor(Image image, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }
        image.color = endColor;
    }

    private IEnumerator FadeTextColor(TextMeshProUGUI text, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }
        text.color = endColor;
    }
}