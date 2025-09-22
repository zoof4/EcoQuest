using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public CutsceneData currentCutscene;
    public Image cutsceneImage;
    public TextMeshProUGUI subtitleText;
    public AudioSource audioSource;
    
    public float effectDuration = 0.5f;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    private int currentPage = 0;
    private bool isTransitioning = false;
    private Coroutine currentShakeCoroutine;

    public int[] pagesWithEffects;
    public int[] pagesWithShake;

    public Image fadePanel;

    // ⭐⭐ 새로 추가된 변수: 테스트용으로 인트로 스킵 ⭐⭐
    public bool skipIntroForTesting = false;
    // ⭐⭐ 새로 추가된 변수 끝 ⭐⭐
    void Start()
    {   // ⭐⭐
        if (skipIntroForTesting || PlayerPrefs.GetInt("HasPlayedIntro", 0) == 0)    // 테스트를 위해⭐⭐
        {
            if (PlayerPrefs.GetInt("HasPlayedIntro", 0) == 0)
            {
                if (currentCutscene != null)
                {
                    // 페이드 패널을 씬 시작 시 투명하게 초기화
                    if (fadePanel != null)
                    {
                        Color panelColor = fadePanel.color;
                        fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 0f);
                    }
                    ShowPage(0);
                }
            }
            else
            {
                EndCutscene();
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTransitioning)
        {
            GoToNextPage();
        }
    }

    public void GoToNextPage()
    {
        currentPage++;
        if (currentPage < currentCutscene.cutsceneImages.Length)
        {
            isTransitioning = true;
            StartCoroutine(TransitionPage());
        }
        else
        {
            EndCutscene();
        }
    }

    public void EndCutscene()
    {
        PlayerPrefs.SetInt("HasPlayedIntro", 1);
        StartCoroutine(FadeOutToNextScene());
    }

    public IEnumerator FadeOutToNextScene()
    {
        float elapsedTime = 0f;
        Color panelColor = fadePanel.color;
        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0f, 1f, elapsedTime / 1.0f);
            fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, newAlpha);
            yield return null;
        }
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator TransitionPage()
    {
        bool hasEffects = System.Array.Exists(pagesWithEffects, element => element == currentPage);
        bool hasShake = System.Array.Exists(pagesWithShake, element => element == currentPage);

        if (hasEffects)
        {
            StartCoroutine(FadeImage(cutsceneImage, 1f, 0f, effectDuration));
            StartCoroutine(FadeText(subtitleText, 1f, 0f, effectDuration));
            yield return new WaitForSeconds(effectDuration);
        }

        cutsceneImage.sprite = currentCutscene.cutsceneImages[currentPage];
        if (currentPage < currentCutscene.cutsceneSubtitles.Length)
        {
            subtitleText.text = currentCutscene.cutsceneSubtitles[currentPage];
        }
        else
        {
            subtitleText.text = "";
        }
        
        if (hasEffects)
        {
            StartCoroutine(FadeImage(cutsceneImage, 0f, 1f, effectDuration));
            StartCoroutine(FadeText(subtitleText, 0f, 1f, effectDuration));
            yield return new WaitForSeconds(effectDuration);
        }
        else
        {
            cutsceneImage.color = new Color(1f, 1f, 1f, 1f);
            subtitleText.color = new Color(1f, 1f, 1f, 1f);
        }

        if (hasShake)
        {
            Debug.Log("셰이크 효과 시작!");
            if (currentShakeCoroutine != null)
            {
                StopCoroutine(currentShakeCoroutine);
            }
            currentShakeCoroutine = StartCoroutine(ShakeScreen(shakeDuration, shakeMagnitude));
        }

        if (hasShake)
        {
            yield return new WaitForSeconds(shakeDuration);
        }
        
        if (currentPage < currentCutscene.cutsceneAudioClips.Length && audioSource != null)
        {
            audioSource.PlayOneShot(currentCutscene.cutsceneAudioClips[currentPage]);
        }

        isTransitioning = false;
    }

    private void ShowPage(int pageIndex)
    {
        cutsceneImage.sprite = currentCutscene.cutsceneImages[pageIndex];
        if (pageIndex < currentCutscene.cutsceneSubtitles.Length)
        {
            subtitleText.text = currentCutscene.cutsceneSubtitles[pageIndex];
        }
        else
        {
            subtitleText.text = "";
        }
        cutsceneImage.color = new Color(1f, 1f, 1f, 1f);
        subtitleText.color = new Color(1f, 1f, 1f, 1f);
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

    private IEnumerator ShakeScreen(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;
    }
}
