using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Member fields
    public static GameManager instance;

    GameObject player;
    float timeScale;
    bool isPaused;

    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        
    }

    // Methods

    public void Pause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        Time.timeScale = timeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Accessors
    public GameObject GetPlayer()
    {
        return player;
    }

    // Mutators
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
}
