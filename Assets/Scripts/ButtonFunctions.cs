using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void Resume()
    {
        if (GameManager.instance.GetPauseState())
           GameManager.instance.Unpause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Resume();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadScene(string _scene)
    {
        // Temporary fix for restarting level; game freezes whenever starting a new level and cursor disappears when going back into start menu
        Resume();
        Cursor.visible = true;
        

        SceneManager.LoadScene(_scene);
    }
}
