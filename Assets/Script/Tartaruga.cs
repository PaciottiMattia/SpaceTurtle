using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class Tartaruga : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("--- RIFERIMENTI VISIVI ---")]
    public GameObject auraScudoVisiva; 

    [Header("--- IMPOSTAZIONI MOVIMENTO ---")]
    public float velocitaCadutaLenta = 0.7f;
    public float forzaSalto = 4.77f;
    public float limiteSuperioreY = 5f;
    public float forzaVolo = 5f;
    public float tempoVoloMassimo = 5f;

    [Header("--- STATO DEL GIOCO ---")]
    public bool inCollisioneConTrave = false;
    public bool eMorto = false; 

    // Gestione Vite
    public GameObject[] cuori;
    public int vite = 3; 
    public GameObject messaggioCuoriAlMassimo;

    // Gestione Salti
    private int numeroSaltiEffettuati = 0; 
    private int numeroSaltiMassimi = 2; 
    private bool isAirborne = false;

    // Power-Up SCARPE
    [HideInInspector] public bool scarpeRaccolte = false;
    [HideInInspector] public GameObject scarpe; 
    private bool staVolando = false;
    [HideInInspector] public float tempoVoloAttuale = 0f;
    private Vector2 direzioneVolo; 
    private int numeroSaltiDopoVolo = 2; 

    // Scudo
    [HideInInspector] public int numeroScudi = 0;
    public int scudiMassimi = 3;
    [HideInInspector] public bool invincibile = false;
    public float tempoInvincibilita = 10f;
    [HideInInspector] public float tempoInvincibilitaAttuale = 0f;

    // Tecniche
    private bool toccoSuUI = false;
    public float numeroRighe = 10f;
    private float altezzaRiga; 
    private float posizioneQuartaRiga;

    private Animator animator;
    public GestionePausa gestionePausa; 

    [Header("--- AUDIO GENERALE ---")]
    public AudioClip jumpSoundClip;         
    public AudioClip hurtSoundClip;         
    public AudioClip gameOverSoundClip;     
    public AudioClip collectHeartClip;      
    public AudioClip collectShieldClip;     
    public AudioClip shieldLoopClip;        
    [Range(0f, 1f)] public float volumeScudo = 0.5f; 

    [Header("--- AUDIO SCARPE (VOLO) ---")]
    public AudioClip scarpeCollectClip;    // 1. Suono quando prendi le scarpe (DING)
    [Range(0f, 1f)] public float volumeRaccoltaScarpe = 1f; // Slider volume raccolta

    public AudioClip flightLoopClip;       // 2. Suono mentre voli (RAZZO)
    [Range(0f, 1f)] public float volumeVolo = 0.5f;         // Slider volume volo
    
    // Sorgenti Audio
    private AudioSource audioSourceSFX;     // Per i suoni singoli (salto, raccolta, danno)
    private AudioSource audioSourceLoop;    // Per lo scudo
    private AudioSource audioSourceFlight;  // Per il volo (nuovo canale dedicato)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        altezzaRiga = Screen.height / numeroRighe;
        posizioneQuartaRiga = altezzaRiga * 9f;

        AggiornaCuori(); 

        // 1. Setup Audio SFX (Effetti singoli)
        audioSourceSFX = gameObject.AddComponent<AudioSource>();
        
        // 2. Setup Loop Scudo
        audioSourceLoop = gameObject.AddComponent<AudioSource>();
        audioSourceLoop.loop = true; 
        audioSourceLoop.volume = volumeScudo; 

        // 3. Setup Loop Volo
        audioSourceFlight = gameObject.AddComponent<AudioSource>();
        audioSourceFlight.loop = true;          // Deve ripetersi
        audioSourceFlight.clip = flightLoopClip;
        audioSourceFlight.volume = volumeVolo;
        audioSourceFlight.playOnAwake = false;

        if (auraScudoVisiva != null) auraScudoVisiva.SetActive(false);

        if (animator != null) animator.SetBool("GuardaDestra", true);
    }

    void Update()
    {
        // 1. FINE GIOCO (Caduta)
        if (transform.position.y <= -5.34f)
        {
            GameController gc = FindObjectOfType<GameController>();
            if (gc != null) gc.GestisciGameOver();
            gameObject.SetActive(false);
            return; 
        }

        // 2. LOGICA GIOCO
        if (!GameController.gameover && (gestionePausa == null || !gestionePausa.giocoInPausa) && !eMorto)
        {
            if (animator != null) animator.SetBool("InTrave", inCollisioneConTrave);

            // --- GESTIONE VOLO (SCARPE) ---
            if (staVolando && tempoVoloAttuale > 0)
            {
                // GESTIONE AUDIO VOLO LOOP
                // Se il suono non sta andando e abbiamo una clip, fallo partire
                if (!audioSourceFlight.isPlaying && flightLoopClip != null)
                {
                    audioSourceFlight.volume = volumeVolo; // Aggiorna volume se cambiato in gioco
                    audioSourceFlight.Play();
                }

                // GESTIONE MOVIMENTO VOLO
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        direzioneVolo = touch.deltaPosition.normalized;
                        rb.linearVelocity = direzioneVolo * forzaVolo;
                        if (animator != null)
                        {
                            if (direzioneVolo.x > 0.1f) animator.SetBool("GuardaDestra", true);
                            else if (direzioneVolo.x < -0.1f) animator.SetBool("GuardaDestra", false);
                        }
                    }
                }
                else rb.linearVelocity = Vector2.zero;
                tempoVoloAttuale -= Time.deltaTime;
            }
            // --- FINE VOLO ---
            else
            {
                // STOP AUDIO VOLO
                if (audioSourceFlight.isPlaying)
                {
                    audioSourceFlight.Stop();
                }

                if (scarpeRaccolte)
                {
                    // Fine del periodo di volo
                    staVolando = false;
                    scarpeRaccolte = false;
                    numeroSaltiEffettuati = 0;
                    numeroSaltiMassimi = numeroSaltiDopoVolo;
                    
                    if (animator != null) animator.SetBool("StaVolando", false); // Spegni animazione
                }

                // --- SALTO NORMALE ---
                if (Input.touchCount > 0)
                {
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            toccoSuUI = false;
                            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("UI")) toccoSuUI = true;
                            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) toccoSuUI = true;

                            if (!toccoSuUI && touch.position.y <= posizioneQuartaRiga)
                            {
                                if (numeroSaltiEffettuati < numeroSaltiMassimi)
                                {
                                    float touchXPosition = touch.position.x;
                                    bool saltoADestra = touchXPosition < Screen.width / 2;
                                    EseguiSalto(saltoADestra); 
                                    numeroSaltiEffettuati++;
                                    isAirborne = true; 
                                }
                            }
                        }
                    }
                }
            }
            // Reset flag trave al tocco
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began) inCollisioneConTrave = false;
                }
            }

            if (transform.position.y > limiteSuperioreY)
            {
                transform.position = new Vector2(transform.position.x, limiteSuperioreY);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }

            if (vite <= 0) StartCoroutine(EseguiMorte());
        }
        else
        {
            // Se gioco in pausa o finito, zittisci il motore
            if (audioSourceFlight.isPlaying) audioSourceFlight.Stop();
        }

        // --- GESTIONE LOOP AUDIO INVINCIBILITÀ (SCUDO) ---
        if (invincibile && auraScudoVisiva.activeSelf)
        {
            tempoInvincibilitaAttuale -= Time.deltaTime;
            
            if (!audioSourceLoop.isPlaying && shieldLoopClip != null)
            {
                audioSourceLoop.clip = shieldLoopClip;
                audioSourceLoop.volume = volumeScudo;
                audioSourceLoop.Play();
            }

            if (tempoInvincibilitaAttuale <= 0)
            {
                invincibile = false;
                if (auraScudoVisiva != null) auraScudoVisiva.SetActive(false);
            }
        }
        else
        {
            if (audioSourceLoop.isPlaying) audioSourceLoop.Stop();

            if (invincibile) 
            {
                tempoInvincibilitaAttuale -= Time.deltaTime;
                if (tempoInvincibilitaAttuale <= 0) invincibile = false;
            }
        }

        if (gestionePausa != null && gestionePausa.GetControlloRiprendi())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -velocitaCadutaLenta);
            gestionePausa.controlloRiprendi = false;
        }
    }

    void EseguiSalto(bool vaiADestra)
    {
        float forzaOrizzontale = vaiADestra ? 4.2f : -4.2f;
        string triggerAnimazione = vaiADestra ? "SaltoVersoDestra" : "SaltoVersoSinistra";
        rb.linearVelocity = new Vector2(forzaOrizzontale, forzaSalto);
        
        if (animator != null)
        {
            animator.SetTrigger(triggerAnimazione);
            animator.SetBool("GuardaDestra", vaiADestra);
        }
        
        if (jumpSoundClip != null) audioSourceSFX.PlayOneShot(jumpSoundClip);
    }

    void AggiornaCuori()
    {
        for (int i = 0; i < cuori.Length; i++)
        {
            if (i < vite) cuori[i].SetActive(true); 
            else cuori[i].SetActive(false); 
        }
    }

    public void RiavviaGioco()
    {
        GameController.gameover = false;
        vite = 3;
        eMorto = false; 
        AggiornaCuori();
        transform.position = new Vector3(0, 0, 0);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.gravityScale = 1f; 
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = true;
        
        // Zittisci tutti i loop
        audioSourceLoop.Stop();
        audioSourceFlight.Stop();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (eMorto) return; 

        if(collision.gameObject.CompareTag("TraveSinistra")||collision.gameObject.CompareTag("TraveDestra"))
        {
            inCollisioneConTrave= true;
            numeroSaltiEffettuati = 0; 
            if (animator != null)
            {
                if(collision.gameObject.CompareTag("TraveSinistra")) 
                {
                    animator.SetTrigger("AtterraggioTraveSinistra");
                    animator.SetBool("GuardaDestra", true); 
                }
                else 
                {
                    animator.SetTrigger("AtterraggioTraveDestra");
                    animator.SetBool("GuardaDestra", false); 
                }
            }
        }                                                               
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (eMorto) return; 
        if(collision.gameObject.CompareTag("TraveSinistra")|| collision.gameObject.CompareTag("TraveDestra"))
        {
            if(inCollisioneConTrave) rb.linearVelocity= new Vector2(rb.linearVelocity.x, -velocitaCadutaLenta);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("TraveSinistra")|| collision.gameObject.CompareTag("TraveDestra")) inCollisioneConTrave= false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (eMorto) return;

        // SCUDO
        if (collision.CompareTag("Scudo"))
        {
            numeroScudi++;
            if (collectShieldClip != null) audioSourceSFX.PlayOneShot(collectShieldClip);

            Destroy(collision.gameObject);
            if (numeroScudi >= scudiMassimi)
            {
                invincibile = true;
                tempoInvincibilitaAttuale = tempoInvincibilita;
                numeroScudi = 0;
                
                if (auraScudoVisiva != null) auraScudoVisiva.SetActive(true);
            }
        }

        // NEMICI
        if (collision.CompareTag("Satellite") || collision.CompareTag("Meteorite") || collision.CompareTag("Navicella"))
        {
            if (!invincibile)
            {
                if (collision.CompareTag("Satellite")) vite--;
                else if (collision.CompareTag("Meteorite")) vite -= 2;
                else if (collision.CompareTag("Navicella")) vite -= 3;
                AggiornaCuori();

                if (vite <= 0) StartCoroutine(EseguiMorte());
                else
                {
                    if (hurtSoundClip != null) audioSourceSFX.PlayOneShot(hurtSoundClip);
                    if (animator != null) animator.SetTrigger("Hurt");
                }
            }
        }

        // CUORE
        if (collision.CompareTag("CuoreSpawn"))
        {
            if (collectHeartClip != null) audioSourceSFX.PlayOneShot(collectHeartClip);
            if (vite < 3) { vite++; AggiornaCuori(); Destroy(collision.gameObject); }
            else { if (messaggioCuoriAlMassimo != null) { messaggioCuoriAlMassimo.SetActive(true); StartCoroutine(NascondiMessaggio()); } Destroy(collision.gameObject); }
        }

        // SCARPE (VOLO)
        if (collision.CompareTag("Scarpe"))
        {
            scarpeRaccolte = true;
            staVolando = true; 
            tempoVoloAttuale = tempoVoloMassimo; 
            Destroy(collision.gameObject); 
            
            // 1. ANIMAZIONE
            if (animator != null) animator.SetBool("StaVolando", true);

            // 2. AUDIO RACCOLTA (One-Shot con volume regolabile)
            if (scarpeCollectClip != null && audioSourceSFX != null)
            {
                audioSourceSFX.PlayOneShot(scarpeCollectClip, volumeRaccoltaScarpe);
            }

            // Nota: L'audio del volo (loop) partirà automaticamente nel prossimo Update
        }
    }

    private IEnumerator NascondiMessaggio()
    {
        yield return new WaitForSeconds(2f); 
        messaggioCuoriAlMassimo.SetActive(false); 
    }

    IEnumerator EseguiMorte()
    {
        eMorto = true; 
        
        // ZITTISCI TUTTO
        if (audioSourceLoop.isPlaying) audioSourceLoop.Stop();
        if (audioSourceFlight.isPlaying) audioSourceFlight.Stop();

        GameController gc = FindObjectOfType<GameController>();
        if (gc != null) gc.BloccaSpawnsImmediato();
        
        rb.linearVelocity = Vector2.zero; 
        rb.bodyType = RigidbodyType2D.Kinematic; 
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        
        if (animator != null) animator.SetTrigger("Die");
        
        yield return new WaitForSeconds(1f); 

        if (gameOverSoundClip != null) audioSourceSFX.PlayOneShot(gameOverSoundClip);

        yield return new WaitForSeconds(2f); 
        
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.gravityScale = 2f; 
        yield return null;
    }
}