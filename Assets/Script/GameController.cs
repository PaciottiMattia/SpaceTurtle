using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;

public class GameController : MonoBehaviour
{
    [Header("--- GESTIONE RECORD & UI ---")]
    public TextMeshProUGUI testoPunteggioInGioco;
    public TextMeshProUGUI testoRecordMigliore;
    public GameObject pannelloGameOver;
    public TextMeshProUGUI testoPunteggioFinale;

    // Variabili di stato
    public static bool gameover;
    public bool stopSpawns = false;

    private float punteggioAttuale = 0;
    private int recordMigliore = 0;

    [Header("--- AUDIO ---")]
    public AudioClip navicellaPassaggioSound;
    [Range(0f, 1f)] public float volumeNavicelle = 0.3f;
    private AudioSource audioSource;

    [Header("--- SPAWNERS & PREFABS ---")]
    public GameObject satellitePrefab;
    public GameObject meteoritePrefab;
    public GameObject cuorePrefab;
    public GameObject scarpePrefab;
    public GameObject scudoPrefab;

    // --- BILANCIAMENTO SPAWN ---
    float spawnRateSatellite = 2.5f;
    float spawnRateMeteorite = 5.0f;
    float spawnRateCuore = 40f;
    float spawnRateScarpe = 30f;
    float spawnRateScudo = 22f;

    float minRateSatellite = 0.8f;
    float minRateMeteorite = 2.0f;
    
    // Variabili interne Timer
    float spawnTimerSatellite;
    float spawnTimerMeteorite;
    float spawnTimerCuore;
    float spawnTimerScarpe;
    float spawnTimerScudo;

    // Variabile Sciame
    public GameObject meteoriteSciamePrefab;
    private bool sciameMeteoriteAttivo = false;
    private float intervalloSciameMeteorite;
    private float timerIntervalloSciameMeteorite;
    private float spawnRateSciameMeteorite = 1.2f;
    private float spawnTimerSciameMeteorite;
    private int numeroSciamiMeteorite;

    public GameObject immagineAvvisoSciame;
    private float timerAvvisoSciame;
    private bool avvisoSciameAttivo = false;

    // Variabile Passaggio Navicelle
    public GameObject navicellaPrefab;
    public float tempoSpawnNavicelle = 100f;
    public float limiteXNavicelle = 8f;
    public float limiteYNavicelle = 4f;
    public float tempoPassaggioNavicelle = 10f;
    private bool passaggioNavicelleAttivo = false;
    private float timerIntervalloNavicelle;
    
    // MODIFICA QUI: Tempi Navicelle
    public float intervalloMinNavicelle = 40f; // Era 60
    public float intervalloMaxNavicelle = 80f;  // Era 120
    
    private int navicelleDaSpawnare;
    private int navicelleSpawnate;
    public float spawnTimerNavicelle;
    private bool stoFermandoNavicelle = false;

    public GameObject SegnaleNavicelle;
    private float timerAvvisoNavicelle;
    private bool avvisoNavicelleAttivo = false;
    private bool avvisoNavicelleMostrato = false;

    // Variabili Raggio Traiettoria
    public GameObject raggioOrizzontalePrefab;
    public GameObject raggioVerticalePrefab;
    public float durataRaggio = 0.5f;

    async void Start()
    {
        gameover = false;
        stopSpawns = false;
        punteggioAttuale = 0;

        spawnTimerSatellite = 0;
        spawnTimerMeteorite = 0;
        spawnTimerCuore = 0;
        spawnTimerScarpe = 0;
        spawnTimerScudo = 0;

        // *** MODIFICA FONDAMENTALE ***
        // Adesso partono entrambi tra 15 e 30 secondi.
        // Chi pesca il numero più basso parte per primo!
        timerIntervalloSciameMeteorite = Random.Range(15f, 30f); // Era 40-90! Troppo lento!
        timerIntervalloNavicelle = Random.Range(15f, 30f); 

        // SETUP AUDIO
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = navicellaPassaggioSound;
        audioSource.playOnAwake = false;

        if (CloudSaveManager.instance != null)
        {
            recordMigliore = await CloudSaveManager.instance.CaricaRecord();
            Debug.Log("Record caricato all'avvio: " + recordMigliore);
        }
    }

