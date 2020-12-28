using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmasherController : MonoBehaviour
{
    Vector2 initPos;
    Vector2 smashPos;
    Rigidbody2D body;
    [SerializeField] float smashSpeed;
    [SerializeField] float riseSpeed;
    bool isSmashing = true;
    GameManagerController managerController;

    void Start()
    {

        managerController = FindObjectOfType<GameManagerController>();
        body = GetComponent<Rigidbody2D>();
        initPos = gameObject.transform.position;
        body.velocity = new Vector2(0, smashSpeed);
    }

    void FixedUpdate()
    {
        if (!isSmashing)
        {
            if ((body.position - smashPos).magnitude > (initPos - smashPos).magnitude)
                isSmashing = true;
        }

        else
        {
            body.MovePosition(new Vector2(initPos.x, body.position.y + smashSpeed * Time.fixedDeltaTime));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            managerController.RestartLevel();
        }

        else if (collision.collider.gameObject.tag == "Platform")
        {
            isSmashing = false;
            smashPos = body.position;
            body.velocity = new Vector2(0, riseSpeed);
        }
    }
}
