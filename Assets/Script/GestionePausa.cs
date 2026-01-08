using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionePausa : MonoBehaviour
{
    public GameObject pulsantePausa;
    public GameObject pulsanteRiprendi;
    public GameObject pannelloPausa;
    public bool giocoInPausa = false;

    public Tartaruga tartaruga;
    // public GameObject[] travi; // (Se non lo usi puoi toglierlo)

    public bool controlloRiprendi = false;

    public void PausaGioco()
    {
        Time.timeScale = 0f;
        pulsantePausa.SetActive(false);
        pulsanteRiprendi.SetActive(true);
        
        // 1. Attiviamo il pannello (Ora i bottoni dentro diventano "trovabili" da Unity)
        pannelloPausa.SetActive(true);
        giocoInPausa = true;

        if (tartaruga != null && tartaruga.inCollisioneConTrave)
        {
            controlloRiprendi = true;
        }
        else
        {
            controlloRiprendi = false;
        }

        // 2. Gestione Audio
        if (AudioSettingsManager.instance != null)
        {
            // Mette in pausa la musica
            AudioSettingsManager.instance.PauseMusic();

            // *** RIGA FONDAMENTALE AGGIUNTA ***
            // Dice all'Audio Manager: "Il menu è aperto, cerca e collega subito il bottone Mute!"
            AudioSettingsManager.instance.RefreshButtonReference();
        }
    }

    public void RiprendiGioco()
    {
        Time.timeScale = 1f;
        pulsantePausa.SetActive(true);
        pulsanteRiprendi.SetActive(false);
        pannelloPausa.SetActive(false);
        giocoInPausa = false;

        // Riprende la musica
        if (AudioSettingsManager.instance != null)
        {
            AudioSettingsManager.instance.ResumeMusic();
        }
    }

    public void VaiAllaSchermataIniziale()
    {
        Time.timeScale = 1f;
        
        // L'AudioManager gestirà il cambio di traccia automaticamente caricando la scena
        SceneManager.LoadScene("SchermataIniziale");
    }

    void Start()
    {
        pulsanteRiprendi.SetActive(false);
        pannelloPausa.SetActive(false);
        // Non serve cercare l'audio manager qui, usiamo 'instance'
    }

    public bool GetControlloRiprendi()
    {
        return controlloRiprendi;
    }
}