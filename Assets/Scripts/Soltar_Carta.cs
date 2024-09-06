using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soltar_Carta : MonoBehaviour
{
    public DrawCard game;
    public int num;
    public GameManager manager;

    //M�todo que activa al m�todo de eliminar cartas
    public void Activar()
    {
        if(manager.Turn == 1)
        {
            game.End(num, manager.deck1);
        }
        if (manager.Turn== 2)
        {
            game.End(num, manager.deck2);
        }
    }
}
