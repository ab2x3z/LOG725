using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayPosition : MonoBehaviour
{
    TMP_Text textComponent ;
    PositionManager positionManager;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        positionManager = FindObjectOfType<PositionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (positionManager.GetPlayerPosition())
        {
            case 1:
                textComponent.text = "1st";
                textComponent.color = Color.yellow;
                break;
            case 2:
                textComponent.text = "2nd";
                textComponent.color = Color.grey;
                break;
            case 3:
                textComponent.text = "3rd";
                textComponent.color = new Color (0.803f, 0.496f, 0.195f, 1);
                break;
            default:
                textComponent.text = positionManager.GetPlayerPosition() + "th";
                textComponent.color = Color.white;
                break;
        }
    }
}
