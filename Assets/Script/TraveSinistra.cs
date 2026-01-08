using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraveSinistra : MonoBehaviour
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
        if(!GameController.gameover){
            if(transform.position.y>=-0.0068){
                 transform.position= new Vector2(transform.position.x, transform.position.y - 1f* Time.deltaTime);
            }
            else{
                transform.position= posIniziale;
            }
        }
       
    }
}
