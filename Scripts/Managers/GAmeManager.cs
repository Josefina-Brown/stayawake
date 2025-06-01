using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenu;
    public GameObject mainPausePanel;
    public GameObject controlsPanel;
    public GameObject playerUI;
    public GameObject confirmationExit;
    public GameObject optionsPanel;
    public GameObject winScreen;

    [Header("Audio")]
    public float musicFadeDuration = 2f;

    public Toggle toggleMuteMusic;
    public Toggle toggleMuteAll;
    public AudioSource musicSource;
    public AudioClip gameplayMusic;
    public AudioClip clickSound;
    public AudioClip winSound;

    [Header("Clock")]
    public TextMeshPro clockText;
    public float totalGameTimeInSeconds = 900f;
    private float elapsedGameTime = 0f;
    private System.TimeSpan startTime = new System.TimeSpan(18, 0, 0);
    private System.TimeSpan endTime = new System.TimeSpan(21, 0, 0);

    [Header("Clock Sound")]
    public AudioSource clockAudioSource;
    public AudioClip hourTickSound;
    private int lastHourPlayed = 18;

    private bool isPaused = false;
    public GameObject instruccionesPanel;
    public TextMeshProUGUI instruccionesText;
    private bool instruccionesActivas = false;
    private bool gameEnded = false;

    void Start()
    {
        ResumeGame();
        PlayGameplayMusic();

        toggleMuteMusic.onValueChanged.AddListener(OnToggleMusic);
        toggleMuteAll.onValueChanged.AddListener(OnToggleAllAudio);

        toggleMuteMusic.isOn = !musicSource.mute;
        toggleMuteAll.isOn = AudioListener.volume > 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (!gameEnded)
        {
            UpdateClock();
        }
    }

    void UpdateClock()
    {
        if (clockText == null || isPaused) return;

        elapsedGameTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedGameTime / totalGameTimeInSeconds);
        double totalSeconds = Mathf.Lerp((float)startTime.TotalSeconds, (float)endTime.TotalSeconds, t);
        System.TimeSpan currentTime = System.TimeSpan.FromSeconds(totalSeconds);

        // Mostrar hora
        clockText.text = currentTime.ToString(@"hh\:mm");

        // Reproducir sonido cada hora
        if (currentTime.Minutes == 0 && currentTime.Hours != lastHourPlayed)
        {
            lastHourPlayed = currentTime.Hours;

            if (clockAudioSource != null && hourTickSound != null)
            {
                clockAudioSource.PlayOneShot(hourTickSound);
            }
        }

        // Tiempo completado → ganar
        if (elapsedGameTime >= totalGameTimeInSeconds)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        gameEnded = true;
        StartCoroutine(FadeOutMusicAndPlayWinSound());
    }

    IEnumerator FadeOutMusicAndPlayWinSound()
    {
        float startVolume = musicSource.volume;

        // Fade out
        float t = 0f;
        while (t < musicFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / musicFadeDuration);
            yield return null;
        }

        musicSource.Stop(); // Detener la música
        musicSource.volume = startVolume; // Restaurar volumen por si se usa más tarde

        // Reproducir el sonido final
        if (winSound != null)
        {
            musicSource.PlayOneShot(winSound);
            yield return new WaitForSecondsRealtime(winSound.length);
        }

        // Silencio total después del sonido
        AudioListener.volume = 0f;

        // Mostrar pantalla de victoria y detener juego
        ShowWinScreen();
    }


    void ShowWinScreen()
    {
        Time.timeScale = 0f;
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pauseMenu.SetActive(true);
        mainPausePanel.SetActive(true);
        controlsPanel.SetActive(false);
        playerUI.SetActive(false);
        confirmationExit.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        playerUI.SetActive(true);
        pauseMenu.SetActive(false);
        mainPausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        confirmationExit.SetActive(false);
        optionsPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowControls()
    {
        mainPausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
        mainPausePanel.SetActive(true);
    }

    public void ShowOptions()
    {
        optionsPanel.SetActive(true);
        mainPausePanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void HideOptions()
    {
        optionsPanel.SetActive(false);
        mainPausePanel.SetActive(true);
    }

    private void PlayGameplayMusic()
    {
        if (musicSource != null && gameplayMusic != null)
        {
            musicSource.clip = gameplayMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayClick()
    {
        if (musicSource != null && clickSound != null)
        {
            musicSource.PlayOneShot(clickSound);
        }
    }

    public void ShowExitConfirmation()
    {
        optionsPanel.SetActive(false);
        confirmationExit.SetActive(true);
        mainPausePanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Init");
    }

    public void MostrarOcultarInstrucciones(string texto, bool mostrar)
    {
        if (instruccionesPanel == null || instruccionesText == null) return;

        instruccionesActivas = mostrar;
        instruccionesPanel.SetActive(mostrar);

        if (mostrar)
        {
            isPaused = true;
            Time.timeScale = 0f;
            instruccionesText.text = texto;
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1f;
            instruccionesText.text = "";
        }
    }

    public void OnToggleMusic(bool isOn)
    {
        if (musicSource != null)
            musicSource.mute = !isOn;
    }

    public void OnToggleAllAudio(bool isOn)
    {
        AudioListener.volume = isOn ? 1f : 0f;
    }
}
