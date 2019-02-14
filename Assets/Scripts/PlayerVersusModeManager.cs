using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class PlayerVersusModeManager : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

        PlayerPanelController[] affectationPanels = this.FetchPlayerPanelControllerScripts(false, true);

        if (affectationPanels != null)
        {
            foreach (PlayerPanelController playerPanelController in  affectationPanels)
            {
                if (!playerPanelController.IsMoving && playerPanelController.IsChoiceMade)
                {
                    this.ManagePlayerAffectation(playerPanelController.OwnerId, playerPanelController.IsAtTheMiddle);
                    playerPanelController.IsRegistered = true;
                }
            }
        }

        this.DisplayValidationButon();
    }

    private void DisplayValidationButon()
    {
        PlayerPanelController[] affectationPanels = this.FetchPlayerPanelControllerScripts(true, true);
        if(affectationPanels != null && affectationPanels.Length > 1 && affectationPanels.All(panelAffectationScript => panelAffectationScript.IsRegistered))
        {
            ActivateConfirmationButton(true);
        }
        else
        {
            ActivateConfirmationButton(false);
        }
    }

    private void ActivateConfirmationButton(bool isInteractable)
    {
        Button button = this.GetComponentsInChildren<Transform>()
                        .Where(child => child.gameObject.CompareTag(TagConstants.TAG_NAME_MAIN_MENU_BUTTON))
                        .Select(child => child.GetComponent<Button>())
                        .First();

        button.interactable = isInteractable;
    }

    private void ManagePlayerAffectation(int playerId, bool isAtTheMiddle)
    {
        string playerType = PlayerTypeConstants.PLAYER_HUMAN;

        if (isAtTheMiddle)
        {
            playerType = PlayerTypeConstants.PLAYER_COMPUTER;
        }

        ApplicationUtils.AffectPlayer(playerId, playerType);
    }

    private PlayerPanelController[] FetchPlayerPanelControllerScripts(bool isRegistered, bool isChoiceMade)
    {
        PlayerPanelController[] playerPanelControllerScripts = GameObject.FindGameObjectsWithTag(TagConstants.TAG_NAME_PLAYER_AFFECTION_PANEL)
            .Select(gameObject => gameObject.GetComponent<PlayerPanelController>())
            .Where(playerPanelControllerScript => playerPanelControllerScript.IsRegistered == isRegistered && playerPanelControllerScript.IsChoiceMade == isChoiceMade)
            .ToArray(); ;
        return playerPanelControllerScripts;
    }
}
