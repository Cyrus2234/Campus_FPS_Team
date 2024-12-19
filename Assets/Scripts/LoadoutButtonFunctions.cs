using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoadoutButtonFunctions : MonoBehaviour
{
    [SerializeField] Material[] colors;
    [SerializeField] GameObject[] throwables;
    enum Colors
    {
        Blue,
        Green,
        LightGreen,
        LightRed,
        Pink,
        Purple,
        Red,
        Teal,
        Yellow
    }

    public void confirm()
    {
        GameManager.instance.Unpause();
    }
    public void shotType()
    {
        GameManager.instance.OpenShotTypeMenu();
    }
    public void gunColor()
    {
        GameManager.instance.OpenGunColorMenu();
    }
    public void colorRed()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Red");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.Red]);
    }
    public void colorBlue()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Blue");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.Blue]);
    }
    public void colorGreen()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Green");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.Green]);
    }
    public void colorYellow()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Yellow");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.Yellow]);
    }
    public void colorTeal()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Teal");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.Teal]);
    }
    public void colorPink()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Pink");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.Pink]);
    }
    public void colorLightRed()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Light Red");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.LightRed]);
    }
    public void colorLightGreen()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Light Green");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.LightGreen]);
    }
    public void colorPurple()
    {
        GameManager.instance.ChangeGunColorText("Currently Selected: Purple");
        GameManager.instance.ChangeGunColor(colors[(int)Colors.Purple]);
    }
    public void throwable()
    {
        GameManager.instance.OpenThrowableMenu();
    }
    public void grenade()
    {
        GameManager.instance.ChangeThrowableText("Currently Selected: Grenade");
        GameManager.instance.playerScript.changeThrowable(throwables[0]);
    }
    public void stunGrenade()
    {
        GameManager.instance.ChangeThrowableText("Currently Selected: Stun Grenade");
        GameManager.instance.playerScript.changeThrowable(throwables[1]);
    }
    public void smokeBomb()
    {
        GameManager.instance.ChangeThrowableText("Currently Selected: Smoke Grenade");
        GameManager.instance.playerScript.changeThrowable(throwables[2]);
    }
}
