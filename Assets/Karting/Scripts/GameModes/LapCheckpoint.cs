using UnityEngine;
using KartGame.KartSystems;

/// <summary>
/// This class inherits from TargetObject and represents a PickupObject.
/// </summary>
public class LapCheckpoint : TargetObject
{
    void Start() {
        Register();
    }

    void OnCollect()
    {
        Objective.OnRegisterPickup(this);
        this.active = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & 1 << other.gameObject.layer) > 0 && other.gameObject.CompareTag("Player"))
        {
            OnCollect();
            // on rentre en collision avec la boucing capsule, dc on va chercher ds le parent
            other.gameObject.GetComponentInParent<ArcadeKart>().IncrementCheckpointCounter();
        }
        // else for other ArcadeKart collision
        if ((layerMask.value & 1 << other.gameObject.layer) > 0 && other.gameObject.CompareTag("AI"))
        {
            other.gameObject.GetComponentInParent<ArcadeKart>().IncrementCheckpointCounter();
        }
    }
}
