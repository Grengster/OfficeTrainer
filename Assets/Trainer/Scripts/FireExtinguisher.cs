using OculusSampleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [SerializeField] private float amountExtinguishedPerParticle = 0.01f;
    [SerializeField] private OVRGrabbable grabbable = null;
    [SerializeField] private ParticleSystem ps = null;
    [SerializeField] private float extinguisherRaycastRange = 3f;
    public List<ParticleCollisionEvent> collisionEvents = new();
    public WaypointArrow arrowScript = null;
    public bool grabbedTutorial = true;
    public GameObject grabTutorial = null;


    public float GetAmountExtinguishedPerParticle()
    {
        return amountExtinguishedPerParticle;
    }

    private void Start()
    {
        if (grabbable == null) { Debug.LogError("GRABBABLE ERROR"); }
        if (ps == null) { Debug.LogError("PS ERROR"); }
        ToggleFireExtinguisher(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbable.isGrabbed || Input.GetKeyDown(KeyCode.E))
        {
            arrowScript.ToggleArrow(false);
            if (grabbedTutorial)
            {
                grabTutorial.SetActive(true);
                if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
                {
                    grabTutorial.SetActive(false);
                    grabbedTutorial = false;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.E) || grabbable.grabbedBy.gameObject.GetComponent<OVRGrabber>().m_controller == OVRInput.Controller.LTouch && OVRInput.Get(OVRInput.RawButton.LIndexTrigger) )
            {
                ToggleFireExtinguisher(true);
            }
            else if (grabbable.grabbedBy.gameObject.GetComponent<OVRGrabber>().m_controller == OVRInput.Controller.RTouch && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            {
                ToggleFireExtinguisher(true);
            }
            else
                ToggleFireExtinguisher(false);
        }
        else
        {
            ToggleFireExtinguisher(false);
        }
    }

    void ToggleFireExtinguisher(bool isActive)
    {
        var em = ps.emission;
        em.enabled = isActive;

    }
}
