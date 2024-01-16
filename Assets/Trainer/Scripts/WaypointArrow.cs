using UnityEngine;

public class WaypointArrow : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius to detect nearby objects
    public float arrowSpeed = 5.0f; // Rotation speed

    private GameObject closestObject;
    public Collider[] hitColliders;
    public Transform centerEye; // Reference to the CenterEye GameObject

    private void Start()
    {
        /*if (centerEye == null)
        {
            // If the CenterEye reference is not set, try to find it
            centerEye = GameObject.Find("CenterEyeAnchor").transform;
        }

        if (centerEye != null)
        {
            // Position the arrow 1 unit away from the CenterEye on the Z-axis
            transform.position = centerEye.position + centerEye.forward * 1.0f;
        }
        else
        {
            Debug.LogError("CenterEye not found. Ensure the reference is set or that the CenterEyeAnchor exists in the scene.");
        }*/
    }

    private void Update()
    {
        
/*
        FindClosestObject();

        if (closestObject)
        {
            // Calculate the direction to the target object
            Vector3 targetDirection = closestObject.transform.position - centerEye.position;

            // Use Quaternion.LookRotation to rotate the arrow to face the target
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Smoothly rotate the arrow towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, arrowSpeed * Time.deltaTime);

            // Position the arrow in front of the player's head using CenterEyeAnchor
            transform.position = centerEye.position + centerEye.forward * 1.0f;
        }*/
    }

    public void ToggleArrow(bool state)
    {
        gameObject.SetActive(state);
    }

    public void PointTo(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0; // Optionally, you can zero out the y component if you want the arrow to only rotate horizontally.

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1.0f);

        // If you have an arrow object that's a child of this GameObject, you might need to adjust its local rotation instead.
    }

    private void FindClosestObject()
    {
        closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = collider.gameObject;
                Debug.Log($"DISTANCE: {closestDistance}");
            }
        }
    }
}
