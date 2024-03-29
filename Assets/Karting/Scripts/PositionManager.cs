using KartGame.KartSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : MonoBehaviour
{ 
    [Tooltip("Liste des voitures dans la scène, si vide, initialisé depuis le code")]
    public List<ArcadeKart> cars; // Liste des voitures ds la scene
    [Tooltip("Liste des checkpoints dans la scène, si vide, initialisé depuis le code")]
    public Transform[] checkpoints; // Liste des checkpoints dans la scène

    private bool isFrozen;

    void Start()
    {
        isFrozen = false;
        if (cars.Count == 0)
            cars = new List<ArcadeKart>(FindObjectsOfType<ArcadeKart>());
        if (checkpoints.Length == 0)
        {
            GameObject checkpointParent = GameObject.Find("Checkpoints");
            checkpoints = new Transform[checkpointParent.transform.childCount];
            for (int i = 0; i < checkpointParent.transform.childCount; i++)
            {
                checkpoints[i] = checkpointParent.transform.GetChild(i).transform;
            }
        }
    }

    void Update()
    {
        if (!isFrozen)
        {
            // Trie les voitures en fonction de leur position
            cars.Sort(CompareCars);
            //string txt = "";
            //for (int i = 0; i < cars.Count; i++)
            //{
            //    txt += "Position " + (i + 1) + ": " + cars[i].name + "\n";
            //}
            //Debug.Log(txt);
        }
    }


    int CompareCars(ArcadeKart car1, ArcadeKart car2)
    {
        // Compare le nb de checkpoints traverses
        int checkpointComparison = car2.GetCheckpointCount().CompareTo(car1.GetCheckpointCount());

        // Si le nb de checkpoints traverses est identique, compare les distances entre les voitures et leur position projetee
        if (checkpointComparison == 0)
        {
            float car1distanceToCheckpoint = ProjectPositionBetweenCheckpoints(car1);
            float car2distanceToCheckpoint = ProjectPositionBetweenCheckpoints(car2);
            return car1distanceToCheckpoint.CompareTo(car2distanceToCheckpoint);
             // On inverse l'ordre pour avoir la voiture la plus proche en premiere place
        }
        else
        {
            return checkpointComparison;
        }
    }

    float ProjectPositionBetweenCheckpoints(ArcadeKart car)
    {
        int currentCheckpointIndex = (car.GetCheckpointCount() + checkpoints.Length - 1) % checkpoints.Length;
        int nextCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Length;

        Vector3 currentCheckpointPosition = checkpoints[currentCheckpointIndex].position;
        Vector3 nextCheckpointPosition = checkpoints[nextCheckpointIndex].position;

        Vector3 trackDirection = (nextCheckpointPosition - currentCheckpointPosition);

        Vector3 carToGoal = car.transform.position - nextCheckpointPosition;
        Vector3 projectedCarToGoal = Vector3.Project(carToGoal, trackDirection);
        return projectedCarToGoal.magnitude;
    }

    public bool IsPlayerFirst()
    {
        return cars[0].gameObject.CompareTag("Player");
    }

    public void UpdateLeaderboardToggle()
    {
        isFrozen = !isFrozen;
    }

    public void StoreLeaderboard()
    {
        //assuming "unlockedWeapons" is a list of strings (names of weapons)

        for (int i = 0; i < cars.Count; i++)
        {
            PlayerPrefs.SetString("carAtPos_" + i, cars[i].name);
        }

        //player prefs will have string variables for unlockedWeapon_0, unlockedWeapon_1, etc.

        //it is also important to save the length of the list
        PlayerPrefs.SetInt("carsLeaderboardLength", cars.Count);
    }

    public int GetPlayerPosition()
    {
        return cars.FindIndex(car => car.gameObject.CompareTag("Player")) + 1;
    }


}