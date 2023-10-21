using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleCollissions : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private FireExtinguisher fireExtinguisher;

    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log(other.name);

        if (other.transform.parent != null)
        {
            GameObject gameObj = other.transform.parent.gameObject;

            if (gameObj.TryGetComponent<Fire>(out Fire fire))
            {
                fire.TryExtinguish(fireExtinguisher.GetAmountExtinguishedPerParticle());
            }
        }
    }
}
