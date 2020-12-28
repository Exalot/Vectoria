using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomController : MonoBehaviour
{
    public GameObject cam;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = cam.transform.position;
    }
}