    void Update()
    {
        if (!gameover && !stopSpawns)
        {
            // 1. CALCOLO PUNTEGGIO E DIFFICOLTA'
            punteggioAttuale += Time.deltaTime;
            AggiornaDifficolta();

            if (testoPunteggioInGioco != null)
            {
                testoPunteggioInGioco.text = ((int)punteggioAttuale).ToString();
            }

            // CONTROLLO SOVRAPPOSIZIONE EVENTI
            bool navicelleInAzione = avvisoNavicelleAttivo || passaggioNavicelleAttivo;
            bool sciameInAzione = avvisoSciameAttivo || sciameMeteoriteAttivo;

            // --- LOGICA DI SPAWN GENERALE ---
            spawnTimerSatellite += Time.deltaTime;
            spawnTimerMeteorite += Time.deltaTime;
            spawnTimerCuore += Time.deltaTime;
            spawnTimerScarpe += Time.deltaTime;
            spawnTimerScudo += Time.deltaTime;

            // ====================================================================
            // LOGICA SCIAME
            // ====================================================================
            if (avvisoSciameAttivo)
            {
                timerAvvisoSciame -= Time.deltaTime;
                immagineAvvisoSciame.SetActive(Mathf.Floor(timerAvvisoSciame * 2f) % 2 == 0);
                if (timerAvvisoSciame <= 0f)
                {
                    immagineAvvisoSciame.SetActive(false);
                    avvisoSciameAttivo = false;
                    sciameMeteoriteAttivo = true;
                    numeroSciamiMeteorite = Random.Range(5, 16);
                    spawnTimerSciameMeteorite = 0f;
                }
            }
            else if (sciameMeteoriteAttivo)
            {
                spawnTimerSciameMeteorite += Time.deltaTime;
                if (spawnTimerSciameMeteorite >= spawnRateSciameMeteorite && numeroSciamiMeteorite > 0)
                {
                    SpawnSciameMeteorite();
                }
                if (numeroSciamiMeteorite <= 0)
                {
                    sciameMeteoriteAttivo = false;
                    // RESET SCIAME: Lo facciamo tornare prima!
                    timerIntervalloSciameMeteorite = Random.Range(30f, 60f); // Era 40-100
                    spawnTimerSatellite = 0f;
                    spawnTimerMeteorite = 0f;
                }
            }
            else
            {
                // Scende solo se le navicelle non rompono le scatole
                if (!navicelleInAzione) 
                {
                    timerIntervalloSciameMeteorite -= Time.deltaTime;
                    if (timerIntervalloSciameMeteorite <= 1f)
                    {
                        avvisoSciameAttivo = true;
                        timerAvvisoSciame = 3f;
                        StartCoroutine(AttivaImmagineConRitardo());
                    }
                }
            }

            // ====================================================================
            // LOGICA NAVICELLE
            // ====================================================================
            if (avvisoNavicelleAttivo)
            {
                timerAvvisoNavicelle -= Time.deltaTime;
                SegnaleNavicelle.SetActive(Mathf.Floor(timerAvvisoNavicelle * 2f) % 2 == 0);
                if (timerAvvisoNavicelle <= 0f)
                {
                    SegnaleNavicelle.SetActive(false);
                    avvisoNavicelleAttivo = false;
                    AvviaPassaggioNavicelle();
                }
            }
            else if (passaggioNavicelleAttivo)
            {
                if (navicelleSpawnate < navicelleDaSpawnare)
                {
                    spawnTimerNavicelle += Time.deltaTime;
                    if (spawnTimerNavicelle >= tempoSpawnNavicelle)
                    {
                        SpawnNavicella();
                        spawnTimerNavicelle = 0; 
                        navicelleSpawnate++;
                    }
                }
                else if (!stoFermandoNavicelle)
                {
                    StartCoroutine(AttendiEChiudiNavicelle());
                }
            }
            else
            {
                // Scende solo se lo sciame non rompe le scatole
                if (!sciameInAzione)
                {
                    if (timerIntervalloNavicelle > 0)
                    {
                        timerIntervalloNavicelle -= Time.deltaTime;
                    }
                    else if (!avvisoNavicelleMostrato)
                    {
                        avvisoNavicelleAttivo = true;
                        timerAvvisoNavicelle = 3f;
                        SegnaleNavicelle.SetActive(true);
                        avvisoNavicelleMostrato = true;
                    }
                }
            }

            // Durante gli eventi speciali i nemici normali non spawnano
            if (!passaggioNavicelleAttivo && !sciameMeteoriteAttivo)
            {
                if (spawnTimerMeteorite >= spawnRateMeteorite) SpawnMeteorite();
                if (spawnTimerSatellite >= spawnRateSatellite) SpawnSatellite();
            }

            if (spawnTimerCuore >= spawnRateCuore) SpawnCuore();
            if (spawnTimerScarpe >= spawnRateScarpe) SpawnScarpe();
            if (spawnTimerScudo >= spawnRateScudo) SpawnScudo();
        }
    }

