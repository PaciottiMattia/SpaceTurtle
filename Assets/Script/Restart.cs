using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public GameObject tartaruga; // Riferimento all'oggetto Tartaruga
    public void CLickRestart(){
        GameController.gameover= false;
        tartaruga.GetComponent<Tartaruga>().RiavviaGioco(); // Chiama il metodo RiavviaGioco()
    }
}
