using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PortalTraveller
{
    public CharacterController cc;

    Camera cam;

    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public float rotationSmoothTime = 0.1f;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    bool isGrounded;

    Vector3 vel;

    public float sensitivity = 20f;
    public Transform playerBody;
    float xRotation = 0f;

    private void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

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

        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        yaw += mX * sensitivity;
        pitch -= mY * sensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        vel = toPortal.TransformVector(fromPortal.InverseTransformVector(vel));
        Physics.SyncTransforms();
    }
}
