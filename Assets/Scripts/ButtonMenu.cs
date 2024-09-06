using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenu : MonoBehaviour
{
    //M�todo para cargar o regresar a la escena del Men� Principal
    public void MenuPrincipal()
    {
        SceneManager.LoadScene(0);
    }

    //M�todo para cargar o regresar a la escena del Compilador
    public void Compilador()
    {
        SceneManager.LoadScene(1);
    }

    //M�todo para cargar o regresar a la escena del Juego
    public void Juego()
    {
        SceneManager.LoadScene(2);
    }

    //M�todo para reproducir el video de la historia
    //M�todo que traslada al jugador al tutorial
}
