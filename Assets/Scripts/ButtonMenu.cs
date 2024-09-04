using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenu : MonoBehaviour
{
    public void MenuPrincipal()
    {
        SceneManager.LoadScene(0);
    }

    public void Compilador()
    {
        SceneManager.LoadScene(1);
    }

    public void Juego()
    {
        SceneManager.LoadScene(2);
    }
}
