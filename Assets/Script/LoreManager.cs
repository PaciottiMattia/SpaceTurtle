
using UnityEngine;
using UnityEngine.UI; // Necessario per Immagine
using UnityEngine.SceneManagement; // Necessario per cambiare scena
using TMPro; // Necessario per TextMeshPro
using System.Collections; // Necessario per le Coroutine (IEnumerator)
using UnityEngine.Audio; // Necessario per AudioSource

// --- CLASSI PER ORGANIZZARE I DATI ---
// (Queste NON sono script separati, restano in questo file)

/**
 * Questa classe raggruppa UN testo e UN audio.
 * [System.Serializable] la rende visibile nell'Inspector di Unity.
 */
[System.Serializable]
public class DialogueClip
{
    [TextArea(3, 10)] // Rende il campo di testo più grande nell'Inspector
    public string dialogueText; // Il testo da mostrare
    public AudioClip dialogueAudio; // Il file audio da riprodurre
    
    // Usato solo se non fornisci un file audio (come backup)
    public float tempoSenzaAudio = 3f; 
}

/**
 * Questa classe raggruppa UNA SCENA INTERA: 
 * 1 sfondo e UN ELENCO di dialoghi/audio per quello sfondo.
 */
[System.Serializable]
public class LoreSceneStep
{
    public Sprite backgroundSprite; // Lo sfondo per questa scena
    public DialogueClip[] dialogues; // Un array di dialoghi per questo sfondo
}


// --- SCRIPT PRINCIPALE ---

/**
 * Questo è lo script principale che devi allegare al tuo
 * "LoreManager_Oggetto" nella scena.
 */
public class LoreManager : MonoBehaviour
{
    [Header("Riferimenti UI (Trascina dalla Hierarchy)")]
    public Image immagineSfondo;
    public Image immagineJarvis;
    public GameObject pannelloDialogo;
    public TextMeshProUGUI testoDialogo;
    public AudioSource voceJarvisAudioSource; // L'AudioSource per la voce

    [Header("Contenuto Lore (Raggruppato)")]
    // Questo è l'elenco principale che compilerai nell'Inspector
    public LoreSceneStep[] loreSteps; 

    [Header("Impostazioni di Uscita")]
    public string nomeScenaDiGioco = "ScenaDiGioco"; // Scrivi qui il nome ESATTO della tua scena di gioco

    // Variabili private per la logica
    private bool staMostrandoLore = true;
    private bool skipPremuto = false;

    
    void Start()
    {
        // All'inizio, mostra solo lo sfondo. Jarvis e il pannello appaiono col primo dialogo.
        immagineJarvis.gameObject.SetActive(false);
        pannelloDialogo.SetActive(false);

        // Se non hai trascinato l'AudioSource nell'Inspector,
        // lo script prova a trovarlo sullo stesso oggetto.
        if (voceJarvisAudioSource == null)
        {
            voceJarvisAudioSource = GetComponent<AudioSource>();
        }

        // Avvia la sequenza principale
        StartCoroutine(EseguiSequenzaLore());
    }

    /**
     * Questa Coroutine gestisce l'intera sequenza della lore.
     */
    IEnumerator EseguiSequenzaLore()
    {
        staMostrandoLore = true;

        // Loop esterno: scorre le SCENE (i tuoi 3 sfondi/capitoli)
        foreach (LoreSceneStep step in loreSteps)
        {
            // Imposta lo sfondo per questa scena
            immagineSfondo.sprite = step.backgroundSprite;
            
            // Mostra Jarvis e il pannello
            immagineJarvis.gameObject.SetActive(true);
            pannelloDialogo.SetActive(true);

            // Loop interno: scorre i DIALOGHI all'interno di QUESTA scena
            foreach (DialogueClip clip in step.dialogues)
            {
                // Mostra il testo
                testoDialogo.text = clip.dialogueText;

                // Riproduci l'audio (se c'è)
                if (clip.dialogueAudio != null)
                {
                    voceJarvisAudioSource.clip = clip.dialogueAudio;
                    voceJarvisAudioSource.Play();

                    // Aspetta la fine dell'audio O lo skip
                    while (voceJarvisAudioSource.isPlaying)
                    {
                        if (skipPremuto)
                        {
                            voceJarvisAudioSource.Stop();
                            break;
                        }
                        yield return null; // Aspetta il prossimo frame
                    }
                }
                else
                {
                    // Se non c'è audio, aspetta un tempo fisso (es. 3 secondi)
                    float timer = 0;
                    while (timer < clip.tempoSenzaAudio)
                    {
                         if (skipPremuto) break;
                         timer += Time.deltaTime;
                         yield return null;
                    }
                }

                if (skipPremuto) break; // Esci dal loop dei dialoghi

                // Piccola pausa tra le singole battute
                yield return new WaitForSeconds(0.2f);
            }

            if (skipPremuto) break; // Esci dal loop delle scene

            // Piccola pausa tra un cambio di sfondo e l'altro
            yield return new WaitForSeconds(0.5f);
        }

        // Finito tutto (o saltato), carica il gioco
        CaricaScenaDiGioco();
    }

    /**
     * Controlla l'input del giocatore per saltare
     */
    void Update()
    {
        // Se si preme il mouse/tocca lo schermo, imposta skipPremuto = true
        if (Input.GetMouseButtonDown(0) && staMostrandoLore && !skipPremuto)
        {
            skipPremuto = true;
            Debug.Log("Skip richiesto!");
        }
    }

    /**
     * Carica la scena di gioco principale
     */
    public void CaricaScenaDiGioco()
    {
        staMostrandoLore = false;
        SceneManager.LoadScene(nomeScenaDiGioco);
    }
}