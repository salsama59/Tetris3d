using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour
{
    private GameObject mainMenu;
    public GameObject[] mainMenuButtons;
    public GameObject[] localModeSubMenuButtons;
    private int currentMenuId;
    public enum MenuId { MAIN_MENU = 0, LOCAL_MODE = 1 };

    private void Start()
    {
        this.CurrentMenuId = (int)MenuId.MAIN_MENU;
        MainMenu = GameObject.FindWithTag(TagConstants.TAG_NAME_MAIN_MENU);

        if (MainMenu == null)
        {
            Debug.Log("Uneable to find the canvas in the scene");
        }
    }

    public void LaunchOnePlayerMode()
    {
        ApplicationData.playerNumber = 1;
        this.LaunchGame(SceneConstants.SCENE_NAME_ONE_PLAYER_MODE);
    }

    public void LaunchTwoPlayerMode()
    {
        ApplicationData.playerNumber = 2;
        this.LaunchGame(SceneConstants.SCENE_NAME_TWO_PLAYER_MODE);
    }

    public void DisplayLocalModeSubMenu1()
    {
        this.ManageCurrentDisplay();
        this.ToggleUIelements(true, localModeSubMenuButtons);
        this.CurrentMenuId = (int)MenuId.LOCAL_MODE;
    }

    public void ReturnToMainMenu()
    {
        this.ManageCurrentDisplay();
        this.ToggleUIelements(true, mainMenuButtons);
        this.CurrentMenuId = (int)MenuId.MAIN_MENU;
    }

    private void ManageCurrentDisplay()
    {
        switch(this.CurrentMenuId)
        {
            case (int)MenuId.MAIN_MENU:
                this.ToggleUIelements(false, mainMenuButtons);
                break;
            case (int)MenuId.LOCAL_MODE:
                this.ToggleUIelements(false, localModeSubMenuButtons);
                break;
        }
    }

    private void ToggleUIelements(bool isActivated, GameObject[] elementList)
    {
        foreach (GameObject element in elementList)
        {
            element.SetActive(isActivated);
        }

    }

    private void LaunchGame(string sceneName)
    {
        Destroy(MainMenu);
        SceneManager.LoadScene(sceneName);
    }

    public GameObject MainMenu
    {
        get
        {
            return mainMenu;
        }

        set
        {
            mainMenu = value;
        }
    }

    public int CurrentMenuId
    {
        get
        {
            return currentMenuId;
        }

        set
        {
            currentMenuId = value;
        }
    }
}
