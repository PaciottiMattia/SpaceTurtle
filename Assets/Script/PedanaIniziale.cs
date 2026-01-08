using UnityEngine;

public class PedanaIniziale : MonoBehaviour
{
    public float velocitaDiscesa = 2f;
    public float tempoDiAttesa = 1f;

    private bool discesaIniziata = false;
    private float timer = 0f;
    private bool personaggioSullaPedana = true;
    public Punti puntiScript; // Riferimento allo script Punti

    void Start()
    {
        timer = tempoDiAttesa;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        if (personaggioSullaPedana)
        {
            return;
        }

        if (!discesaIniziata)
        {
            discesaIniziata = true;
        }

        if (discesaIniziata)
        {
            transform.Translate(Vector3.down * velocitaDiscesa * Time.deltaTime);

            if (transform.position.y < -6f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tartaruga"))
        {
            personaggioSullaPedana = true;
            puntiScript.personaggioSullaPedana = true; // Imposta a true quando entra in collisione
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tartaruga"))
        {
            personaggioSullaPedana = false;
            puntiScript.PersonaggioLasciaPedana(); // Chiama il metodo per impostare a false
        }
    }
}  