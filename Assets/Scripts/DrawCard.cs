using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCard : MonoBehaviour
{
    public RawImage[] Image = new RawImage[10]; //Array para guardar los GameObject/RawImage que se seleccionaran para el intercambio inicial
    public GameManager GameManager; 
    public GameObject button;
    private bool inicio = false;
    int cambio = 0;
    private void OnMouseDown()
    {
        //Da inicio al juego
        if(!inicio)
        {
            GameManager.deck2.Robar(10); //Da las 10 cartas a la mano del Player2
            GameManager.deck1.Robar(10); //Da las 10 cartas a la mano del Player1

            //Pasa las cartas del Player1 a los RawImage
            for(int i = 0; i < 10; i++)
            {
                Image[i].texture = GameManager.deck1.Hand[i].GetComponent<GeneralCard>().sprite.texture;
                Image[i].transform.localScale =  new Vector2(1.5f,2);
            }
            button.transform.localScale = Vector3.one; //Aumenta la escala del botón para hacerlo visible
            inicio = true;
        }
    }

    //Método para hacer el intercambio de cartas del Player2
    public void Cambio()
    {
     if(GameManager.Turn == 2 && GameManager.jug2)
        {
            for(int i = 0; i<Image.Length;i++)
            {
                Image[i].texture = GameManager.deck2.Hand[i].GetComponent<GeneralCard>().sprite.texture;
                Image[i].transform.localScale = new Vector2(1.5f, 2);
            }
            button.transform.localScale = Vector3.one;
        }
    }

    //Método para eliminar la carta seleccionada e intercambiarla por una nueva carta random
    public void End(int num, Deck deck)
    {
        Destroy(deck.Hand[num]);
        deck.Hand[num] = null;
        deck.Robar(1);
        Image[num].transform.localScale = Vector3.zero;
        cambio++;
        if(cambio == 2)
        {
            Se_Acabo();
        }
    }

    //Método para eliminar las opciones de intercambio y el botón 
    public void Se_Acabo()
    {
        for(int i = 0; i < Image.Length;i++)
        {
            if(Image[i] != null)
            {
                Image[i].transform.localScale = Vector3.zero;
            }
        }
        button.transform.localScale = Vector3.zero;

        if(GameManager.Turn == 1) 
        {
            GameManager.jug1 = false; //Ya que el Player1 hizo sus intercambios
        }
        if (GameManager.Turn== 2)
        {
            GameManager.jug2 = false; //Ya que el Player2 hizo sus intercambios
        }
        GameManager.invoke = false;
        cambio = 0;                   //Permite reiniciar los cambios para que pueda cambiar sus cartas el siguiente Player
    }
}
