using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    [SerializeField] CharacterController _controller = null;
    [SerializeField] Camera _cam = null;
    [SerializeField] private float GroundDistance = 1;
    [SerializeField] private float JumpHeight = 1;
    [SerializeField] private float _acceleration = 1;
    [SerializeField] private float _drag = 0.1f;
    [SerializeField] private float _maxSpeed = 5;
    [SerializeField] private float sensitivity = 10;

    Vector3 _velocity = new Vector3();

    void FixedUpdate()
    {
        //Mouse rotation
        float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");
        Transform rot = _cam.transform;
        rot.RotateAround(rot.position, Vector3.up, rotateHorizontal * sensitivity);
        rot.RotateAround(rot.position, -rot.right, rotateVertical * sensitivity);

        Vector3 eul = rot.rotation.eulerAngles;
        _cam.transform.rotation = Quaternion.Euler(Mathf.Clamp((eul.x + 180) % 360, 140, 240) - 180, eul.y, 0);


        //Movement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = move.normalized * Mathf.Min(move.magnitude, 1);
        move = Quaternion.AngleAxis(eul.y, Vector3.up) * move;
        _velocity += move * _acceleration;

        Vector3 _vxz = new Vector3(_velocity.x, 0, _velocity.z);
        _vxz = _vxz.normalized * Mathf.Min(_maxSpeed, _vxz.magnitude);
        _vxz = _vxz * (1 - _drag);
        _velocity = new Vector3(_vxz.x, _velocity.y, _vxz.z);

        //Gravity
        _velocity.y += Physics.gravity.y * Time.deltaTime;

        //Jump
        bool _isGrounded = Physics.Raycast(transform.position, -Vector3.up, GroundDistance + 0.1f);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        if (Input.GetButtonDown("Jump") && _isGrounded)
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);

        _controller.Move(_velocity * Time.deltaTime);
    }
}
