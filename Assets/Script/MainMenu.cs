using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
    public void StartTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void StartGame()
    {
       SceneManager.LoadScene("LoreScene");
    }
}