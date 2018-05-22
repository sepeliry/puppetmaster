using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreenController : MonoBehaviour {

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void SelectChallenge()
    {
        var instance = DataController.Instance;
        SceneManager.LoadScene("ChallengeMenu");
    }

    public void HomeScreen()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ScoreScreen()
    {
        SceneManager.LoadScene("Score");
    }
}
