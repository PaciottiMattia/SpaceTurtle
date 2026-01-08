
using UnityEngine;
using TMPro; //Importa il namespace TextMeshPro

public class Punti : MonoBehaviour
{   
    //Gestione Punteggio
    public int punteggio = 0; // Punteggio iniziale
    public float tempoPunteggio = 1f; // Ogni quanti secondi aumentare il punteggio
    private float timerPunteggio = 0f;
    public TextMeshProUGUI testoPunteggio; // Riferimento al testo del punteggio
    public bool personaggioSullaPedana = true; // Aggiunta variabile

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update()
    {
       if(!GameController.gameover && !personaggioSullaPedana){
        timerPunteggio += Time.deltaTime;
        if (timerPunteggio >= tempoPunteggio){
            timerPunteggio = 0f;
            punteggio++; // Aumenta il punteggio
            AggiornaTestoPunteggio(); // Aggiorna il testo del punteggio
        }
       }
    void AggiornaTestoPunteggio(){
        if (testoPunteggio != null)
        {
            testoPunteggio.text = punteggio.ToString();
        }
    }

    }
     public void PersonaggioLasciaPedana()
    {
        personaggioSullaPedana = false;
    }
}
