using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public const float maxAllowedPlayableLine = 19.5f;
    public GameObject[] objects;
    public float startWait;
    public float spawnWait;
    public Vector3 spawnPosition;
    private bool isReadyToSpawnObject;
    public GameObject horizontalLine;
    public GameObject verticalLine;
    private PositionMapElement[,] map;
    public GameObject gameField;
    public GameObject foreseeWindow;
    //nullable int shorthand
    private int? nextObjectIndex = null;
    public Text restartText;
    public Text gameOverText;
    private bool restart;
    public Transform backgroundTransform;
    private ScoreManager scoreManagerScript;
    private int pieceId;
    public float pieceMovementSpeed;
    public GameObject explosionEffects;
    private GameObject currentGamePiece;
    private bool isDeletingLines;

    private void Start()
    {
        this.restart = false;
        this.restartText.text = "";
        this.gameOverText.text = "";
        Instantiate(gameField, new Vector3(), Quaternion.identity);
        Instantiate(foreseeWindow, foreseeWindow.transform.position, Quaternion.identity);
        IsReadyToSpawnObject = true;
        this.DefineMapSize();
        //this.BuildFieldGrid();
        this.CreatePositionMap();
        GameObject scoreManagerObject = GameObject.FindGameObjectWithTag("ScoreManager");
        scoreManagerScript = scoreManagerObject.GetComponent<ScoreManager>();
        scoreManagerScript.ScoreText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update () {

        if (CurrentGamePiece != null && !IsDeletingLines)
        {
            this.FreezePiece(false, false);
        }
        else if (CurrentGamePiece != null && IsDeletingLines)
        {
            this.FreezePiece(true, true);
        }

        if (IsReadyToSpawnObject)
        {
            StartCoroutine(SpawnObjects());
            IsReadyToSpawnObject = false;
        }

        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Game_Scene");
            }
        }
    }

    private void FreezePiece(bool isPieceMoving, bool pieceKinematicState)
    {
        PieceMovement pieceMovementScript = CurrentGamePiece.GetComponent<PieceMovement>();
        pieceMovementScript.IsMoving = isPieceMoving == false? true : false;
        Rigidbody currentPieceRigidBody = CurrentGamePiece.GetComponent<Rigidbody>();
        currentPieceRigidBody.isKinematic = pieceKinematicState;
    }

    IEnumerator SpawnObjects()
    {
        yield return new WaitForSeconds(startWait);
        GameObject piece = null;

        GameObject foreseePieceObject = GameObject.FindGameObjectWithTag("ForeseePiece");

        //Destroy the former foreseen piece before displaying the new one if it exists
        if(foreseePieceObject != null)
        {
            Destroy(foreseePieceObject);
        }

        //Choose the object to instantiate either the current foreseen or a fresh new one (from start)
        if (this.nextObjectIndex == null)
        {
            piece = objects[UnityEngine.Random.Range(0, objects.Length)];
        }
        else
        {
            piece = objects[(int)this.nextObjectIndex];
        }
        //Calculate initial rotation
        Quaternion spawnRotation = Quaternion.identity;

        //Manage the gameField object
        this.ManageGameFieldObject(piece, spawnRotation);

        //Manage the foreseen object
        this.ManageForeseeObject(spawnRotation);
       
        yield return new WaitForSeconds(spawnWait);
    }

    private void ManageGameFieldObject(GameObject piece, Quaternion spawnRotation)
    {
        PieceMovement pieceMovementScript = piece.GetComponent<PieceMovement>();

        GameObject field = GameObject.FindGameObjectWithTag("Background");
        pieceMovementScript.Field = field;


        GameObject instanciatedPiece = Instantiate(piece, spawnPosition, spawnRotation);

        //Update parent piece name and the children too thank to the pieceId
        this.UpdatePiecesName(instanciatedPiece);

        CurrentGamePiece = instanciatedPiece;

    }

    private void ManageForeseeObject(Quaternion spawnRotation)
    {
        GameObject foreseePiece = null;
        //Randomly select the foreseenObject
        this.nextObjectIndex = UnityEngine.Random.Range(0, objects.Length);
        foreseePiece = objects[(int)this.nextObjectIndex];

        Vector3 foreseePiecePosition = new Vector3(
            foreseeWindow.transform.position.x,
            0.5f,
            foreseeWindow.transform.position.z);

        GameObject instantiateForeseeObject = Instantiate(foreseePiece, foreseePiecePosition, spawnRotation);
        Transform[] childrensTransform =  instantiateForeseeObject.GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in childrensTransform)
        {
            childTransform.gameObject.tag = "ForeseePiece";
        }
        PieceMovement instantiateForeseeObjectPieceMovementScript = instantiateForeseeObject.GetComponent<PieceMovement>();
        instantiateForeseeObjectPieceMovementScript.enabled = false;
        Rigidbody foreseePieceRigidBody = instantiateForeseeObject.GetComponent<Rigidbody>();
        foreseePieceRigidBody.isKinematic = true;
        foreseePieceRigidBody.detectCollisions = false;
    }

    private void BuildFieldGrid()
    {
        Vector3 maxRange = GetFieldMaxRange(backgroundTransform);
        //Calcul the halfsize of the field
        maxRange.x *= 0.5f;
        maxRange.y *= 0.5f;
        maxRange.z *= 0.5f;

        Vector3 minRange = new Vector3(maxRange.x * -1, maxRange.y * -1, maxRange.z * -1);


        for (float i = minRange.x + backgroundTransform.position.x; i < maxRange.x + backgroundTransform.position.x; i++)
        {

            Vector3 objectPosition = new Vector3(0f, 0f, 0f);
            GameObject line = Instantiate(verticalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(i, 0.5f, maxRange.z + backgroundTransform.position.z);

            Vector3 lineVerticePosition2 = new Vector3(i, 0.5f, minRange.z + backgroundTransform.position.z);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }

        for (float i = minRange.z + backgroundTransform.position.z; i < maxRange.z + backgroundTransform.position.z; i++)
        {
            Vector3 objectPosition = new Vector3(0f, 0f, 0f);
            GameObject line = Instantiate(horizontalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(maxRange.x + backgroundTransform.position.x, 0.5f, i);

            Vector3 lineVerticePosition2 = new Vector3(minRange.x + backgroundTransform.position.x, 0.5f, i);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }

    }

    private void CreatePositionMap()
    {
        Vector3 maxRange = GetFieldMaxRange(backgroundTransform);
        for (int k = 0; k < Mathf.RoundToInt(maxRange.z); k++)
        {
            for (int l = 0; l < Mathf.RoundToInt(maxRange.x); l++)
            {
                GameMap[k, l] = new PositionMapElement (new Vector3(l + 0.5f, 0.5f, k + 0.5f), false);
            }
        }

    }

    private Vector3 GetFieldMaxRange(Transform backgroundTransform)
    {

        Vector3 objectSize = backgroundTransform.position;
        Vector3 objectScale = backgroundTransform.localScale;
        Vector3 maxRange = new Vector3(
            objectSize.x * objectScale.x,
            objectSize.y * objectScale.y,
            objectSize.z * objectScale.z
            );
        return maxRange;
    }

    private void DefineMapSize()
    {
        Vector3 maxRange = GetFieldMaxRange(backgroundTransform);
        //Initialise the position matrix for the game elements [lines, collumns]
        GameMap = new PositionMapElement[Mathf.RoundToInt(maxRange.z), Mathf.RoundToInt(maxRange.x)];
    }

    public void DestroyObjectLines()
    {
        //Retrieve lines to destroy
        SortedDictionary<int, List<GameObject>> linesToDestroy = this.FetchLinesToDestroy();

        int numberOfLinesToDestroy = linesToDestroy.Count;

        if (numberOfLinesToDestroy == 0)
        {
            return;
        }

        List<int> linesLimit = new List<int>();

        int processedLineCounter = 0;

        this.TogglePieceChildObjectCollider(false);

        foreach (KeyValuePair<int, List<GameObject>> objectsToDestroy in linesToDestroy)
        {
            //Save the destroyed lines index for later use
            linesLimit.Add(objectsToDestroy.Key);
            //Keep count of the current process line id to calculate the right object position
            processedLineCounter++;
            //Call the coroutine for line destruction
            StartCoroutine(this.SuppressLineRoutine(linesToDestroy, objectsToDestroy, numberOfLinesToDestroy, processedLineCounter));
            //Errase datas about the suppressed lines in the position map
            this.UpdateSuppressedLinesInPositionMap(objectsToDestroy.Key);
        }

        foreach (int lineLimit in linesLimit)
        {
            //The position map should be updated to impact the pieces new positions after going down by numberOfLinesToDestroy
            this.UpdatePositionMapForNewPiecesPosition(lineLimit);
        }

    }

    IEnumerator SuppressLineRoutine(SortedDictionary<int, List<GameObject>> linesToDestroy, KeyValuePair<int, List<GameObject>> objectsToDestroy, int numberOfLinesToDestroy, int processedLineCounter)
    {

        IsDeletingLines = true;
        //Display the current amount of points earned for the line break
        this.scoreManagerScript.DisplayEarnedPoints(numberOfLinesToDestroy, linesToDestroy.Keys.Last());
        //Keep count of the current process line id to calculate the right object position
        processedLineCounter++;
        //Destroy one line
        yield return this.DestroyObjectLine(objectsToDestroy, processedLineCounter, numberOfLinesToDestroy);
        
    }

    IEnumerator DestroyObjectLine(KeyValuePair<int, List<GameObject>> objectsToDestroy, int processedLineCounter, int numberOfLinesToDestroy)
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
        yield return this.MovePiecesDown(objectsToDestroy.Key, processedLineCounter, numberOfLinesToDestroy);
    }

    public void WaitCoroutine(IEnumerator func)
    {
        while (func.MoveNext())
        {
            if (func.Current != null)
            {
                IEnumerator num;
                try
                {
                    num = (IEnumerator)func.Current;
                }
                catch (InvalidCastException)
                {
                    if (func.Current.GetType() == typeof(WaitForSeconds))
                        Debug.LogWarning("Skipped call to WaitForSeconds. Use WaitForSecondsRealtime instead.");
                    return;  // Skip WaitForSeconds, WaitForEndOfFrame and WaitForFixedUpdate
                }
                WaitCoroutine(num);
            }
        }
    }

    private SortedDictionary<int, List<GameObject>> FetchLinesToDestroy()
    {
        //Sort the line to destroy by line number
        SortedDictionary<int, List<GameObject>> totalObjectListToDestroy = new SortedDictionary<int, List<GameObject>>();

        for(int i = 0; i < (int)(maxAllowedPlayableLine + 0.5f); i++)
        {
            //Find the game object on the current map line
            GameObject[] listToDestroy = GameObject.FindGameObjectsWithTag("PieceChild")
                .Where(pieceChildObject => this.IsGameObjectOnLine(ref pieceChildObject, i))
                .ToArray();
            //If the gameObject unmber match the field width it is a complete destroyable line
            if (this.GameMap.GetLength(1) == listToDestroy.Length)
            {
                totalObjectListToDestroy.Add(i, listToDestroy.ToList());
            }

        }

        return totalObjectListToDestroy;
    }

    private bool IsGameObjectOnLine(ref GameObject targetObject, int lineNumber)
    {
        int? targetObjectLineNumber = null;
        if (!targetObject.CompareTag("ForeseePiece"))
        {
            PieceMetadatas targetObjectScriptPieceMetadatas = targetObject.GetComponent<PieceMetadatas>();
            targetObjectLineNumber = (int)(targetObjectScriptPieceMetadatas.CurrentPieceLine);
        }
        
        return targetObjectLineNumber == lineNumber && LayerMask.LayerToName(targetObject.layer).Equals("DestroyablePiece");
    }

    IEnumerator MovePiecesDown(int lineLimit, int processedLineCounter, int numberOfLinesToDestroy)
    {

        GameObject[] objects = GameObject.FindGameObjectsWithTag("PieceChild")
                               .Where(pieceObject => LayerMask.LayerToName(pieceObject.layer).Equals("DestroyablePiece"))
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
        this.scoreManagerScript.PointsText.gameObject.SetActive(false);
        //Display new calculated score
        this.scoreManagerScript.AddPlayerPointAmountToScore(numberOfLinesToDestroy);
        IsDeletingLines = false;
        this.TogglePieceChildObjectCollider(true);

        yield return null;

    }


    IEnumerator LowerPiecePosition(GameObject currentObject, Vector3 positionGap)
    {
        float targetPosition = currentObject.transform.position.z + positionGap.z;
        while (true)
        {
            yield return new WaitForSeconds(0.06f);

            Vector3 newCalculatedPosition = currentObject.transform.position + positionGap;

            currentObject.transform.position = Vector3.MoveTowards(currentObject.transform.position, newCalculatedPosition, 0.5f);

            if (currentObject.transform.position.z <= targetPosition)
            {
                yield break;
            }
        }
    }

    private void UpdatePositionMapForNewPiecesPosition(int lineLimit)
    {
        for (int i = 0; i < this.GameMap.GetLength(0); i++)
        {

            if (i < lineLimit)
            {
                continue;
            }

            for (int j = 0; j < this.GameMap.GetLength(1); j++)
            {
                PositionMapElement currentElement = GameMap[i, j];

                if(currentElement.IsOccupied && currentElement.CurrentMapElement != null)
                {
                    //Update the below element
                    GameMap[i - 1, j].CurrentMapElement = currentElement.CurrentMapElement;
                    GameMap[i - 1, j].IsOccupied = true;

                    //initialise current element
                    currentElement.IsOccupied = false;
                    currentElement.CurrentMapElement = null;

                }
            }
        }
    }

    private void UpdateSuppressedLinesInPositionMap(int lineLimit)
    {
        //for every column in the map
        for (int j = 0; j < this.GameMap.GetLength(1); j++)
        {
            PositionMapElement currentElement = GameMap[lineLimit, j];
            //initialise current element
            currentElement.IsOccupied = false;
            currentElement.CurrentMapElement = null;
        }
        
    }

    public GameObject FetchHighestPieceChild()
    {

        for(int i = 0; i < this.GameMap.GetLength(1); i++)
        {
            bool occupied = this.GameMap[(int)(maxAllowedPlayableLine + 0.5f), i].IsOccupied;

            if(occupied)
            {
                return this.GameMap[(int)(maxAllowedPlayableLine + 0.5f), i].CurrentMapElement;
            }
             
        }

        return null;
        
    }

    public bool IsGameOver()
    {

        GameObject highestPieceChild = this.FetchHighestPieceChild();

        if(highestPieceChild == null)
        {
            return false;
        }

        int currentHighestPieceChildLine = (int)(highestPieceChild.transform.position.z - 0.5f);

        return currentHighestPieceChildLine > maxAllowedPlayableLine;

    }

    public void GameOver()
    {
        this.gameOverText.text = "Game Over";
        this.restartText.text = "Press 'Return' for restart";
        this.restart = true;
        this.gameOverText.gameObject.SetActive(true);
        this.restartText.gameObject.SetActive(true);
    }

    public void CleanUpPieceObject(GameObject parent)
    {
        parent.transform.DetachChildren();
        Destroy(parent);
        this.CurrentGamePiece = null;
    }

    private void TogglePieceChildObjectCollider(bool activate)
    {
        GameObject[] pieceChildObjectList = GameObject.FindGameObjectsWithTag("PieceChild");

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

    public bool IsReadyToSpawnObject
    {
        get
        {
            return isReadyToSpawnObject;
        }

        set
        {
            isReadyToSpawnObject = value;
        }
    }

    public PositionMapElement[,] GameMap
    {
        get
        {
            return map;
        }

        set
        {
            map = value;
        }
    }

    public GameObject CurrentGamePiece
    {
        get
        {
            return currentGamePiece;
        }

        set
        {
            currentGamePiece = value;
        }
    }

    public bool IsDeletingLines
    {
        get
        {
            return isDeletingLines;
        }

        set
        {
            isDeletingLines = value;
        }
    }
}
