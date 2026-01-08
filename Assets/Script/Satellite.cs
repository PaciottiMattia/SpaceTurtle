
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Satellite : MonoBehaviour
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
        transform.position= new Vector2(transform.position.x, transform.position.y - 3f* Time.deltaTime);// Spostamento del satellite
        if(!GameController.gameover){
            if(transform.position.y<-5.385f){
                Destroy(gameObject); //Distrugge l'oggetto satellite, q uesto avviene una volta che fuori esce dallo schermo
            }
        }
    }
}