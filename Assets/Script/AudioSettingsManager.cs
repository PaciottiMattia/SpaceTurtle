using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager instance;

    [Header("--- COMPONENTI AUDIO ---")]
    public AudioSource musicAudioSource;
    public AudioMixer mainMixer;

    [Header("--- GRAFICA PULSANTI ---")]
    public Sprite audioOnSprite;   
    public Sprite audioOffSprite;  

    [Header("--- MUSICHE PER LE SCENE ---")]
    public AudioClip backgroundMusicMenu;     
    public AudioClip backgroundMusicGame;     
    public AudioClip backgroundMusicTutorial; 

    // Variabili private
    private TextMeshProUGUI muteButtonText;
    private Image muteButtonImage;
    private bool isMuted = false;

    private const string MUSIC_VOLUME_PARAM = "MusicVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Cerca pulsanti UI (se ci sono)
        RefreshButtonReference(); // Usiamo la funzione interna per non ripetere codice

        // 2. Applica le impostazioni salvate
        isMuted = PlayerPrefs.GetInt("IsAudioMuted", 0) == 1;
        UpdateMixerVolume();
        if (muteButtonImage != null) UpdateMuteButtonVisuals();

        // --- 3. GESTIONE MUSICA ---
        AudioClip clipDaSuonare = null;
        bool fermaMusica = false; 

        if (scene.name == "SchermataIniziale") 
        {
            clipDaSuonare = backgroundMusicMenu;
        }
        else if (scene.name == "Primo") 
        {
            clipDaSuonare = backgroundMusicGame;
        }
        else if (scene.name == "TutorialScene") 
        {
            clipDaSuonare = backgroundMusicTutorial;
        }
        else if (scene.name == "LoreScene") 
        {
            fermaMusica = true; // Silenzio per la Lore
        }

        // 4. Applica la logica musicale
        if (fermaMusica)
        {
            if (musicAudioSource.isPlaying) musicAudioSource.Stop();
        }
        else if (clipDaSuonare != null)
        {
            if (musicAudioSource.clip != clipDaSuonare)
            {
                musicAudioSource.clip = clipDaSuonare;
                musicAudioSource.Play();
            }
            else if (!musicAudioSource.isPlaying)
            {
                musicAudioSource.Play(); 
            }
        }
    }

    // --- QUESTA È LA FUNZIONE CHE MANCAVA E CAUSAVA L'ERRORE ---
    public void RefreshButtonReference()
    {
        // Cerca il Testo
        GameObject textObj = GameObject.Find("MuteButtonText");
        if (textObj != null) muteButtonText = textObj.GetComponent<TextMeshProUGUI>();

        // Cerca il Bottone Principale
        GameObject btnObj = GameObject.Find("MuteButton");
        if (btnObj != null)
        {
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners(); 
                btn.onClick.AddListener(ToggleMute);
            }
            muteButtonImage = btnObj.GetComponent<Image>();
        }
        
        // Aggiorna subito la grafica se trovata
        if (muteButtonImage != null) UpdateMuteButtonVisuals();
    }
    // -----------------------------------------------------------

    public void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateMixerVolume();
        UpdateMuteButtonVisuals();
        PlayerPrefs.SetInt("IsAudioMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PauseMusic()
    {
        if (musicAudioSource != null) musicAudioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicAudioSource != null) musicAudioSource.UnPause();
    }

    private void UpdateMixerVolume()
    {
        if (mainMixer != null)
        {
            float volume = isMuted ? -80f : 0f;
            mainMixer.SetFloat(MUSIC_VOLUME_PARAM, volume);
            mainMixer.SetFloat(SFX_VOLUME_PARAM, volume);
        }
    }

    private void UpdateMuteButtonVisuals()
    {
        if (muteButtonText != null) muteButtonText.text = isMuted ? "Audio Off" : "Audio On";
        if (muteButtonImage != null) muteButtonImage.sprite = isMuted ? audioOffSprite : audioOnSprite;
    }
}