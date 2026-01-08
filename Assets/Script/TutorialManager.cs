using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("--- RIFERIMENTI UI ---")]
    public GameObject pannelloTutorial;
    public TextMeshProUGUI testoTutorial;
    public Slider barraTempoLettura; 
    public Button esciButton;
    public GameObject immagineJarvis; 

    [Header("--- AUDIO ---")]
    public AudioClip navicellaSound; // ### NUOVO ###
    [Range(0f, 1f)] public float volumeNavicelle = 0.3f; // ### NUOVO ### Cursore volume
    private AudioSource audioSource; // ### NUOVO ###

    [Header("--- CONTENUTO TUTORIAL ---")]
    [TextArea(5, 10)]
    public string[] testiDegliStep;
    public float[] tempoPerLeggere; 

    [Header("--- PREFABS ---")]
    public GameObject pianetaPrefab;
    public GameObject scudoPrefab;
    public GameObject scarpePrefab;
    public GameObject satellitePrefab;
    public GameObject meteoritePrefab;
    public GameObject sciameMeteoritePrefab;
    public GameObject navicellaPrefab;
    public GameObject raggioAvvisoPrefab; 

    [Header("--- POSIZIONI DI SPAWN ---")]
    public Transform spawnPianeta; 
    public Transform[] spawnScudi; 
    public Transform spawnScarpe;
    public Transform[] spawnSatelliti; 
    public Transform[] spawnMeteoriti; 
    public Transform spawnSciameStart; 
    public Transform spawnNavicellaStart; 

    [Header("--- IMPOSTAZIONI ---")]
    public string nomeScenaMenu = "SchermataIniziale";

    private int stepCorrente = 0;

    void Start()
    {
        GameController.gameover = false;

        barraTempoLettura.gameObject.SetActive(false);
        pannelloTutorial.SetActive(false);
        esciButton.gameObject.SetActive(false);
        
        if(immagineJarvis != null) immagineJarvis.SetActive(false);

        // ### NUOVO ### Setup Audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true; // Loop attivo nel caso il suono sia breve

        StartCoroutine(EseguiSequenzaTutorial());
    }

    IEnumerator EseguiSequenzaTutorial()
    {
        for (stepCorrente = 0; stepCorrente < testiDegliStep.Length; stepCorrente++)
        {
            // --- FASE A: LETTURA ---
            Time.timeScale = 0f;
            pannelloTutorial.SetActive(true);
            barraTempoLettura.gameObject.SetActive(true);
            
            if(immagineJarvis != null) immagineJarvis.SetActive(true);
            
            testoTutorial.text = testiDegliStep[stepCorrente];

            float tempoTotale = (tempoPerLeggere.Length > stepCorrente) ? tempoPerLeggere[stepCorrente] : 5f;
            float timer = tempoTotale;
            barraTempoLettura.value = 1; 

            while (timer > 0)
            {
                timer -= Time.unscaledDeltaTime;
                barraTempoLettura.value = timer / tempoTotale;
                yield return null;
            }

            // --- FASE B: PRATICA ---
            pannelloTutorial.SetActive(false);
            barraTempoLettura.gameObject.SetActive(false);
            
            if(immagineJarvis != null) immagineJarvis.SetActive(false);
            
            Time.timeScale = 1f; 

            yield return StartCoroutine(EseguiCoreografiaStep(stepCorrente));
        }

        FineTutorial();
    }

    IEnumerator EseguiCoreografiaStep(int step)
    {
        switch (step)
        {
            case 0: // STEP 1: Solo Salto
                yield return new WaitForSeconds(6f); 
                break;

            case 1: // STEP 2: Pianeta
                if (pianetaPrefab && spawnPianeta)
                    Instantiate(pianetaPrefab, spawnPianeta.position, Quaternion.identity);
                
                yield return new WaitForSeconds(9f);
                break;

            case 2: // STEP 3: SCUDI E PERICOLI
                if (scudoPrefab && spawnScudi.Length > 0) Instantiate(scudoPrefab, spawnScudi[0].position, Quaternion.identity);
                yield return new WaitForSeconds(5f); 
                if (scudoPrefab && spawnScudi.Length > 1) Instantiate(scudoPrefab, spawnScudi[1].position, Quaternion.identity);
                yield return new WaitForSeconds(5f); 
                if (scudoPrefab && spawnScudi.Length > 2) Instantiate(scudoPrefab, spawnScudi[2].position, Quaternion.identity);
                
                yield return new WaitForSeconds(2f); 

                if (satellitePrefab && spawnSatelliti.Length > 0)
                {
                    Instantiate(satellitePrefab, spawnSatelliti[0].position, Quaternion.identity);
                    yield return new WaitForSeconds(2.5f); 
                    if (spawnSatelliti.Length > 1) 
                        Instantiate(satellitePrefab, spawnSatelliti[1].position, Quaternion.identity);
                    yield return new WaitForSeconds(2.5f); 
                    if (spawnSatelliti.Length > 2) 
                        Instantiate(satellitePrefab, spawnSatelliti[2].position, Quaternion.identity);
                }

                yield return new WaitForSeconds(1.5f); 

                if (meteoritePrefab && spawnMeteoriti.Length > 0)
                {
                    Instantiate(meteoritePrefab, spawnMeteoriti[0].position, Quaternion.Euler(0f, 0f, 47.8f));
                    yield return new WaitForSeconds(2f); 
                    if (spawnMeteoriti.Length > 1)
                        Instantiate(meteoritePrefab, spawnMeteoriti[1].position, Quaternion.Euler(0f, 0f, 47.8f));
                    yield return new WaitForSeconds(2f); 
                    if (spawnMeteoriti.Length > 2)
                        Instantiate(meteoritePrefab, spawnMeteoriti[2].position, Quaternion.Euler(0f, 0f, 47.8f));
                }
                
                yield return new WaitForSeconds(6f); 
                break;

            case 3: // STEP 4: SCARPE, SCIAME E NAVICELLA
                if (scarpePrefab && spawnScarpe)
                    Instantiate(scarpePrefab, spawnScarpe.position, Quaternion.identity);
                
                yield return new WaitForSeconds(2.5f); 

                if (sciameMeteoritePrefab && spawnSciameStart)
                {
                    for(int i=0; i<7; i++) { 
                        Vector3 pos = spawnSciameStart.position + new Vector3(Random.Range(-2.2f, 2.2f), 0, 0);
                        Instantiate(sciameMeteoritePrefab, pos, Quaternion.Euler(0f, 0f, 52f));
                        yield return new WaitForSeconds(0.3f);
                    }
                }

                yield return new WaitForSeconds(3f);

                if (navicellaPrefab && spawnNavicellaStart && raggioAvvisoPrefab)
                {
                    // 1. Raggio Avviso
                    GameObject raggio = Instantiate(raggioAvvisoPrefab, spawnNavicellaStart.position, Quaternion.identity);
                    yield return new WaitForSeconds(1.5f);
                    Destroy(raggio);

                    // 2. Spawn Navicella
                    GameObject nav = Instantiate(navicellaPrefab, spawnNavicellaStart.position, Quaternion.identity);
                    MovimentoNavicella scriptNav = nav.GetComponent<MovimentoNavicella>();
                    if (scriptNav) scriptNav.direzione = Vector2.right; 

                    // ### NUOVO ### PLAY SUONO NAVICELLA
                    if (audioSource != null && navicellaSound != null)
                    {
                        audioSource.clip = navicellaSound;
                        audioSource.volume = volumeNavicelle;
                        audioSource.Play();
                    }

                    // 3. Aspettiamo che esca dallo schermo (circa 4 secondi)
                    yield return new WaitForSeconds(4f);

                    // ### NUOVO ### STOP SUONO
                    if (audioSource != null) audioSource.Stop();
                }

                yield return new WaitForSeconds(1f); // Piccola pausa finale prima di chiudere
                break;
        }
    }

    void FineTutorial()
    {
        Time.timeScale = 0f;
        pannelloTutorial.SetActive(true);
        
        // Spegne l'audio se per caso era rimasto acceso
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();

        if(immagineJarvis != null) immagineJarvis.SetActive(true);
        
        testoTutorial.text = "Tutorial completato! Ora sei pronto per la vera sfida. In bocca al lupo, Splint!";
        
        esciButton.gameObject.SetActive(true);
        esciButton.GetComponentInChildren<TextMeshProUGUI>().text = "Torna al Menu";
        esciButton.onClick.RemoveAllListeners();
        esciButton.onClick.AddListener(TornaAlMenu);
    }

    void TornaAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeScenaMenu);
    }
}