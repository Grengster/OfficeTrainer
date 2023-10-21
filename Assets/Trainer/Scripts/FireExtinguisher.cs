using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [SerializeField] private float amountExtinguishedPerParticle = 0.01f;
    [SerializeField] private ParticleSystem PS = null;
    [SerializeField] private float extinguisherRaycastRange = 3f;
    public List<ParticleCollisionEvent> collisionEvents = new();


    public float GetAmountExtinguishedPerParticle()
    {
        return amountExtinguishedPerParticle;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward * extinguisherRaycastRange);
    }
}
