using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenu : MonoBehaviour
{
    //Método para cargar o regresar a la escena del Menú Principal
    public void MenuPrincipal()
    {
        SceneManager.LoadScene(0);
    }

    //Método para cargar o regresar a la escena del Compilador
    public void Compilador()
    {
        SceneManager.LoadScene(1);
    }

    //Método para cargar o regresar a la escena del Juego
    public void Juego()
    {
        SceneManager.LoadScene(2);
    }

    //Método para reproducir el video de la historia
    //Método que traslada al jugador al tutorial
}