    void AggiornaDifficolta()
    {
        float fattoreRiduzione = Time.deltaTime * 0.01f; 

        if (spawnRateSatellite > minRateSatellite)
             spawnRateSatellite -= fattoreRiduzione;

        if (spawnRateMeteorite > minRateMeteorite)
             spawnRateMeteorite -= fattoreRiduzione;
    }

    public void BloccaSpawnsImmediato()
    {
        stopSpawns = true;
        if (immagineAvvisoSciame != null) immagineAvvisoSciame.SetActive(false);
        if (SegnaleNavicelle != null) SegnaleNavicelle.SetActive(false);
        avvisoSciameAttivo = false;
        avvisoNavicelleAttivo = false;

        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
    }

    public async void GestisciGameOver()
    {
        if (gameover) return;
        gameover = true;

        if (pannelloGameOver != null) pannelloGameOver.SetActive(true);

        int punteggioFinale = (int)punteggioAttuale;

        if (punteggioFinale > recordMigliore)
        {
            recordMigliore = punteggioFinale;
            if (testoRecordMigliore != null) testoRecordMigliore.text = "NUOVO RECORD: " + recordMigliore;
            if (CloudSaveManager.instance != null) await CloudSaveManager.instance.SalvaRecord(recordMigliore);
        }
        else
        {
            if (testoRecordMigliore != null) testoRecordMigliore.text = "Record Migliore: " + recordMigliore;
        }
    }

    // --- FUNZIONI DI SPAWN ---
    void SpawnSatellite()
    {
        spawnTimerSatellite = 0; 
        Vector2 spawnPos = new Vector2(Random.Range(-1.8f, 1.7f), 5.348f);
        GameObject satellite = Instantiate(satellitePrefab, spawnPos, Quaternion.identity);
        satellite.tag = "Satellite";
    }
    void SpawnMeteorite()
    {
        spawnTimerMeteorite = 0;
        Vector2 spawnPos = new Vector2(Random.Range(-1.765f, 1.63f), 6.01f);
        Quaternion rotazione = Quaternion.Euler(0f, 0f, 47.8f);
        GameObject meteorite = Instantiate(meteoritePrefab, spawnPos, rotazione);
        meteorite.tag = "Meteorite";
    }
    void SpawnCuore()
    {
        spawnTimerCuore = 0;
        Vector2 spawnPos = new Vector2(Random.Range(-1.765f, 1.63f), 6.01f);
        GameObject cuoreSpawn = Instantiate(cuorePrefab, spawnPos, Quaternion.identity);
        cuoreSpawn.tag = "CuoreSpawn";
    }
    void SpawnScarpe()
    {
        spawnTimerScarpe = 0;
        Vector2 spawnPos = new Vector2(Random.Range(-1.859f, 1.843f), 6.01f);
        GameObject scarpe = Instantiate(scarpePrefab, spawnPos, Quaternion.identity);
        scarpe.tag = "Scarpe";
    }
    void SpawnScudo()
    {
        spawnTimerScudo = 0;
        Vector2 spawnPos = new Vector2(Random.Range(-1.859f, 1.843f), 6.01f);
        GameObject scudo = Instantiate(scudoPrefab, spawnPos, Quaternion.identity);
        scudo.tag = "Scudo";
    }
    void SpawnSciameMeteorite()
    {
        spawnTimerSciameMeteorite = 0; 
        Vector2 spawnPos = new Vector2(Random.Range(-1.806f, 1.85f), 6.01f);
        Quaternion rotazione = Quaternion.Euler(0f, 0f, 52f);
        GameObject meteoriteSciame = Instantiate(meteoriteSciamePrefab, spawnPos, rotazione);
        meteoriteSciame.tag = "MeteoriteSciame";
        numeroSciamiMeteorite--;
    }

