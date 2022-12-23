using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //É necessário criar nas variáveis um código que escolha um número aleatório entre os leveis "fáceis" para começar
    private void Start()
    {
        PlayerPrefs.SetInt("TutorialHasPlayed", 0);
    }
    public void PlayGame()
    {
        if (PlayerPrefs.GetInt("TutorialHasPlayed", 0) <= 0)
        {
            PlayerPrefs.SetInt("TutorialHasPlayed", 1);
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
