using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelController : GenericElementController
{
    private bool isAtTheMiddle;
    private bool isAtTheLeft;
    private bool isAtTheRight;
    private float middleTargetPosition;
    private float rightTargetPosition;
    private float leftTargetPosition;
    private bool isChoiceMade;
    private bool isRegistered;
    private bool isChoiceInputBlocked;
    private bool isPositionInputBlocked;
    private bool isLeftInputPushed;
    private bool isRigthInputPushed;

    public GameObject panelMoveEffect;
    public GameObject confirmEffect;

    public override void Awake()
    {
        this.enabled = true;
    }

    // Use this for initialization
    void Start () {

        GameObject graphicInterface = GameUtils.FetchGraphicInterface();
        RectTransform interfaceRectTransform = graphicInterface.GetComponent<RectTransform>();
        
        this.IsMoving = true;
        this.IsAtTheLeft = false;
        this.IsAtTheMiddle = true;
        this.IsAtTheRight = false;
        this.IsChoiceInputBlocked = false;
        this.IsPositionInputBlocked = false;
        this.IsLeftInputPushed = false;
        this.IsRigthInputPushed = false;
        MiddleTargetPosition = 0;
        RightTargetPosition = this.GetRightPositionValue(interfaceRectTransform);
        LeftTargetPosition = this.GetLeftPositionValue(interfaceRectTransform);
        this.InitializeDifficultyDropdown();

    }
	
	// Update is called once per frame
	void FixedUpdate() {

        KeyCode calculatedRightMovementKey = DetectPlayerMovement(DirectionEnum.Direction.RIGHT);
        KeyCode calculatedLeftMovementKey = DetectPlayerMovement(DirectionEnum.Direction.LEFT);

        if (Input.GetKey(calculatedRightMovementKey) && this.IsMoving && !this.IsPositionInputBlocked)
        {
            if (!this.IsMoveForbiden(calculatedRightMovementKey))
            {
                this.MoveObjectToNewPosition(this.GetPanelTargetPosition(DirectionEnum.Direction.RIGHT));
                this.UpdateGlobalPositionState();
                this.IsPositionInputBlocked = true;
                this.IsRigthInputPushed = true;
            }
        }
        else if (Input.GetKey(calculatedLeftMovementKey) && this.IsMoving && !this.IsPositionInputBlocked)
        {
            if (!this.IsMoveForbiden(calculatedLeftMovementKey))
            {
                this.MoveObjectToNewPosition(this.GetPanelTargetPosition(DirectionEnum.Direction.LEFT));
                this.UpdateGlobalPositionState();
                this.IsPositionInputBlocked = true;
                this.IsLeftInputPushed = true;
            }
        }

        if (!Input.GetKey(calculatedLeftMovementKey) && this.IsLeftInputPushed && this.IsPositionInputBlocked)
        {
            this.IsPositionInputBlocked = false;
            this.IsLeftInputPushed = false;
        }
        else if (!Input.GetKey(calculatedRightMovementKey) && this.IsRigthInputPushed && this.IsPositionInputBlocked)
        {
            this.IsPositionInputBlocked = false;
            this.IsRigthInputPushed = false;
        }

        if ((int)PlayerEnum.PlayerId.PLAYER_1 == this.OwnerId)
        {
            this.ManagePlayerConfimChoiceInput(KeyCode.Space);
        }

        if ((int)PlayerEnum.PlayerId.PLAYER_2 == this.OwnerId)
        {
            this.ManagePlayerConfimChoiceInput(KeyCode.KeypadEnter);
        }

    }

    private void AffectDifficulty(Dropdown currentDropdown)
    {
        ApplicationUtils.AffectDifficulty(this.OwnerId, currentDropdown.options[currentDropdown.value].text);
    }

    private void InitializeDifficultyDropdown()
    {
        GameObject dropdownGameObject = GameObject.FindGameObjectsWithTag(TagConstants.TAG_NAME_PLAYER_PANEL_DIFFICULTY_DROPDOWN)
            .Where(dropdown => dropdown.transform.parent.gameObject == this.gameObject).First();
        Dropdown dropdownComponent = dropdownGameObject.GetComponent<Dropdown>();

        dropdownComponent.ClearOptions();
        List<string> optionList = new List<string>
        {
            DifficultyConstant.EASY_MODE,
            DifficultyConstant.NORMAL_MODE
        };
        dropdownComponent.AddOptions(optionList);

        dropdownComponent
            .onValueChanged
            .AddListener(delegate { this.AffectDifficulty(dropdownComponent); });

        ApplicationUtils.AffectDifficulty(this.OwnerId, optionList.First());

    }

    private void ManagePlayerConfimChoiceInput(KeyCode keyPressed)
    {
        if (Input.GetKey(keyPressed) && !this.IsChoiceInputBlocked)
        {
            this.ManagePlayerPanelInformation();
            this.IsChoiceInputBlocked = true;
        }
        else if (!Input.GetKey(keyPressed) && this.IsChoiceInputBlocked)
        {
            this.IsChoiceInputBlocked = false;
        }
    }

    private void ManagePlayerPositionInput(KeyCode keyPressed)
    {
        if (Input.GetKey(keyPressed) && !this.IsChoiceInputBlocked)
        {
            this.ManagePlayerPanelInformation();
            this.IsChoiceInputBlocked = true;
        }
        else if (!Input.GetKey(keyPressed) && this.IsChoiceInputBlocked)
        {
            this.IsChoiceInputBlocked = false;
        }
    }

    private void ManagePlayerPanelInformation()
    {
        Text confirmText = this.GetComponentsInChildren<Text>()
            .Where(text => text.CompareTag(TagConstants.TAG_NAME_PLAYER_PANEL_CONFIRM_TEXT))
            .First();

        if (this.IsMoving)
        {
            this.IsMoving = false;
            this.IsChoiceMade = true;
            confirmText.enabled = true;
        }
        else
        {
            this.IsMoving = true;
            this.IsChoiceMade = false;
            this.IsRegistered = false;
            confirmText.enabled = false;
        }

        Instantiate(confirmEffect, this.transform.position, Quaternion.identity);
    }

    private void UpdateGlobalPositionState()
    {
        RectTransform panelRectTransform = this.GetComponent<RectTransform>();

        if (panelRectTransform.anchoredPosition.x == MiddleTargetPosition)
        {
            this.UpdatePositionStates(true, false, false);
        }
        else if(panelRectTransform.anchoredPosition.x == RightTargetPosition)
        {
            this.UpdatePositionStates(false, true, false);
        }
        else if (panelRectTransform.anchoredPosition.x == LeftTargetPosition)
        {
            this.UpdatePositionStates(false, false, true);
        }

        GameObject dropdownGameObject = GameObject.FindGameObjectsWithTag(TagConstants.TAG_NAME_PLAYER_PANEL_DIFFICULTY_DROPDOWN)
            .Where(dropdown => dropdown.transform.parent.gameObject == this.gameObject).First();
        Dropdown dropdownComponent = dropdownGameObject.GetComponent<Dropdown>();
        dropdownComponent.interactable = this.IsAtTheMiddle;
    }

    private void UpdatePositionStates(bool isAtTheMiddle, bool isAtTheRight, bool isAtThLeft)
    {
        this.IsAtTheMiddle = isAtTheMiddle;
        this.IsAtTheRight = isAtTheRight;
        this.IsAtTheLeft = isAtThLeft;
    }

    public float CalculateTargetXposition(DirectionEnum.Direction direction, RectTransform interfaceRectTransform)
    {
        float targetXposition;

        if (this.IsAtTheLeft || this.IsAtTheRight)
        {
            targetXposition = 0;
        }
        else
        {
            switch (direction)
            {
                case DirectionEnum.Direction.LEFT:
                    targetXposition = GetLeftPositionValue(interfaceRectTransform);
                    break;
                case DirectionEnum.Direction.RIGHT:
                    targetXposition = GetRightPositionValue(interfaceRectTransform);
                    break;
                default:
                    targetXposition = 0;
                    break;
            }
        }

        return targetXposition;
    }

    public float CalculateTargetYposition(RectTransform interfaceRectTransform)
    {
        float targetYposition;
        if (this.OwnerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            targetYposition = 25 * interfaceRectTransform.rect.height / 100;
        }
        else
        {
            targetYposition = -25 * interfaceRectTransform.rect.height / 100;
        }

        return targetYposition;
    }

    private float GetRightPositionValue(RectTransform interfaceRectTransform)
    {
        return 100/3 * interfaceRectTransform.rect.width / 100;
    }

    private float GetLeftPositionValue(RectTransform interfaceRectTransform)
    {
        return -100/3 * interfaceRectTransform.rect.width / 100;
    }

    private Vector3 GetPanelTargetPosition(DirectionEnum.Direction direction)
    {
        GameObject graphicInterface = GameUtils.FetchGraphicInterface();
        RectTransform interfaceRectTransform = graphicInterface.GetComponent<RectTransform>();
        RectTransform panelRectTransform = this.GetComponent<RectTransform>();

        float targetXposition = this.CalculateTargetXposition(direction, interfaceRectTransform);
        float targetYposition = this.CalculateTargetYposition(interfaceRectTransform);
        float targetZposition = panelRectTransform.anchoredPosition3D.z;

        return new Vector3(targetXposition, targetYposition, targetZposition);
    }

    private new void MoveObjectToNewPosition(Vector3 newPosition)
    {
        RectTransform panelRectTransform = this.GetComponent<RectTransform>();
        panelRectTransform.anchoredPosition = newPosition;
        Instantiate(panelMoveEffect, this.transform.position, Quaternion.identity);
    }

    private bool IsMoveForbiden(KeyCode keyPushed)
    {
        DirectionEnum.Direction expectedDirection = DirectionEnum.Direction.LEFT;

        if(keyPushed == KeyCode.Q || keyPushed == KeyCode.LeftArrow)
        {
            expectedDirection = DirectionEnum.Direction.LEFT;
        }
        else if(keyPushed == KeyCode.D || keyPushed == KeyCode.RightArrow)
        {
            expectedDirection = DirectionEnum.Direction.RIGHT;
        }
        
        if((expectedDirection == DirectionEnum.Direction.LEFT) && this.IsAtTheLeft)
        {
            return true;
        }

        if ((expectedDirection == DirectionEnum.Direction.RIGHT) && this.IsAtTheRight)
        {
            return true;
        }

        return false;
    }

    public bool IsAtTheMiddle
    {
        get
        {
            return isAtTheMiddle;
        }

        set
        {
            isAtTheMiddle = value;
        }
    }

    public bool IsAtTheLeft
    {
        get
        {
            return isAtTheLeft;
        }

        set
        {
            isAtTheLeft = value;
        }
    }

    public bool IsAtTheRight
    {
        get
        {
            return isAtTheRight;
        }

        set
        {
            isAtTheRight = value;
        }
    }

    public float MiddleTargetPosition
    {
        get
        {
            return middleTargetPosition;
        }

        set
        {
            middleTargetPosition = value;
        }
    }

    public float RightTargetPosition
    {
        get
        {
            return rightTargetPosition;
        }

        set
        {
            rightTargetPosition = value;
        }
    }

    public float LeftTargetPosition
    {
        get
        {
            return leftTargetPosition;
        }

        set
        {
            leftTargetPosition = value;
        }
    }

    public bool IsChoiceMade
    {
        get
        {
            return isChoiceMade;
        }

        set
        {
            isChoiceMade = value;
        }
    }

    public bool IsRegistered
    {
        get
        {
            return isRegistered;
        }

        set
        {
            isRegistered = value;
        }
    }

    public bool IsChoiceInputBlocked
    {
        get
        {
            return isChoiceInputBlocked;
        }

        set
        {
            isChoiceInputBlocked = value;
        }
    }

    public bool IsPositionInputBlocked
    {
        get
        {
            return isPositionInputBlocked;
        }

        set
        {
            isPositionInputBlocked = value;
        }
    }

    public bool IsLeftInputPushed
    {
        get
        {
            return isLeftInputPushed;
        }

        set
        {
            isLeftInputPushed = value;
        }
    }

    public bool IsRigthInputPushed
    {
        get
        {
            return isRigthInputPushed;
        }

        set
        {
            isRigthInputPushed = value;
        }
    }
}
