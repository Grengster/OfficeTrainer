using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WaypointArrow : MonoBehaviour
{
    public GameObject arrow;
    private void Start()
    {
        
        this.arrow.SetActive(false);
    }

    public void PointTo(Vector3 targetPosition)
    {
        this.arrow.SetActive(true);

        Vector3 relativePos = targetPosition - transform.position;
        // Slerp smoothly interpolates the rotation towards the target rotation
        this.transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        // Debugging aids
        Debug.DrawLine(transform.position, targetPosition, Color.red, 2f); // Draws a line to the target

    }

}
