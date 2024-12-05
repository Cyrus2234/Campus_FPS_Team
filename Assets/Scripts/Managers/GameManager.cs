using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Member fields
    public static GameManager instance;

    public GameObject player;
    public PlayerMovement playerScript;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    float timeScale;

    bool isPaused;
    bool isStartMenu;

    int goalCount;

    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                Pause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                Unpause();
            }
        }
    }

    // Methods

    public void Pause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive.SetActive(false);
        menuActive = null;
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

    public void updateGameGoal(int scoreToAdd)
    {
        goalCount += scoreToAdd;
    }
}
