using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("----- Character Controller -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [Seperator]


    [Header("----- Stats -----")]
    [SerializeField,Range(0, 100)] int speed;
    [SerializeField,Range(0, 100)] int sprintMultiplier;

    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;

    [SerializeField, Range(0, 100)] float gravity = 9.18f;


    Vector3 moveDirection;
    Vector3 moveVelocity;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        moveDirection = (transform.right * InputManager.Instance.Movement.x)
                        +
                        (transform.forward * InputManager.Instance.Movement.y);

        controller.Move(moveDirection * speed * Time.deltaTime);

        moveVelocity.y -= gravity * Time.deltaTime;

        controller.Move(moveVelocity * Time.deltaTime);
    }


}
