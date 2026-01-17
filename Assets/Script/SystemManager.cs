using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    void Start()
    {
        // --------------------------------------------------------
        // 1. FLUIDITÀ (FPS)
        // --------------------------------------------------------
        // Sblocca il framerate. 
        // Su iPhone 13 Pro questo renderà il movimento fluidissimo
        // ed eliminerà l'effetto "scia" o sfocato.
        Application.targetFrameRate = 120;

        // NOTA: Se vuoi spingere al massimo l'iPhone 13 Pro (120Hz),
        // puoi scrivere 120, ma consumerà più batteria. 
        // 60 è il compromesso perfetto.


        // --------------------------------------------------------
        // 2. SCHERMO SEMPRE ACCESO
        // --------------------------------------------------------
        // Questo impedisce al telefono di andare in blocco schermo (schermo nero)
        // se il giocatore non tocca nulla per un po' (es. mentre legge la Lore).
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}