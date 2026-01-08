using UnityEngine;
using UnityEngine.UI;

public class TempoVoloUI : MonoBehaviour
{
    public Slider tempoVoloSlider;
    public Tartaruga tartaruga;

    void Update()
    {
        if (tartaruga.scarpeRaccolte)
        {
            tempoVoloSlider.gameObject.SetActive(true);
            tempoVoloSlider.maxValue = tartaruga.tempoVoloMassimo;
            tempoVoloSlider.value = tartaruga.tempoVoloAttuale;
        }
        else
        {
            tempoVoloSlider.gameObject.SetActive(false);
        }
    }
}