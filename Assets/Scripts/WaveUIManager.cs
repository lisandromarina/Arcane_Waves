using TMPro;
using UnityEngine;

public class WaveUIManager : MonoBehaviour, IWaveUIManager
{
    [SerializeField] private CanvasGroup wavePanel;
    [SerializeField] private TextMeshProUGUI waveText;
    public float fadeDuration = 1f;

    private TextMeshProUGUI waveScoreText;
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private float fadeTime = 0f;

    void Start()
    {
        waveScoreText = wavePanel.GetComponentInChildren<TextMeshProUGUI>();
        wavePanel.alpha = 0f; // Hide the panel initially
        wavePanel.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleFade();
    }

    public void ShowWavePanel()
    {
        wavePanel.gameObject.SetActive(true);
        isFadingIn = true;
        isFadingOut = false;
        fadeTime = 0f;
        Invoke("HideWavePanel", 3f);
    }

    public void HideWavePanel()
    {
        isFadingIn = false;
        isFadingOut = true;
        fadeTime = 0f;
    }

    public void UpdateWaveScore(int currentWave)
    {
        if (waveScoreText != null)
        {
            waveScoreText.text = "Wave: " + currentWave;
            waveText.text = "WAVE\n\n" + currentWave.ToString();
        }
    }

    private void HandleFade()
    {
        if (isFadingIn)
        {
            fadeTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTime / fadeDuration);
            wavePanel.alpha = alpha;
            if (fadeTime >= fadeDuration)
            {
                isFadingIn = false;
            }
        }
        else if (isFadingOut)
        {
            fadeTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (fadeTime / fadeDuration));
            wavePanel.alpha = alpha;
            if (fadeTime >= fadeDuration)
            {
                isFadingOut = false;
                wavePanel.gameObject.SetActive(false);
            }
        }
    }
}
