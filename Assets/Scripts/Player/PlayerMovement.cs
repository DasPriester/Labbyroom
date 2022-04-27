using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController cc;

    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    bool isGrounded;

    Vector3 vel;

    
    // Update is called once per frame
    void Update()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && vel.y < 0)
            vel.y = -2f;


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        cc.Move(speed * Time.deltaTime * move);

        if (Input.GetButtonDown("Jump") && isGrounded)
            vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        vel.y += gravity * Time.deltaTime;

        cc.Move(vel * Time.deltaTime);
    }
}
