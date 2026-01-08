using UnityEngine;
using UnityEngine.UI;

public class ScudiUI : MonoBehaviour
{
    public Slider scudiSlider;
    public Slider invincibilitaSlider;
    public Tartaruga tartaruga;

    void Update()
    {
        // Barra degli scudi
        scudiSlider.maxValue = tartaruga.scudiMassimi;
        scudiSlider.value = tartaruga.numeroScudi;

        // Gestione della visibilità della barra degli scudi
        if (tartaruga.numeroScudi > 0 && tartaruga.numeroScudi < tartaruga.scudiMassimi)
        {
            scudiSlider.gameObject.SetActive(true);
        }
        else
        {
            scudiSlider.gameObject.SetActive(false);
        }

        // Barra dell'invincibilità
        invincibilitaSlider.maxValue = tartaruga.tempoInvincibilita;
        invincibilitaSlider.value = tartaruga.tempoInvincibilitaAttuale;
        invincibilitaSlider.gameObject.SetActive(tartaruga.invincibile);
    }
}