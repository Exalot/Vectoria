using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    //[SerializeField] float moveFactor;
    //Camera cam;
    //Vector2 initPlayerPos;
    //Vector2 BackgroundPosFactor;
    //float xPos;

    private float length, startpos;
    float temp;
    float dist;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        temp = (cam.transform.position.x * (1 - parallaxEffect)); 
        dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos +  dist, transform.position.y, transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }

    /*
    void Awake()
    {
        cam = Camera.main;
        initPlayerPos = cam.transform.position;
        BackgroundPosFactor = gameObject.transform.position;

    }
    */

    // Update is called once per frame
    /*
    private void Update()
    {
        xPos = BackgroundPosFactor.x + (cam.transform.position.x - initPlayerPos.x) / moveFactor % 1;
        gameObject.transform.position = new Vector3(xPos, 0, 0);

        if (Mathf.Abs(cam.transform.position.x - gameObject.transform.position.x) > 30f)
        {
            if ((cam.transform.position.x - gameObject.transform.position.x) > 0)
                BackgroundPosFactor = new Vector2(BackgroundPosFactor.x + 60f, BackgroundPosFactor.y);
            else
                BackgroundPosFactor = new Vector2(BackgroundPosFactor.x - 60f, BackgroundPosFactor.y);
        }
    }
    */

}

