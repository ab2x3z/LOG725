using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{

    Vector3 offset;
    Vector3 newPos;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0.0f, transform.position.y, 0.0f);
        transform.position = player.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;   
    }
}
