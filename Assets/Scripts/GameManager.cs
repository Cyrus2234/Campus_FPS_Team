using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Member fields
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuStart;
    [SerializeField] GameObject menuLevelSelect;
    [SerializeField] GameObject reticle;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] Image grenadeCooldown;

    GameObject player;
    public PlayerController playerScript;
    public Image playerHPBar;
    public Image playerStaminaBar;
    public Image playerStaminaBack;
    public GameObject playerDamageScreen;

    bool isPaused;
    public bool isStartScreen;

    float timeScale;

    int goalCount;
    int flagCount;
    //int flagCurrentHeld;

    bool hasFlag = false;

    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        grenadeCooldown.fillAmount = 0;
        hasFlag = false;
        if (isStartScreen)
        {
            menuActive = menuStart;
            menuActive.SetActive(true);
        }
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
        reticle.SetActive(false);
    }

    public void Unpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        reticle.SetActive(true);
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void youLose()
    {
        Pause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void ToLevelScreen()
    {
        menuActive.SetActive(false);
        menuActive = menuLevelSelect;
        menuActive.SetActive(true);
    }

    // Accessors
    public GameObject GetPlayer()
    {
        return player;
    }

    public int GetGoalCount()
    {
        return goalCount;
    }
    public int GetFlagCount()
    {
        return goalCount;
    }

    public Image GetGrenadeCooldownImage()
    {
        return grenadeCooldown;
    }

    public bool GetPauseState()
    {
        return isPaused;
    }

    //public bool checkFlagState()
    //{
    //    return hasFlag;
    //}

    // Mutators
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");

        if (goalCount <= 0)
        {
            Pause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void updateFlagGoal(int amount)
    {
        flagCount += amount;
        //goalCountText.text = flagCount.ToString("F0"); //TODO ADD GOAL COUNT TEXT

    }

    public bool FlagDropOffComplete()
    {
        bool toReturn = false;
        if (flagCount <= 0)
        {
            toReturn = true;
            Pause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
        return toReturn;
    }
}
