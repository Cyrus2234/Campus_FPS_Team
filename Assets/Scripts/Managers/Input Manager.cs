using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] PlayerActions playerInput;

    //[Seperator]
    //float 


    public PlayerActions.GeneralActions Actions => playerInput.General;
    public Vector2 Movement => Actions.Move.ReadValue<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void Awake()
    {
        Instance = this;
        playerInput = new PlayerActions();
        playerInput.Enable();
    }
}
