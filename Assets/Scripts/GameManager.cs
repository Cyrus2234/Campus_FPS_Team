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

    [Header("----- Basic Menu -----")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuStart;
    [SerializeField] GameObject menuLevelSelect;

    [Header("----- Loadout Menu -----")]
    [SerializeField] GameObject menuLoadout;
    [SerializeField] GameObject menuLoadoutActive;
    [SerializeField] GameObject loadoutShotTypeMenu;
    [SerializeField] TMP_Text loadoutShotTypeText;
    [SerializeField] GameObject loadoutGunColorMenu;
    [SerializeField] TMP_Text loadoutGunColorText;
    [SerializeField] GameObject loadoutThrowableMenu;
    [SerializeField] TMP_Text loadoutThrowableText;

    [Header("----- Misc -----")]
    [SerializeField] GameObject reticle;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] Image grenadeCooldown;

    [Header("----- Player -----")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerGunColor;
    public Image playerHPBar;
    public Image playerStaminaBar;
    public Image playerStaminaBack;
    public GameObject playerDamageScreen;

    public GameObject[] team;
    public GameObject[] enemy;

    public GameObject hasFlagUI;

    bool isPaused;
    public GameObject playerStunScreen;
    public bool isStartScreen;

    int goalCount;
    float timeScale;

    int flagCount;
    //int flagCurrentHeld;

    //bool hasFlag;

    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerGunColor = GameObject.FindWithTag("Player Gun");

        loadoutGunColorText.text = "Currently Selected: Teal";
        loadoutThrowableText.text = "Currently Selected: Stun Grenade";

        team = GameObject.FindGameObjectsWithTag("Team");
        enemy = GameObject.FindGameObjectsWithTag("Enemy");

        grenadeCooldown.fillAmount = 0;
        //hasFlag = false;
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
        if (Input.GetButtonDown("Loadout"))
        {
            if (menuActive == null)
            {
                OpenLoadoutMenu();
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

    public void OpenLoadoutMenu()
    {
        Pause();
        menuActive = menuLoadout;
        menuActive.SetActive(true);
    }

    public void ClosePreviousLoadoutMenu()
    {
        if (menuLoadoutActive != null)
        {
            menuLoadoutActive.SetActive(false);
            menuLoadoutActive = null;
        }
    }

    public void OpenShotTypeMenu()
    {
        ClosePreviousLoadoutMenu();
        menuLoadoutActive = loadoutShotTypeMenu;
        menuLoadoutActive.SetActive(true);
    }
    public void OpenGunColorMenu()
    {
        ClosePreviousLoadoutMenu();
        menuLoadoutActive = loadoutGunColorMenu;
        menuLoadoutActive.SetActive(true);
    }
    public void ChangeGunColorText(string text)
    {
        loadoutGunColorText.text = text;
    }
    public void ChangeThrowableText(string text)
    {
        loadoutThrowableText.text = text;
    }
    public void ChangeGunColor(Material color)
    {
        playerGunColor.GetComponent<MeshRenderer>().material = color;
    }
    public void OpenThrowableMenu()
    {
        ClosePreviousLoadoutMenu();
        menuLoadoutActive = loadoutThrowableMenu;
        menuLoadoutActive.SetActive(true);
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
        goalCountText.text = goalCount.ToString("F0") + " Enemies Remaining";

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
