using System.Collections;
using System.Collections.Generic;
using KartGame.KartSystems;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    Vector3 offset;
    Vector3 newPos;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindObjectOfType<CarManager>().gameObject;
        }

        offset = new Vector3(0.0f, transform.position.y, 0.0f);
        transform.position = player.transform.position + offset;
        //change rotation on X and Z axis
        transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;
        transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
    }
}
