using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("----- Character Controller -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;


    [Header("----- Stats -----")]
    [SerializeField][Range(0, 100)] int speed;
    [SerializeField][Range(0, 100)] int sprintMultiplier;

    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;

    [SerializeField] int gravity;


    Vector3 moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = transform.right * Input.GetAxis("Horizontal")
                        +
                        transform.forward * Input.GetAxis("Vertical");

        controller.Move(moveDirection * speed * Time.deltaTime);
    }


}
