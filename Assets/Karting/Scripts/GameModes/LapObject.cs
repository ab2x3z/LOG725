using UnityEngine;

/// <summary>
/// This class inherits from TargetObject and represents a LapObject.
/// </summary>
public class LapObject : TargetObject
{
    [Header("LapObject")]
    [Tooltip("Is this the first/last lap object?")]
    public bool finishLap;

    [Tooltip("Sound of second lap")]
    public AudioClip CollectSound1;

    [Tooltip("Sound of last lap")]
    public AudioClip CollectSound2;

    [HideInInspector]
    public bool lapOverNextPass;

    private int currentLap = 0;

    void Start() {
        Register();
    }
    
    void OnEnable()
    {
        lapOverNextPass = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!((layerMask.value & 1 << other.gameObject.layer) > 0 && other.CompareTag("Player")))
            return;

        Objective.OnUnregisterPickup?.Invoke(this);

        switch(currentLap) {
            case 0: AudioUtility.CreateSFX(CollectSound, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
                break;
            
            case 1: AudioUtility.CreateSFX(CollectSound1, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
                break;

            case 2: AudioUtility.CreateSFX(CollectSound2, transform.position, AudioUtility.AudioGroups.Pickup, 0f);
                break;
            default:
                break;
        }
        currentLap++;
    }
}