    public void AvviaPassaggioNavicelle()
    {
        passaggioNavicelleAttivo = true;
        stoFermandoNavicelle = false;
        navicelleDaSpawnare = Random.Range(3, 8);
        navicelleSpawnate = 0;
        tempoSpawnNavicelle = Random.Range(1.5f, 3.0f); 
        spawnTimerNavicelle = 0f;

        spawnTimerSatellite = 0f;
        spawnTimerMeteorite = 0f;

        if (navicellaPassaggioSound != null && audioSource != null)
        {
            audioSource.clip = navicellaPassaggioSound;
            audioSource.volume = volumeNavicelle;
            audioSource.Play();
        }
    }

    IEnumerator AttendiEChiudiNavicelle()
    {
        stoFermandoNavicelle = true;
        yield return new WaitForSeconds(4f);
        FermaPassaggioNavicelle();
    }

    public void FermaPassaggioNavicelle()
    {
        passaggioNavicelleAttivo = false;
        stoFermandoNavicelle = false;
        // QUI IMPOSTIAMO IL PROSSIMO INTERVALLO NAVICELLE
        timerIntervalloNavicelle = Random.Range(intervalloMinNavicelle, intervalloMaxNavicelle);
        spawnTimerSatellite = 0f;
        spawnTimerMeteorite = 0f;
        spawnTimerNavicelle = 0f;
        avvisoNavicelleMostrato = false;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void SpawnNavicella()
    {
        int tipoSpawn = Random.Range(0, 4);
        Vector2 spawnPos = Vector2.zero;
        GameObject raggio = null;
        Vector2 direzione = Vector2.zero;

        switch (tipoSpawn)
        {
            case 0: spawnPos = new Vector2(-13.366f, Random.Range(-4.514f, 4.4838f)); raggio = Instantiate(raggioOrizzontalePrefab, spawnPos, Quaternion.identity); direzione = Vector2.right; break;
            case 1: spawnPos = new Vector2(13.64f, Random.Range(-4.514f, 4.4838f)); raggio = Instantiate(raggioOrizzontalePrefab, new Vector2(spawnPos.x - 16.706f, spawnPos.y), Quaternion.identity); direzione = Vector2.left; break;
            case 2: spawnPos = new Vector2(Random.Range(-1.3356f, 1.309f), 15.6567f); raggio = Instantiate(raggioVerticalePrefab, new Vector2(spawnPos.x, spawnPos.y - 20.524395f), Quaternion.Euler(0, 0, 90)); direzione = Vector2.down; break;
            case 3: spawnPos = new Vector2(Random.Range(-1.3356f, 1.309f), -15.667f); raggio = Instantiate(raggioVerticalePrefab, spawnPos, Quaternion.Euler(0, 0, 90)); direzione = Vector2.up; break;
        }
        StartCoroutine(DisattivaRaggio(raggio));
        GameObject navicella = Instantiate(navicellaPrefab, spawnPos, Quaternion.identity);
        navicella.tag = "Navicella";
        MovimentoNavicella movimentoNavicella = navicella.GetComponent<MovimentoNavicella>();
        if (movimentoNavicella != null) movimentoNavicella.direzione = direzione;
    }

    System.Collections.IEnumerator AttivaImmagineConRitardo()
    {
        yield return new WaitForSeconds(0.02f);
        immagineAvvisoSciame.SetActive(true);
    }
    IEnumerator DisattivaRaggio(GameObject raggio)
    {
        if (raggio != null)
        {
            SpriteRenderer spriteRenderer = raggio.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    spriteRenderer.enabled = false; yield return new WaitForSeconds(0.3f);
                    spriteRenderer.enabled = true; yield return new WaitForSeconds(0.3f);
                }
                Destroy(raggio);
            }
            else { Destroy(raggio); }
        }
    }
}