using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePanelScript : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(0);

        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
