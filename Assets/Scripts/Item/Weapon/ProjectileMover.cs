using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;

    private void OnTriggerEnter(Collider other)
    {
        if (hit != null)
        {
            Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);

            var hitInstance = Instantiate(hit, hitPoint, Quaternion.identity);
            if (UseFirePointRotation)
            {
                hitInstance.transform.rotation = Quaternion.LookRotation(other.transform.position - hitPoint, Vector3.up);
            }
            else if (rotationOffset != Vector3.zero)
            {
                hitInstance.transform.rotation = Quaternion.Euler(rotationOffset);
            }

            //Destroy hit effects depending on particle Duration time
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }
    }
}
