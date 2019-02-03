using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour
{
    public GameObject[] mainMenuButtons;
    public GameObject[] localModeSubMenuButtons;
    public GameObject[] versusModeSubMenuButtons;

    private void Start()
    {
        this.CurrentMenuId = (int)MenuEnum.MenuId.MAIN_MENU;
        MainMenu = GameObject.FindWithTag(TagConstants.TAG_NAME_MAIN_MENU);

        if (MainMenu == null)
        {
            Debug.Log("Unable to find the canvas in the scene");
        }
    }

    public void LaunchOnePlayerMode()
    {
        ApplicationUtils.playerNumber = 1;
        this.LaunchGame(SceneConstants.SCENE_NAME_ONE_PLAYER_MODE);
    }

    public void LaunchTwoPlayerMode()
    {
        ApplicationUtils.playerNumber = 2;
        this.LaunchGame(SceneConstants.SCENE_NAME_TWO_PLAYER_MODE);
    }

    public void LaunchVersusMatch(Dictionary<int, string> playersAffectations)
    {
        ApplicationUtils.playerNumber = 2;

        for(int i = 0; i < ApplicationUtils.playerNumber; i++)
        {
            ApplicationUtils.AffectPlayer(i, playersAffectations[i]);
        }

        this.LaunchGame(SceneConstants.SCENE_NAME_TWO_PLAYER_MODE);
    }

    public void DisplayVersusModeSubMenu1()
    {
        this.SwitchMenu(true, MenuEnum.MenuId.VERSUS_MODE, versusModeSubMenuButtons);
    }

    public void DisplayLocalModeSubMenu1()
    {
        this.SwitchMenu(true, MenuEnum.MenuId.LOCAL_MODE, localModeSubMenuButtons);
    }

    public void ReturnToMainMenu()
    {
        this.SwitchMenu(true, MenuEnum.MenuId.MAIN_MENU, mainMenuButtons);
    }

    public void ReturnToLocalModeMenu()
    {
        this.SwitchMenu(true, MenuEnum.MenuId.LOCAL_MODE, localModeSubMenuButtons);
    }

    private void SwitchMenu(bool activateUiElementsFlag, MenuEnum.MenuId currentMenuId, GameObject[] uiElementListToDisable)
    {
        this.ManageCurrentDisplay();
        this.ToggleUIelements(activateUiElementsFlag, uiElementListToDisable);
        this.CurrentMenuId = (int)currentMenuId;
    }

    private void ManageCurrentDisplay()
    {
        switch(this.CurrentMenuId)
        {
            case (int)MenuEnum.MenuId.MAIN_MENU:
                this.ToggleUIelements(false, mainMenuButtons);
                break;
            case (int)MenuEnum.MenuId.LOCAL_MODE:
                this.ToggleUIelements(false, localModeSubMenuButtons);
                break;
            case (int)MenuEnum.MenuId.VERSUS_MODE:
                this.ToggleUIelements(false, versusModeSubMenuButtons);
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

        SoundUtils.StopSound(TagConstants.TAG_NAME_MAIN_MENU_SOUND);

        switch (sceneName)
        {
            case SceneConstants.SCENE_NAME_ONE_PLAYER_MODE:
                SoundUtils.PlaySound(TagConstants.TAG_NAME_ONE_PLAYER_MODE_SOUND);
                break;
            case SceneConstants.SCENE_NAME_TWO_PLAYER_MODE:
                SoundUtils.PlaySound(TagConstants.TAG_NAME_TWO_PLAYER_MODE_SOUND);
                break;
            default:
                break;
        }

        Destroy(MainMenu);
        SceneManager.LoadScene(sceneName);
    }

    public GameObject MainMenu { get; set; }

    public int CurrentMenuId { get; set; }
}
