using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Oculus.Platform;
using Oculus.Platform.Models;
using System;
using System.Collections.Generic;
using TMPro;

public class GameManagement : MonoBehaviour
{
    [Header("Game Controls")]
    public OVRInput.RawButton startButton = OVRInput.RawButton.X; // Button to start the game
    public GameObject tutorialScreen; // Reference to the tutorial message UI
    public TextMeshProUGUI tutorialText; // Reference to the text component where tutorial text is displayed
    public string sceneToLoad = "TrainerTestMap"; // The name of your game scene
    public GameObject playerController;
    private GameObject currentTarget = null;
    public WaypointArrow arrowScript;

    [Header("Tutorial Objects")]
    [SerializeField]
    public GameObject[][] tutorialObjects; // Now an array of arrays
    [System.Serializable]
    public class TutorialStep
    {
        public List<GameObject> objects = new List<GameObject>();
        public string message;
        public List<Outline> outlines = new List<Outline>();
    }

    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private bool gamePaused = true;
    private int currentTutorialIndex = 0; // To keep track of which tutorial item we're on
    private bool readyForNextStep = false;

    private void Start()
    {
        Debug.Log("GameManagement Start: Tutorial initialized.");
        tutorialScreen.SetActive(true);
        gamePaused = true;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start) || Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("GameManagement Update: Scene reloading triggered.");
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        if (gamePaused && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!readyForNextStep)
            {
                DisplayCurrentTutorialStep();
            }
            else
            {
                tutorialScreen.SetActive(false);
                gamePaused = false;
                UpdateOutlines();
            }
        }
        if (readyForNextStep && currentTarget == null)
        {
            currentTarget = FindNearestHighlightedObject();
            if (currentTarget != null)
            {
                arrowScript.PointTo(currentTarget.transform.position);
            }
        }

        // Actively track the distance to the closest object
        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(currentTarget.transform.position, playerController.transform.position);
            Debug.Log($"Distance to target: {distanceToTarget}"); // For debugging

            if (distanceToTarget < 3.0f)
            {
                Debug.Log("Player is within 3.0f units of the target.");
                tutorialScreen.SetActive(true);
                gamePaused = true;
                currentTarget = null;
                readyForNextStep = false;
                NextTutorialItem();
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("GameManagement StartGame: Loading game scene.");
        SceneManager.LoadScene(sceneToLoad);
    }

    private void NextTutorialItem()
    {
        currentTutorialIndex++;

        if (currentTutorialIndex < tutorialSteps.Count)
        {
            DisplayCurrentTutorialStep(); // Display the next tutorial step
        }
        else
        {
            // End of the tutorial
            tutorialScreen.SetActive(false);
            gamePaused = false;
        }

        UpdateOutlines(); // Update the outlines for all objects
    }

    private GameObject FindNearestHighlightedObject()
    {
        Debug.Log("GameManagement FindNearestHighlightedObject: Searching for nearest object.");
        GameObject nearestObj = null;
        float minDistance = float.MaxValue;

        foreach (var obj in tutorialSteps[currentTutorialIndex].objects)
        {
            float distance = Vector3.Distance(obj.transform.position, playerController.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObj = obj;
                Debug.Log($"Distance: {minDistance}");
            }
        }

        if (nearestObj != null)
        {
            Debug.Log($"GameManagement FindNearestHighlightedObject: Nearest object is {nearestObj.name}.");
        }
        else
        {
            Debug.Log("GameManagement FindNearestHighlightedObject: No object found.");
        }

        return nearestObj;
    }

    private void DisplayCurrentTutorialStep()
    {
        Debug.Log($"GameManagement DisplayCurrentTutorialStep: Displaying tutorial step {currentTutorialIndex}.");
        if (currentTutorialIndex < tutorialSteps.Count)
        {
            tutorialText.text = tutorialSteps[currentTutorialIndex].message;
            tutorialScreen.SetActive(true);
            readyForNextStep = true;
        }
        else
        {
            Debug.Log("GameManagement DisplayCurrentTutorialStep: Tutorial ended.");
            tutorialScreen.SetActive(false);
            gamePaused = false;
        }
    }

    private void UpdateOutlines()
    {
        for (int i = 0; i < tutorialSteps.Count; i++)
        {
            bool shouldEnable = i == currentTutorialIndex;
            foreach (var obj in tutorialSteps[i].objects)
            {
                Outline outline = obj.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = shouldEnable;
                }
            }
        }
    }

}
