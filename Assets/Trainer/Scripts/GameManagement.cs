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
        public bool[] visitedObjects; // Keeps track of visited objects
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

        if (gamePaused && (Input.GetKeyDown(KeyCode.KeypadEnter) || OVRInput.GetDown(OVRInput.RawButton.A)))
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

        currentTarget = FindNearestHighlightedObject();

        // Actively track the distance to the closest object
        if (currentTarget != null && !gamePaused)
        {
            float distanceToTarget = Vector3.Distance(currentTarget.transform.position, playerController.transform.position);
            //Debug.Log($"Distance to target: {distanceToTarget}");

            if (distanceToTarget < 3.0f)  // Check if the player is within 3.0f units of the target
            {
                Debug.Log("Player is within 3.0f units of the target.");

                // Check if all objects in the current step are visited
                if (AllObjectsVisited())
                {
                    tutorialScreen.SetActive(true);
                    arrowScript.DisableArrow();
                    gamePaused = true;
                    currentTarget = null;
                    readyForNextStep = false;
                    NextTutorialItem();
                }
                else
                {
                    DisableNearestOutline();
                    Debug.Log("Please visit all objects in the current step.");
                }
            }
            else
            {
                arrowScript.PointTo(currentTarget.transform.position);  // Continuously update arrow direction
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
            InitializeVisitedObjects(); // Initialize visited objects for the new step
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

    private void DisableNearestOutline()
    {
        GameObject nearestObject = FindNearestHighlightedObject();
        if (nearestObject != null)
        {
            Outline outline = nearestObject.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;

                // Set visited to true for the nearest object
                int objectIndex = tutorialSteps[currentTutorialIndex].objects.IndexOf(nearestObject);
                if (objectIndex != -1)
                {
                    tutorialSteps[currentTutorialIndex].visitedObjects[objectIndex] = true;
                }
            }
        }
    }



    private GameObject FindNearestHighlightedObject()
    {
        Debug.Log("GameManagement FindNearestHighlightedObject: Searching for nearest object.");
        GameObject nearestObj = null;
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        if (tutorialSteps[currentTutorialIndex] == null)
            return null;
        foreach (var obj in tutorialSteps[currentTutorialIndex].objects)
        {

            float dist = Vector3.Distance(obj.transform.position, playerController.transform.position);
            if (dist < minDist)
            {
                tMin = obj;
                minDist = dist;
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

        return tMin;
    }

    private void DisplayCurrentTutorialStep()
    {
        InitializeVisitedObjects();
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
            if (tutorialSteps[i] != null)
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

    private void InitializeVisitedObjects()
    {
        // Initialize visited objects array for the current step
        int objectCount = tutorialSteps[currentTutorialIndex].objects.Count;
        tutorialSteps[currentTutorialIndex].visitedObjects = new bool[objectCount];
        for (int i = 0; i < objectCount; i++)
        {
            tutorialSteps[currentTutorialIndex].visitedObjects[i] = false;
        }
    }

    private bool AllObjectsVisited()
    {
        // Check if all objects in the current step are visited
        foreach (bool visited in tutorialSteps[currentTutorialIndex].visitedObjects)
        {
            if (!visited)
                return false;
        }
        return true;
    }
}
