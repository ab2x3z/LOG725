using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    private GameObject[] stats_slider;

    // Start is called before the first frame update
    void Start()
    {
        //Gather all GameObjects children of the StatsPanel
        stats_slider = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            stats_slider[i] = transform.GetChild(i).gameObject;
        }

    }

    // Update is called once per frame
    public void UpdateStats(GameObject newCar)
    {
        ArcadeKart car = newCar.GetComponent<ArcadeKart>();

        for (int i = 0; i < transform.childCount; i++)
        {
            //Update the value of the children sliders os stats_slider[i]
            //stats_slider[i].GetComponent<Slider>().value = car.baseStats.topSpeed;
            switch (i)
            {
                case 0:
                    stats_slider[i].GetComponentInChildren<Slider>().value = newCar.GetComponent<Rigidbody>().mass;
                    break;
                case 1:
                    stats_slider[i].GetComponentInChildren<Slider>().value = car.baseStats.TopSpeed;
                    break;
                case 2:
                    stats_slider[i].GetComponentInChildren<Slider>().value = car.baseStats.Acceleration * car.baseStats.AccelerationCurve;
                    break;
                case 3:
                    stats_slider[i].GetComponentInChildren<Slider>().value = car.baseStats.Braking * car.baseStats.CoastingDrag;
                    break;
                case 4:
                    stats_slider[i].GetComponentInChildren<Slider>().value = car.baseStats.Steer;
                    break;
                case 5:
                    stats_slider[i].GetComponentInChildren<Slider>().value = car.DriftGrip;
                    break;
            }

        }
    }
}
