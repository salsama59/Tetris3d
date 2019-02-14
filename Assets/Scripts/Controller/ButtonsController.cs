using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class ButtonsController : MonoBehaviour
{
    public GameObject[] mainMenuButtons;
    public GameObject[] localModeSubMenuButtons;
    public GameObject[] versusModeSubMenuButtons;
    public GameObject versusModeOptionsPanel;
    public GameObject playerSideSelectionPanel;

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
        this.LaunchGame(SceneConstants.SCENE_NAME_TWO_PLAYER_MODE);
    }

    public void DisplayVersusModeSubMenu1()
    {

        ApplicationUtils.playerNumber = 2;

        this.ManageCurrentDisplay();

        GameObject graphicInterface = GameUtils.FetchGraphicInterface();

        RectTransform graphicInterfaceRectTransform = graphicInterface.GetComponent<RectTransform>();

        GameObject instantiatedVersusModePanel = Instantiate(versusModeOptionsPanel);

        instantiatedVersusModePanel.transform.SetParent(graphicInterface.transform, false);

        Button instantiatedVersusModePanelButton = instantiatedVersusModePanel.GetComponentsInChildren<Transform>()
                        .Where(child => child.gameObject.CompareTag(TagConstants.TAG_NAME_MAIN_MENU_BUTTON))
                        .Select(child => child.GetComponent<Button>())
                        .First();
        instantiatedVersusModePanelButton.onClick.AddListener(this.LaunchTwoPlayerMode);

        RectTransform instantiatedVersusModePanelRectTransform = instantiatedVersusModePanel.GetComponent<RectTransform>();
        instantiatedVersusModePanelRectTransform.anchoredPosition3D = Vector3.zero;

        for (int i = 0; i < ApplicationUtils.playerNumber; i++)
        {
            GameObject instantiatedPlayerSideSelectionPanel = Instantiate(playerSideSelectionPanel);
            RectTransform instantiatedPlayerPanelRectTransform = instantiatedPlayerSideSelectionPanel.GetComponent<RectTransform>();
            
            instantiatedPlayerSideSelectionPanel.transform.SetParent(graphicInterface.transform, false);
           
            PlayerPanelController playerPanelControllerScript = instantiatedPlayerSideSelectionPanel.GetComponent<PlayerPanelController>();
            playerPanelControllerScript.OwnerId = i;
            instantiatedPlayerPanelRectTransform.anchoredPosition3D = new Vector3(0, playerPanelControllerScript.CalculateTargetYposition(graphicInterfaceRectTransform), 0);

            Image instantiatedPlayerSideSelectionPanelImage = instantiatedPlayerSideSelectionPanel.GetComponent<Image>();
            instantiatedPlayerSideSelectionPanelImage.color = ApplicationUtils.GetPlayerColor(i);

            Text instantiatedPlayerSideSelectionPanelText = instantiatedPlayerSideSelectionPanel.GetComponentInChildren<Text>();

            string playerNumberId = "";

            if((int)PlayerEnum.PlayerId.PLAYER_1 == i)
            {
                playerNumberId = "1";
            }
            else if((int)PlayerEnum.PlayerId.PLAYER_2 == i)
            {
                playerNumberId = "2";
            }

            instantiatedPlayerSideSelectionPanelText.text = instantiatedPlayerSideSelectionPanelText.text.Replace("{0}", playerNumberId);
        }

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
