using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFuctions : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
        Time.timeScale = 1;
    }

    public void ToMain()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
