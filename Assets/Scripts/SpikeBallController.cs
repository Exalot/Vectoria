using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBallController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private float rotationVelocity;
    [SerializeField] private float rotationAcceleration;
    private GameManagerController controller;
    Rigidbody2D body;
    bool isMovingLeft;

    // Start is called before the first frame update
    void Start()
    {
        rotationVelocity = rotationSpeed;
        body = GetComponent<Rigidbody2D>();
        controller = FindObjectOfType<GameManagerController>();
        isMovingLeft = true;
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        rotationVelocity += rotationAcceleration * Time.fixedDeltaTime;

        if (rotationVelocity > 0 && isMovingLeft || rotationVelocity < 0 && !isMovingLeft)
        {
            isMovingLeft = !isMovingLeft;
        }

        transform.Rotate(new Vector3(0, 0, gameObject.transform.rotation.z + rotationVelocity * Time.fixedDeltaTime));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            controller.RestartLevel();
        }
        else if (collision.collider.gameObject.tag == "Swing Check")
        {
            Debug.Log("Allah");
            rotationAcceleration = -rotationAcceleration;
        }
    }
}
