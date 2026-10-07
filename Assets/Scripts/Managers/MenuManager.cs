using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Game"); 
    }

    public void SalirDelJuego()
    {
        Debug.Log("El juego se ha cerrado");
        Application.Quit();
    }
}
