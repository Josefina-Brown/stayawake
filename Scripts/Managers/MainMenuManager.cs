using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject storyPanel;

    [Header("Transition Settings")]
    public float transitionDuration = 0.4f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip menuMusic;

    private void Start()

    {
        AudioListener.volume = 1f;
        Time.timeScale = 1;
        ShowPanel(mainMenuPanel);
        HidePanel(controlsPanel);
        HidePanel(storyPanel);

        PlayMenuMusic();
    }

    public void OnPlayClicked()
    {
        PlayClick();
        HidePanel(mainMenuPanel);
        ShowPanel(storyPanel);
    }

    public void OnControlsClicked()
    {
        PlayClick();
        HidePanel(mainMenuPanel);
        ShowPanel(controlsPanel);
    }

    public void OnBackFromControls()
    {
        PlayClick();
        HidePanel(controlsPanel);
        ShowPanel(mainMenuPanel);
    }

    public void OnBackFromStory()
    {
        PlayClick();
        HidePanel(storyPanel);
        ShowPanel(mainMenuPanel);
    }

    public void OnContinueFromStory()
    {
        PlayClick();
        SceneManager.LoadScene(1);
    }

    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
            StartCoroutine(FadeCanvasGroup(cg, 0f, 1f));
    }

    private void HidePanel(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
            StartCoroutine(FadeAndDisable(cg));
        else
            panel.SetActive(false);
    }

    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to)
    {
        float elapsed = 0f;
        cg.alpha = from;
        while (elapsed < transitionDuration)
        {
            cg.alpha = Mathf.Lerp(from, to, elapsed / transitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = to;
    }

    private System.Collections.IEnumerator FadeAndDisable(CanvasGroup cg)
    {
        yield return FadeCanvasGroup(cg, 1f, 0f);
        cg.gameObject.SetActive(false);
    }

    private void PlayClick()
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);
    }

    private void PlayMenuMusic()
    {
        if (audioSource && menuMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
