using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    // === UI 오브젝트들 ===
    public Image backgroundImage;
    public Image titleImage;
    public Image menuContainer;
    public Button[] menuButtons;

    // === 애니메이션 설정 ===
    public float fadeDuration = 1.0f;
    public float buttonFadeDuration = 0.3f;
    public float containerExpandDuration = 0.5f;

    private Color originalBackgroundColor;

    void Awake()
    {
        // 배경 이미지의 원래 색상을 저장
        originalBackgroundColor = backgroundImage.color;
    }

    void Start()
    {
        // 모든 애니메이션 코루틴을 시작
        StartCoroutine(PlayIntroAnimations());
    }

    private IEnumerator PlayIntroAnimations()
    {
        // 1. 배경 인물 이미지: 하얗게 물들었다가 원래 색으로 되찾기
        backgroundImage.color = Color.white; 
        StartCoroutine(FadeColor(backgroundImage, Color.white, originalBackgroundColor, fadeDuration));
        yield return new WaitForSeconds(fadeDuration);

        // 2. 타이틀: 불이 켜지듯 네온사인 효과
        titleImage.color = new Color(1f, 1f, 1f, 0f);
        int flickerCount = 10;
        for (int i = 0; i < flickerCount; i++)
        {
            titleImage.color = new Color(1f, 1f, 1f, Random.Range(0f, 1f));
            yield return new WaitForSeconds(Random.Range(0.01f, 0.05f));
        }
        StartCoroutine(FadeImage(titleImage, titleImage.color.a, 1f, 0.5f));
        yield return new WaitForSeconds(0.5f);

        // ⭐ 3. 메뉴 컨테이너: 가로선에서 원래 설정한 크기로 확장
        float originalContainerHeight = menuContainer.rectTransform.sizeDelta.y;
        menuContainer.rectTransform.sizeDelta = new Vector2(menuContainer.rectTransform.sizeDelta.x, 0f);
        StartCoroutine(ExpandContainer(menuContainer.rectTransform, containerExpandDuration, originalContainerHeight));
        yield return new WaitForSeconds(containerExpandDuration);

        // 4. 버튼: 순서대로 나타나기
        foreach (Button button in menuButtons)
        {
            Image buttonImage = button.GetComponent<Image>();
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
            if (buttonText != null)
            {
                buttonText.alpha = 0f;
            }

            StartCoroutine(FadeImage(buttonImage, 0f, 1f, buttonFadeDuration));
            if (buttonText != null)
            {
                StartCoroutine(FadeText(buttonText, 0f, 1f, buttonFadeDuration));
            }

            yield return new WaitForSeconds(buttonFadeDuration);
        }
    }

    // === 보조 코루틴 함수들 ===
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

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }
        image.color = new Color(color.r, color.g, color.b, endAlpha);
    }
    
    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        if (text == null) yield break;

        float elapsedTime = 0f;
        Color color = text.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            text.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }
        text.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private IEnumerator ExpandContainer(RectTransform rectTransform, float duration, float targetHeight)
    {
        float elapsedTime = 0f;
        float originalHeight = rectTransform.sizeDelta.y;
        Vector2 originalSize = rectTransform.sizeDelta;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newHeight = Mathf.Lerp(originalHeight, targetHeight, elapsedTime / duration);
            rectTransform.sizeDelta = new Vector2(originalSize.x, newHeight);
            yield return null;
        }
        rectTransform.sizeDelta = new Vector2(originalSize.x, targetHeight);
    }

    // --- 버튼 클릭 함수들 ---
    public void OnStartButtonClicked()
    {
        Debug.Log("새 게임 시작!");
    }

    public void OnContinueButtonClicked()
    {
        Debug.Log("이어하기!");
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("설정창 열기!");
    }
}
