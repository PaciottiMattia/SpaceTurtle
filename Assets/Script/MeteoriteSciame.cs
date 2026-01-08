
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeteoriteSciame : MonoBehaviour
{
    Vector2 posIniziale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posIniziale=transform.position;
    }

    // Update is called once per frame
    void Update()
    {
         transform.position= new Vector2(transform.position.x, transform.position.y - 1.5f* Time.deltaTime);// Spostamento del meteorite nello sciame
        if(!GameController.gameover){
            if(transform.position.y<-5.72f){
                Destroy(gameObject); //Distrugge l'oggetto meteorite, questo avviene una volta che fuori esce dallo schermo
            }
        }
    }
}
