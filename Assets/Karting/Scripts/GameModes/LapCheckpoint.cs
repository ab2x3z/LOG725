using UnityEngine;

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
        }
    }
}
