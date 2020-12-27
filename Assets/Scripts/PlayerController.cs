using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D myRigidbody;

    private Vector2 movement;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Filp();

        anim.SetFloat("speed", movement.magnitude);
    }

    void Filp()
    {
        if (movement.x == 0) return;
        transform.rotation = movement.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    private void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + movement * speed * Time.fixedDeltaTime);
    }
}
