﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private const float INITIAL_PIECE_SPEED = 8f;
    public GameObject[] gamePiecesPool;
    public float startWait;
    public float spawnWait;
    public GameObject horizontalLine;
    public GameObject verticalLine;
    public GameObject gameField;
    public GameObject foreseeWindow;
    public Text restartText;
    public Text gameOverText;
    public Text winnerText;
    private ScoreManager scoreManagerScript;
    private int pieceId;
    public GameObject explosionEffects;
    private Dictionary<int, GameObject> playersField = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> playersForeSeeWindow = new Dictionary<int, GameObject>();
    private Dictionary<int, bool> playersSpawnAuthorisation = new Dictionary<int, bool>();
    private Dictionary<int, int?> playersNextObjectIndex = new Dictionary<int, int?>();
    private Dictionary<int, PositionMapElement[,]> playersPositionMap = new Dictionary<int, PositionMapElement[,]>();
    private Dictionary<int, float> playersPiecesMovementSpeed = new Dictionary<int, float>();
    private Dictionary<int, GameObject> playersCurrentGamePiece = new Dictionary<int, GameObject>();
    private Dictionary<int, bool> playersDeletingLinesState = new Dictionary<int, bool>();
    private Dictionary<int, Quaternion?> playersNextIntantiateRotation = new Dictionary<int, Quaternion?>();
    private Dictionary<int, List<Material>> playersPieceNextColorsList = new Dictionary<int, List<Material>>();
    private List<float> authorizedRotations;
    public Material[] elementalColorList;
    public GameObject victoryEffects;

    private void Start()
    {
        this.scoreManagerScript = GameUtils.FetchScoreManagerScript();
        this.InitialiseAllGameManagerElements();
    }

    // Update is called once per frame
    void Update()
    {
        for (int playerId = 0; playerId < ApplicationUtils.playerNumber; playerId++)
        {
            this.ManagePlayersFrame(playerId);
        }
    }

    private void InitialiseAllGameManagerElements()
    {
        float angle = MovementUtils.rotationMinValue;

        //TODO manage the random rotation on Yand Z axis
        this.AuthorizedRotations = new List<float>();

        while (angle <= 180f)
        {
            this.AuthorizedRotations.Add(angle);
            angle += 180f;
        }

        for (int playerId = 0; playerId < ApplicationUtils.playerNumber; playerId++)
        {
            this.InitialiseGameManagerProperties(playerId);
            this.PrepareGameField(playerId);
            //this.BuildFieldGrid(playerId);
            this.CreatePositionMap(playerId);
        }
        
    }

    private void InitialiseGameManagerProperties(int playerId)
    {
        this.playersSpawnAuthorisation.Add(playerId, true);
        this.playersNextObjectIndex.Add(playerId, null);
        this.playersNextIntantiateRotation.Add(playerId, null);
        this.playersPiecesMovementSpeed.Add(playerId, INITIAL_PIECE_SPEED);
        this.playersCurrentGamePiece.Add(playerId, null);
        this.playersDeletingLinesState.Add(playerId, false);
        this.playersPieceNextColorsList.Add(playerId, new List<Material>());
        this.Restart = false;
        this.gameOverText.text = "";
        this.restartText.text = "";
        this.winnerText.text = "";
        this.DefineMapSize(playerId);
    }

    private void PrepareGameField(int playerId)
    {
        GameObject playerField, playerForeseeWindow;

        if(ApplicationUtils.IsInMultiPlayerMode())
        {
            this.PrepareGameFieldForTwoPlayerMode(playerId, out playerField, out playerForeseeWindow);
        }
        else
        {
            this.PrepareGameFieldForOnePlayerMode(out playerField, out playerForeseeWindow);
        }
        
        this.PlayersField.Add(playerId, playerField);
        this.PlayersForeSeeWindow.Add(playerId, playerForeseeWindow);

    }

    private void PrepareGameFieldForTwoPlayerMode(int playerId, out GameObject playerField, out GameObject playerForeseeWindow)
    {
        float fieldPositionX = 0f;
        float foreseeWindowPositionX = 0f;

        Vector3 fieldsize = ElementType.CalculateGameObjectMaxRange(GameField.transform.GetChild(0).gameObject);

        Vector3 foreseeWindowSize = ElementType.CalculateGameObjectMaxRange(ForeseeWindow.transform.GetChild(0).gameObject);

        String fieldTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            fieldPositionX = (fieldsize.x + GameFieldUtils.FIELD_MARGIN) * -1;
            foreseeWindowPositionX = (GameFieldUtils.FIELD_MARGIN + fieldsize.x + foreseeWindowSize.x / 2) * -1;
            fieldTagName = TagConstants.TAG_NAME_PLAYER_1_FIELD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            fieldPositionX = foreseeWindowSize.x + GameFieldUtils.FIELD_MARGIN;
            foreseeWindowPositionX = GameFieldUtils.FIELD_MARGIN + foreseeWindowSize.x / 2;
            fieldTagName = TagConstants.TAG_NAME_PLAYER_2_FIELD;
        }

        Vector3 fieldPosition = new Vector3(
                fieldPositionX
            , GameField.transform.position.y
            , GameField.transform.position.z
            );
        Vector3 foreseeWindowPosition = new Vector3(
            foreseeWindowPositionX
            , ForeseeWindow.transform.position.y
            , ForeseeWindow.transform.position.z
            );

        this.InstantiateFieldElements(out playerField, out playerForeseeWindow, fieldTagName, fieldPosition, foreseeWindowPosition);
    }

    private void PrepareGameFieldForOnePlayerMode(out GameObject playerField, out GameObject playerForeseeWindow)
    {
        float fieldPositionX = 0f;
        float foreseeWindowPositionX = 0f;
        Vector3 foreseeWindowSize = ElementType.CalculateGameObjectMaxRange(ForeseeWindow.transform.GetChild(0).gameObject);

        String fieldTagName = null;

        fieldPositionX = 0f;
        foreseeWindowPositionX = (foreseeWindowSize.x / 2) * -1;
        fieldTagName = TagConstants.TAG_NAME_PLAYER_1_FIELD;

        Vector3 fieldPosition = new Vector3(
                fieldPositionX
            , GameField.transform.position.y
            , GameField.transform.position.z
            );
        Vector3 foreseeWindowPosition = new Vector3(
            foreseeWindowPositionX
            , ForeseeWindow.transform.position.y
            , ForeseeWindow.transform.position.z
            );

        this.InstantiateFieldElements(out playerField, out playerForeseeWindow, fieldTagName, fieldPosition, foreseeWindowPosition);
    }

    private void InstantiateFieldElements(out GameObject playerField, out GameObject playerForeseeWindow, string fieldTagName, Vector3 fieldPosition, Vector3 foreseeWindowPosition)
    {
        playerField = Instantiate(GameField, fieldPosition, Quaternion.identity);
        playerField.tag = fieldTagName;
        playerForeseeWindow = Instantiate(ForeseeWindow, foreseeWindowPosition, ForeseeWindow.transform.rotation);
    }

    private void ManagePlayersFrame(int currentPlayerId)
    {
        
        if(!this.Restart)
        {
            if (this.playersCurrentGamePiece[currentPlayerId] != null && !this.playersDeletingLinesState[currentPlayerId])
            {
                this.FreezePiece(false, false, currentPlayerId);
            }
            else if (this.playersCurrentGamePiece[currentPlayerId] != null && this.playersDeletingLinesState[currentPlayerId])
            {
                this.FreezePiece(true, true, currentPlayerId);
            }
        }
        
        if (this.playersSpawnAuthorisation[currentPlayerId])
        {
            StartCoroutine(this.SpawnObjects(currentPlayerId));
            this.playersSpawnAuthorisation[currentPlayerId] = false;
        }

        if (Restart)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if(!ApplicationUtils.IsInMultiPlayerMode())
                {
                    SceneManager.LoadScene(SceneConstants.SCENE_NAME_ONE_PLAYER_MODE);
                }
                else
                {
                    SceneManager.LoadScene(SceneConstants.SCENE_NAME_TWO_PLAYER_MODE);
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                SoundUtils.StopSound(TagConstants.TAG_NAME_ONE_PLAYER_MODE_SOUND);
                SoundUtils.StopSound(TagConstants.TAG_NAME_TWO_PLAYER_MODE_SOUND);
                Destroy(GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_SOUND_CONTROLLER));
                SceneManager.LoadScene(SceneConstants.SCENE_NAME_MAIN_MENU_SCENE);
            }
        }
    }

    private void FreezePiece(bool isPieceMoving, bool pieceKinematicState, int playerId)
    {
        GameObject playerPiece = this.playersCurrentGamePiece[playerId];
        if(playerPiece != null)
        {
            PieceController genericParentPieceMovementScript = PieceUtils.FetchCurrentlyActivatedPieceControllerScript(playerPiece);
            genericParentPieceMovementScript.IsMoving = isPieceMoving == false ? true : false;
            Rigidbody currentPieceRigidBody = playerPiece.GetComponent<Rigidbody>();
            currentPieceRigidBody.isKinematic = pieceKinematicState;
        }
    }

    IEnumerator SpawnObjects(int playerId)
    {
        yield return new WaitForSeconds(startWait);
        GameObject piece = null;
        Quaternion randomInstanciationRotation;
        List<Material> randomPieceColors;

        this.DestroyPlayerForeseenObject(playerId);

        piece = this.GetPlayerNextPiece(playerId);
        randomInstanciationRotation = this.GetPieceNextSpawnRotation(playerId);
        randomPieceColors = this.GetPlayerNextPieceMaterial(playerId, piece);

        //Manage the gameField object
        this.ManageGameFieldObject(piece, playerId, randomInstanciationRotation, randomPieceColors);

        //Manage the foreseen object
        this.ManageForeseeObject(playerId);

        yield return new WaitForSeconds(spawnWait);
    }

    private void DestroyPlayerForeseenObject(int playerId)
    {
        GameObject foreseePieceObject = null;

        if ((int)PlayerEnum.PlayerId.PLAYER_1 == playerId)
        {
            foreseePieceObject = GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_PLAYER_1_FORESEE_PIECE);
        }
        else if ((int)PlayerEnum.PlayerId.PLAYER_2 == playerId)
        {
            foreseePieceObject = GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_PLAYER_2_FORESEE_PIECE);
        }

        //Destroy the former foreseen piece before displaying the new one if it exists
        if (foreseePieceObject != null)
        {
            Destroy(foreseePieceObject);
        }
    }

    private Quaternion GetPieceNextSpawnRotation(int playerId)
    {
        Quaternion randomInstanciationRotation;
        //Chose the spawn rotation for the object either the last calculated one or a new calculated
        if (this.PlayersNextIntantiateRotation[playerId] == null)
        {
            //TODO manage the random rotation on the Y and Z axis
            randomInstanciationRotation = Quaternion.Euler(
            Vector3.zero.x,
            this.AuthorizedRotations[UnityEngine.Random.Range(0, AuthorizedRotations.Count)],
            Vector3.zero.z);
        }
        else
        {
            randomInstanciationRotation = (Quaternion)this.PlayersNextIntantiateRotation[playerId];
        }

        return randomInstanciationRotation;
    }

    private List<Material> GetPlayerNextPieceMaterial(int playerId, GameObject piece)
    {
        List<Material> materials = new List<Material>();
        //Choose the object to instantiate either the current foreseen or a fresh new one (from start)

        if(this.PlayersPieceNextColorsList[playerId].Count == 0)
        {
            Transform[] pieceTransformElements = piece.GetComponentsInChildren<Transform>();

            foreach (Transform element in pieceTransformElements)
            {
                if (element.gameObject != piece)
                {
                    materials.Add(elementalColorList[UnityEngine.Random.Range(0, elementalColorList.Length)]);
                }
            }
        }
        else
        {
            materials = this.PlayersPieceNextColorsList[playerId];
        }

        return materials;
    }

    private GameObject GetPlayerNextPiece(int playerId)
    {
        GameObject piece;
        //Choose the object to instantiate either the current foreseen or a fresh new one (from start)
        if (this.playersNextObjectIndex[playerId] == null)
        {
            piece = gamePiecesPool[UnityEngine.Random.Range(0, gamePiecesPool.Length)];
        }
        else
        {
            piece = gamePiecesPool[(int)this.playersNextObjectIndex[playerId]];
        }

        return piece;
    }

    private void ManageGameFieldObject(GameObject piece, int playerId, Quaternion randomInstanciationRotation, List<Material> randomPieceColor)
    {

        PieceMetadatas pieceMetadatas = PieceUtils.FetchPieceMetadataScript(piece);

        float positionCorrection = 0f;

        if(pieceMetadatas.IsExcentered)
        {
            positionCorrection = 0.5f;
        }

        GameObject field = this.PlayersField[playerId];

        Vector3 fieldsize = ElementType.CalculateGameObjectMaxRange(field.transform.transform.GetChild(0).gameObject);

        Vector3 instantiatePosition = new Vector3(
                fieldsize.x / 2 + field.transform.position.x - 0.5f + positionCorrection
                , 0.5f
                , fieldsize.z + field.transform.position.z - 1.5f);
        
        GameObject instanciatedPiece = Instantiate(piece, instantiatePosition, randomInstanciationRotation);


        PieceController genericController = null;

        if (ApplicationUtils.IsTypeComputer(playerId))
        {
            genericController = PieceUtils.FetchPieceComputerPieceControllerScript(instanciatedPiece);
        }
        else
        {
            genericController = PieceUtils.FetchPiecePlayerPieceControllerScript(instanciatedPiece);
        }

        genericController.enabled = true;
        genericController.OwnerId = playerId;
        genericController.Field = field;
        
        for (int i = 0; i < randomPieceColor.Count; i++)
        {
            instanciatedPiece.transform.GetChild(i).GetComponent<MeshRenderer>().material = randomPieceColor[i];
        }

        PieceMetadatas instanciatedPieceMetadataScript = PieceUtils.FetchPieceMetadataScript(instanciatedPiece);

        instanciatedPieceMetadataScript.IsPieceReady = true;

        //Update parent piece name and the children too thank to the pieceId
        this.UpdatePiecesName(instanciatedPiece);
        this.UpdatePieceChildrenTagName(playerId, instanciatedPiece);
        this.playersCurrentGamePiece[playerId] = instanciatedPiece;

    }

    private void ManageForeseeObject(int playerId)
    {
        Quaternion randomInstanciationRotation;
        GameObject foreseePiece = null;
        GameObject foreseeWindow = this.PlayersForeSeeWindow[playerId];
        List<Material> randomPieceColor = null;
        //Randomly select the foreseenObject
        this.playersNextObjectIndex[playerId] = UnityEngine.Random.Range(0, gamePiecesPool.Length);
        foreseePiece = gamePiecesPool[(int)this.playersNextObjectIndex[playerId]];
        //Randomly calculate rotation for the next forsee object
        //TODO manage the rotation for the Y and Z axis
        this.PlayersNextIntantiateRotation[playerId] = Quaternion.Euler(
            Vector3.zero.x,
            this.AuthorizedRotations[UnityEngine.Random.Range(0, AuthorizedRotations.Count)],
            Vector3.zero.z);
        randomInstanciationRotation = (Quaternion)this.PlayersNextIntantiateRotation[playerId];
        //Randomly calculate colors for the next forsee object
        this.PlayersPieceNextColorsList[playerId].Clear();
        Transform[] pieceTransformElements = foreseePiece.GetComponentsInChildren<Transform>();
        foreach (Transform element in pieceTransformElements)
        {
            if (element.gameObject != foreseePiece)
            {
                this.PlayersPieceNextColorsList[playerId].Add(elementalColorList[UnityEngine.Random.Range(0, elementalColorList.Length)]);
            }
        }
        randomPieceColor = this.PlayersPieceNextColorsList[playerId];

        Vector3 foreseePiecePosition = new Vector3(
            foreseeWindow.transform.position.x,
            0.5f,
            foreseeWindow.transform.position.z);

        //Instanciate the foresee piece
        GameObject instantiateForeseeObject = Instantiate(foreseePiece, foreseePiecePosition, randomInstanciationRotation);

        //Update instanciated foresee piece
        Transform[] childrensTransform =  instantiateForeseeObject.GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in childrensTransform)
        {
            if(playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
            {
                childTransform.gameObject.tag = TagConstants.TAG_NAME_PLAYER_1_FORESEE_PIECE;
            }
            else if(playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
            {
                childTransform.gameObject.tag = TagConstants.TAG_NAME_PLAYER_2_FORESEE_PIECE;
            }
        }
        for (int i = 0; i < randomPieceColor.Count; i++)
        {
            instantiateForeseeObject.transform.GetChild(i).GetComponent<MeshRenderer>().material = randomPieceColor[i];
        }

        Rigidbody instantiatedForeseePieceRigidBody = instantiateForeseeObject.GetComponent<Rigidbody>();
        instantiatedForeseePieceRigidBody.isKinematic = true;
        instantiatedForeseePieceRigidBody.detectCollisions = false;

        List<PieceController> pieceControllerList = new List<PieceController>
        {
            PieceUtils.FetchPiecePlayerPieceControllerScript(instantiateForeseeObject),
            PieceUtils.FetchPieceComputerPieceControllerScript(instantiateForeseeObject)
        };

        foreach(PieceController pieceController in pieceControllerList)
        {
            if(pieceController != null)
            {
                pieceController.enabled = false;
                pieceController.IsMoving = false;
            }  
        }
    }

    private void UpdatePieceChildrenTagName(int playerId, GameObject instanciatedPiece)
    {
        Transform[] childTransforms = instanciatedPiece.GetComponentsInChildren<Transform>().Where(childTransform => childTransform.gameObject != instanciatedPiece).ToArray();

        String targetTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD;
        }

        foreach (Transform transform in childTransforms)
        {
            transform.gameObject.tag = targetTagName;
        }
    }

    private void BuildFieldGrid(int playerId)
    {
        GameObject fieldBackground = this.PlayersField[playerId].transform.GetChild(0).gameObject;

        Vector3 maxRange = ElementType.CalculateGameObjectMaxRange(fieldBackground);
        //Calcul the halfsize of the field
        maxRange.x *= 0.5f;
        maxRange.y *= 0.5f;
        maxRange.z *= 0.5f;

        Vector3 minRange = new Vector3(maxRange.x * -1, maxRange.y * -1, maxRange.z * -1);


        for (float i = minRange.x + fieldBackground.transform.position.x; i < maxRange.x + fieldBackground.transform.position.x; i++)
        {

            Vector3 objectPosition = new Vector3(0f, 0f, 0f);
            GameObject line = Instantiate(verticalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(i, 0.5f, maxRange.z + fieldBackground.transform.position.z);

            Vector3 lineVerticePosition2 = new Vector3(i, 0.5f, minRange.z + fieldBackground.transform.position.z);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }

        for (float i = minRange.z + fieldBackground.transform.position.z; i < maxRange.z + fieldBackground.transform.position.z; i++)
        {
            Vector3 objectPosition = Vector3.zero;
            GameObject line = Instantiate(horizontalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(maxRange.x + fieldBackground.transform.position.x, 0.5f, i);

            Vector3 lineVerticePosition2 = new Vector3(minRange.x + fieldBackground.transform.position.x, 0.5f, i);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }

    }

    private void CreatePositionMap(int playerId)
    {
        Vector3 maxRange = ElementType.CalculateGameObjectMaxRange(GameField.transform.GetChild(0).gameObject);
        for (int k = 0; k < Mathf.RoundToInt(maxRange.z); k++)
        {
            for (int l = 0; l < Mathf.RoundToInt(maxRange.x); l++)
            {
                this.PlayersPositionMap[playerId][k, l] = new PositionMapElement (false);
            }
        }

    }

    private void DefineMapSize(int playerId)
    {
        Vector3 maxRange = ElementType.CalculateGameObjectMaxRange(GameField.transform.GetChild(0).gameObject);
        //Initialise the position matrix for the game elements [lines, collumns]
        this.PlayersPositionMap[playerId] = new PositionMapElement[Mathf.RoundToInt(maxRange.z), Mathf.RoundToInt(maxRange.x)];
    }

    public void DestroyObjectLines(int playerId)
    {
        //Retrieve lines to destroy
        SortedDictionary<int, List<GameObject>> linesToDestroy = this.FetchLinesToDestroy(playerId);

        int numberOfLinesToDestroy = linesToDestroy.Count;

        if (numberOfLinesToDestroy == 0)
        {
            return;
        }

        List<int> linesLimit = new List<int>();

        int processedLineCounter = 0;

        this.TogglePieceChildObjectCollider(false, playerId);

        foreach (KeyValuePair<int, List<GameObject>> objectsToDestroy in linesToDestroy)
        {
            //Keep count of the current processed line id to calculate the right object position
            processedLineCounter++;
            //Save the destroyed lines index for later use
            linesLimit.Add(objectsToDestroy.Key);
            //Call the coroutine for line destruction
            StartCoroutine(this.SuppressLineRoutine(linesToDestroy.Keys.Last(), objectsToDestroy, numberOfLinesToDestroy, processedLineCounter, playerId));
            //Errase datas about the suppressed lines in the position map
            this.UpdateSuppressedLinesInPositionMap(objectsToDestroy.Key, playerId);
        }

        foreach (int lineLimit in linesLimit)
        {
            //The position map should be updated to impact the pieces new positions after going down by numberOfLinesToDestroy
            this.UpdatePositionMapForNewPiecesPosition(lineLimit, playerId);
        }

        //Display new calculated score
        this.scoreManagerScript.AddPlayerPointAmountToScore(numberOfLinesToDestroy, playerId);

    }

    IEnumerator SuppressLineRoutine(int linesToDestroyLastKey, KeyValuePair<int, List<GameObject>> objectsToDestroy, int numberOfLinesToDestroy, int processedLineCounter, int playerId)
    {
        //Set to disable somme fonctionnality while deleting lines
        this.playersDeletingLinesState[playerId] = true;
        //Display the current amount of points earned near the last line that will be breaked
        this.scoreManagerScript.DisplayEarnedPoints(numberOfLinesToDestroy, linesToDestroyLastKey, playerId);
        //Destroy one line
        yield return this.DestroyObjectLine(objectsToDestroy, processedLineCounter, numberOfLinesToDestroy, playerId);
        
    }

    IEnumerator DestroyObjectLine(KeyValuePair<int, List<GameObject>> objectsToDestroy, int processedLineCounter, int numberOfLinesToDestroy, int playerId)
    {

        List<GameObject> sortedObjectsToDestroyList = objectsToDestroy.Value.OrderBy(
            currentObject => currentObject.transform.position.x
            ).ToList();

        foreach (GameObject currentObject in sortedObjectsToDestroyList)
        {
            Destroy(currentObject.transform.gameObject);
            Instantiate(explosionEffects, currentObject.transform.position, explosionEffects.transform.rotation);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        //All the relevant pieces going down by one square
        yield return this.MovePiecesDown(objectsToDestroy.Key, processedLineCounter, numberOfLinesToDestroy, playerId);
    }

    private SortedDictionary<int, List<GameObject>> FetchLinesToDestroy(int playerId)
    {
        //Sort the line to destroy by line number
        SortedDictionary<int, List<GameObject>> totalObjectListToDestroy = new SortedDictionary<int, List<GameObject>>();

        String targetTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD;
        }

        for (int i = 0; i < (int)(GameFieldUtils.maxAllowedPlayableLine + 0.5f); i++)
        {
            //Find the game object on the current map line
            GameObject[] listToDestroy = GameObject.FindGameObjectsWithTag(targetTagName)
                .Where(pieceChildObject => this.IsGameObjectOnLine(pieceChildObject, i, playerId))
                .ToArray();
            //If the gameObject number match the field width it is a complete destroyable line
            if (this.PlayersPositionMap[playerId].GetLength(1) == listToDestroy.Length)
            {
                totalObjectListToDestroy.Add(i, listToDestroy.ToList());
            }
        }

        return totalObjectListToDestroy;
    }

    private bool IsGameObjectOnLine(GameObject targetObject, int lineNumber, int playerId)
    {
        int? targetObjectLineNumber = null;

        String targetTagName = null;

        if(playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_1_FORESEE_PIECE;
        }
        else if(playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_2_FORESEE_PIECE;
        }

        if (!targetObject.CompareTag(targetTagName))
        {
            PieceMetadatas targetObjectScriptPieceMetadatas = targetObject.GetComponent<PieceMetadatas>();
            targetObjectLineNumber = (int)(targetObjectScriptPieceMetadatas.CurrentPieceLine);
        }
        
        return targetObjectLineNumber == lineNumber && LayerMask.LayerToName(targetObject.layer).Equals(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE);
    }

    IEnumerator MovePiecesDown(int lineLimit, int processedLineCounter, int numberOfLinesToDestroy, int playerId)
    {
        String targetTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD;
        }

        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTagName)
                               .Where(pieceObject => LayerMask.LayerToName(pieceObject.layer).Equals(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE))
                               .OrderBy(currentObject => currentObject.transform.position.x).ToArray();

        //When there is more than one line to destroy we compensate the lines destroyed by lowering the line limit for each line processed
        if (numberOfLinesToDestroy > 1 && processedLineCounter > 1)
        {
            lineLimit--;
        }

        foreach (GameObject currentObject in objects)
        {
            int currentLine = (int)(currentObject.transform.position.z - 0.5f);

            Vector3 positionGap = Vector3.back;

            if (currentLine >= lineLimit)
            {
                //lower the all pieces in an asynchronous way 
                StartCoroutine(this.LowerPiecePosition(currentObject, positionGap));
            }
        }

        //Hide the line break earned points
        this.scoreManagerScript.PlayersPointText[playerId].gameObject.SetActive(false);
        this.playersDeletingLinesState[playerId] = false;
        this.TogglePieceChildObjectCollider(true, playerId);

        yield return null;

    }


    IEnumerator LowerPiecePosition(GameObject currentObject, Vector3 positionGap)
    {

        float timeWaiting = 0.5f;

        Vector3 newCalculatedPosition = currentObject.transform.position + positionGap;
        currentObject.transform.position = newCalculatedPosition;
       
        yield return new WaitForSecondsRealtime(timeWaiting);
    }

    private void UpdatePositionMapForNewPiecesPosition(int lineLimit, int playerId)
    {
        for (int i = 0; i < this.PlayersPositionMap[playerId].GetLength(0); i++)
        {

            if (i < lineLimit)
            {
                continue;
            }

            for (int j = 0; j < this.PlayersPositionMap[playerId].GetLength(1); j++)
            {
                PositionMapElement currentElement = this.PlayersPositionMap[playerId][i, j];

                if(currentElement.IsOccupied && currentElement.CurrentMapElement != null)
                {
                    //Update the below element
                    this.PlayersPositionMap[playerId][i - 1, j].CurrentMapElement = currentElement.CurrentMapElement;
                    this.PlayersPositionMap[playerId][i - 1, j].IsOccupied = true;

                    //initialise current element
                    currentElement.IsOccupied = false;
                    currentElement.CurrentMapElement = null;

                }
            }
        }
    }

    private void UpdateSuppressedLinesInPositionMap(int lineLimit, int playerId)
    {
        //for every column in the map
        for (int j = 0; j < this.PlayersPositionMap[playerId].GetLength(1); j++)
        {
            PositionMapElement currentElement = this.PlayersPositionMap[playerId][lineLimit, j];
            //initialise current element
            currentElement.IsOccupied = false;
            currentElement.CurrentMapElement = null;
        }
        
    }

    public bool IsGameOver(int playerId)
    {

        GameObject highestPieceChild = GameFieldUtils.FetchHighestPieceInPlayerField(playerId);

        if(highestPieceChild == null)
        {
            return false;
        }

        int currentHighestPieceChildLine = (int)(highestPieceChild.transform.position.z - 0.5f);

        return currentHighestPieceChildLine > GameFieldUtils.maxAllowedPlayableLine;

    }

    public void GameOver(int looserPlayerId)
    {
        Vector3 gameOverTextPosition = CalculateTextScreenPositionToMiddleField(looserPlayerId, 0);

        RectTransform gameOverTextRectTransform = this.gameOverText.GetComponent<RectTransform>();
        gameOverTextRectTransform.position = gameOverTextPosition;

        Vector3 restartTextPosition = CalculateTextScreenPositionToMiddleField(looserPlayerId, -6f);

        RectTransform restartTextRectTransform = this.restartText.GetComponent<RectTransform>();
        restartTextRectTransform.position = restartTextPosition;

        this.gameOverText.text = "GAME OVER";
        this.restartText.text = "Press 'Enter'\n for restart level\n";
        this.restartText.text += "\nPress 'Escape'\n to return to the main menu";
        
        this.Restart = true;
        this.gameOverText.gameObject.SetActive(true);
        this.restartText.gameObject.SetActive(true);
        
    }

    public void DeclareWinner(int winnerPlayerId)
    {
        Vector3 winnerTextPosition = CalculateTextScreenPositionToMiddleField(winnerPlayerId, 0f);

        RectTransform winnerTextRectTransform = this.winnerText.GetComponent<RectTransform>();
        winnerTextRectTransform.position = winnerTextPosition;

        this.winnerText.text = "WINNER";
        this.winnerText.gameObject.SetActive(true);
        this.FreezePiece(true, true, winnerPlayerId);
        Instantiate(victoryEffects, CalculateFireWorkPosition(winnerPlayerId, 0f), victoryEffects.transform.rotation);
    }

    private static Vector3 CalculateTextScreenPositionToMiddleField(int playerId, float offset)
    {
        String fieldTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            fieldTagName = TagConstants.TAG_NAME_PLAYER_1_FIELD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            fieldTagName = TagConstants.TAG_NAME_PLAYER_2_FIELD;
        }

        GameObject field = GameObject.FindGameObjectWithTag(fieldTagName);

        Vector3 fieldsize = ElementType.CalculateGameObjectMaxRange(field.transform.GetChild(0).gameObject);

        Vector3 textTargetWorldPosition = new Vector3(
              field.transform.position.x + fieldsize.x / 2
            , 0.5f
            , field.transform.position.z + fieldsize.z / 2 + offset);

        Vector3 textScreenPosition = Camera.main.WorldToScreenPoint(textTargetWorldPosition);
        return textScreenPosition;
    }

    private Vector3 CalculateFireWorkPosition(int winnerId, float offset)
    {
        String fieldTagName = null;

        if (winnerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            fieldTagName = TagConstants.TAG_NAME_PLAYER_1_FIELD;
        }
        else if (winnerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            fieldTagName = TagConstants.TAG_NAME_PLAYER_2_FIELD;
        }

        GameObject field = GameObject.FindGameObjectWithTag(fieldTagName);

        Vector3 fieldsize = ElementType.CalculateGameObjectMaxRange(field.transform.GetChild(0).gameObject);

        Vector3 fireworksTargetWorldPosition = new Vector3(
              field.transform.position.x + fieldsize.x / 2
            , 0.5f
            , field.transform.position.z + fieldsize.z / 2 + offset);

        return fireworksTargetWorldPosition;
    }

    public void CleanUpPieceObject(GameObject parent, int playerId)
    {
        parent.transform.DetachChildren();
        Destroy(parent);
        this.playersCurrentGamePiece[playerId] = null;
    }

    private void TogglePieceChildObjectCollider(bool activate, int playerId)
    {

        String targetTagName = null;

        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD;
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            targetTagName = TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD;
        }

        GameObject[] pieceChildObjectList = GameObject.FindGameObjectsWithTag(targetTagName);

        foreach (GameObject pieceChildObject in pieceChildObjectList)
        {
            Collider pieceChildObjectCollider = pieceChildObject.GetComponent<BoxCollider>();

            pieceChildObjectCollider.enabled = activate;
        }
    }

    private void UpdatePiecesName(GameObject piece)
    {
        piece.name = this.GenerateParentPieceName(piece);

        Transform[] childrenTransform = piece.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childrenTransform)
        {
            if(childTransform.gameObject != piece)
            {
                childTransform.gameObject.name = this.GenerateChildPieceName(childTransform.gameObject, piece.name);
            }   
        }

        pieceId++;
    }

    private string GenerateParentPieceName(GameObject parentPiece)
    {
        return parentPiece.name.Replace("(Clone)", "") + "_" +  pieceId;
    }

    private string GenerateChildPieceName(GameObject childPiece, string currentParentName)
    {
        return currentParentName + "_" + childPiece.name;
    }

    public Dictionary<int, bool> PlayersSpawnAuthorisation
    {
        get
        {
            return playersSpawnAuthorisation;
        }

        set
        {
            playersSpawnAuthorisation = value;
        }
    }

    public Dictionary<int, PositionMapElement[,]> PlayersPositionMap
    {
        get
        {
            return playersPositionMap;
        }

        set
        {
            playersPositionMap = value;
        }
    }

    public Dictionary<int, float> PlayersPiecesMovementSpeed
    {
        get
        {
            return playersPiecesMovementSpeed;
        }

        set
        {
            playersPiecesMovementSpeed = value;
        }
    }

    public GameObject GameField
    {
        get
        {
            return gameField;
        }

        set
        {
            gameField = value;
        }
    }

    public GameObject ForeseeWindow
    {
        get
        {
            return foreseeWindow;
        }

        set
        {
            foreseeWindow = value;
        }
    }

    public bool Restart { get; set; }

    public List<float> AuthorizedRotations
    {
        get
        {
            return authorizedRotations;
        }

        set
        {
            authorizedRotations = value;
        }
    }

    public Dictionary<int, Quaternion?> PlayersNextIntantiateRotation
    {
        get
        {
            return playersNextIntantiateRotation;
        }

        set
        {
            playersNextIntantiateRotation = value;
        }
    }

    public Dictionary<int, List<Material>> PlayersPieceNextColorsList
    {
        get
        {
            return playersPieceNextColorsList;
        }

        set
        {
            playersPieceNextColorsList = value;
        }
    }

    public Dictionary<int, GameObject> PlayersField
    {
        get
        {
            return playersField;
        }

        set
        {
            playersField = value;
        }
    }

    public Dictionary<int, GameObject> PlayersForeSeeWindow
    {
        get
        {
            return playersForeSeeWindow;
        }

        set
        {
            playersForeSeeWindow = value;
        }
    }

    public Dictionary<int, bool> PlayersDeletingLinesState
    {
        get
        {
            return playersDeletingLinesState;
        }

        set
        {
            playersDeletingLinesState = value;
        }
    }
}
