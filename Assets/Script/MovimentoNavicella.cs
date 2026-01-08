using UnityEngine;

public class MovimentoNavicella : MonoBehaviour
{
    public float velocita = 3f;
    public Vector2 direzione;

    void Update()
    {
        transform.Translate(direzione * velocita * Time.deltaTime);

        // Distruzione basata sulla posizione
        if (direzione == Vector2.right && transform.position.x >= 3.64f)
        {
            Destroy(gameObject);
        }
        else if (direzione == Vector2.left && transform.position.x <= -3.366f)
        {
            Destroy(gameObject);
        }
        else if (direzione == Vector2.down && transform.position.y <= -5.667f)
        {
            Destroy(gameObject);
        }
        else if (direzione == Vector2.up && transform.position.y >= 5.6567f)
        {
            Destroy(gameObject);
        }
    }
}