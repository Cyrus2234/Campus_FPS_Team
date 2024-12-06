using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Member fields
    public static GameManager instance;

    GameObject player;
    public PlayerController playerScript;

    [Header("----- Menus -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("----- General UI -----")]
    public Image playerHPBar;

    public GameObject playerDamageScreen;

    float timeScale;

    [Header("----- Bools -----")]
    bool isPaused;
    public bool isStartMenu;

    int goalCount;

    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !isStartMenu)
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

    public void Lose()
    {
        Pause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

}
