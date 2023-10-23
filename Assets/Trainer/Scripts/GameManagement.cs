using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Oculus.Platform;
using Oculus.Platform.Models;

public class GameManagement : MonoBehaviour
{
    public OVRInput.RawButton startButton = OVRInput.RawButton.X; // Button to start the game
    public GameObject tutorialScreen; // Reference to the tutorial message UI Text
    public string sceneToLoad = "TrainerTestMap"; // The name of your game scene

    private bool gamePaused = true;

    private void Start()
    {
        tutorialScreen.SetActive(true);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start) || Input.GetKeyDown(KeyCode.R))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        if (gamePaused && OVRInput.GetDown(startButton))
        {
            tutorialScreen.SetActive(false);
            gamePaused = false;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
