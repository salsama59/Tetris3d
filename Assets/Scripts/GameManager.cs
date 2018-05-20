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
    private GameObject foreseeWindow;
    //nullable int shorthand
    private int? nextObjectIndex = null;
    public Text restartText;
    public Text gameOverText;
    private bool restart;
    public Transform backgroundTransform;
    private ScoreManager scoreManagerScript;

    private void Start()
    {
        this.restart = false;
        this.restartText.text = "";
        this.gameOverText.text = "";
        Instantiate(gameField, new Vector3(), Quaternion.identity);
        this.foreseeWindow = GameObject.FindGameObjectWithTag("ForeseeWindow");
        IsReadyToSpawnObject = true;
        this.DefineMapSize();
        //this.BuildFieldGrid();
        this.CreatePositionMap();
        GameObject scoreManagerObject = GameObject.FindGameObjectWithTag("ScoreManager");
        scoreManagerScript = scoreManagerObject.GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update () {
        if(IsReadyToSpawnObject)
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
            piece = objects[Random.Range(0, objects.Length)];
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
        pieceMovementScript.field = field;

        
        Instantiate(piece, spawnPosition, spawnRotation);
    }

    private void ManageForeseeObject(Quaternion spawnRotation)
    {
        GameObject foreseePiece = null;
        //Randomly select the foreseenObject
        this.nextObjectIndex = Random.Range(0, objects.Length);
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

        foreach (KeyValuePair<int, List<GameObject>> objectsToDestroy in linesToDestroy)
        {
            //Save the destroyed lines index for later use
            linesLimit.Add(objectsToDestroy.Key);
            //Keep count of the current process line id to calculate the right object position
            processedLineCounter++;
            //Destroy one line
            this.DestroyObjectLine(objectsToDestroy.Value);
            //All the relevant pieces going down by one square and parents without child are destroyed
            this.CleanUpParents(objectsToDestroy.Key, processedLineCounter, numberOfLinesToDestroy);
            //Errase datas about the suppressed lines in the position map
            this.UpdateSuppressedLinesInPositionMap(objectsToDestroy.Key);
        }

        this.scoreManagerScript.DisplayEarnedPoints(numberOfLinesToDestroy, linesToDestroy.Keys.Last());

        foreach (int lineLimit in linesLimit)
        {
            //The position map should be updated to impact the pieces new positions after going down by numberOfLinesToDestroy
            this.UpdatePositionMapForNewPiecesPosition(lineLimit);
        }
        // TODO make the text disappear after the destroy animation finished (should take arround 1 or 2 seconds)
        this.scoreManagerScript.pointsText.gameObject.SetActive(false);
        this.scoreManagerScript.AddPlayerPointAmountToScore(numberOfLinesToDestroy);
    }

    private void DestroyObjectLine(List<GameObject> objectsToDestroy)
    {
        foreach (GameObject currentObject in objectsToDestroy)
        {
            this.UpdateParentObjectData(currentObject);
            Destroy(currentObject);
        }
    }

    private SortedDictionary<int, List<GameObject>> FetchLinesToDestroy()
    {
        SortedDictionary<int, List<GameObject>> totalObjectListToDestroy = new SortedDictionary<int, List<GameObject>>();

        for (int i = 0; i < this.GameMap.GetLength(0); i++)
        {
            List<GameObject> listToDestroy = new List<GameObject>();
            for (int j = 0; j < this.GameMap.GetLength(1); j++)
            {
                GameObject element = this.GameMap[i, j].CurrentMapElement;
                bool isOcupied = this.GameMap[i, j].IsOccupied;

                if (element != null && isOcupied)
                {
                    listToDestroy.Add(element);
                }
            }

            if (this.GameMap.GetLength(1) == listToDestroy.Count)
            {
                totalObjectListToDestroy.Add(i, listToDestroy);
            }
        }

        return totalObjectListToDestroy;
    }

    private void UpdateParentObjectData(GameObject childObject)
    {
        Transform childTransform = childObject.GetComponent<Transform>();
        if (childTransform.parent != null && childTransform.parent.gameObject != null)
        {
            GameObject parent = childTransform.parent.gameObject;
            PieceData parentData = parent.GetComponent<PieceData>();

            if (parentData.maxChildNumber != 0 && parentData.childNumberRemaining != 0)
            {
                parentData.childNumberRemaining--;
            }
        }
    }

    private void CleanUpParents(int lineLimit, int processedLineCounter, int numberOfLinesToDestroy)
    {

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Piece");

        //When there is more than one line to destroy we compensate the lines destroyed by lowering the line limit for each line processed
        if (numberOfLinesToDestroy > 1 && processedLineCounter > 1)
        {
            lineLimit--;
        }

        foreach (GameObject currentObject in objects)
        {
            PieceData parentData = currentObject.GetComponent<PieceData>();

            if (parentData.maxChildNumber != 0 && parentData.childNumberRemaining == 0)
            {
                Destroy(currentObject);
                continue;
            }

            int currentLine = (int)(currentObject.transform.position.z - 0.5f);

            Vector3 positionGap = Vector3.back;

            if (currentLine > lineLimit)
            {
                currentObject.transform.position += positionGap;
            }
            else if(currentLine == lineLimit)
            {
                Transform[] childsTransform =  currentObject.GetComponentsInChildren<Transform>();

                foreach (Transform childrenTransform in childsTransform)
                {
                    if(childrenTransform.gameObject != currentObject)
                    {
                        float childZaxePosition = childrenTransform.position.z;
                        float parentZaxePosition = currentObject.transform.position.z;

                        if(childZaxePosition > parentZaxePosition)
                        {
                            childrenTransform.position += positionGap;
                        }
                    }
                }
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

    public GameObject FetchHighestChild(GameObject parent)
    {
        Transform[] childrenTransform = parent.GetComponentsInChildren<Transform>();

        GameObject highestChild = null;

        foreach (Transform childTransform in childrenTransform)
        {

            if (childTransform.gameObject == parent)
            {
                continue;
            }

            if (highestChild == null)
            {
                highestChild = childTransform.gameObject;

                continue;
            }

            if (childTransform.position.z > highestChild.transform.position.z)
            {
                highestChild = childTransform.gameObject;
            }
        }

        return highestChild;
    }

    public bool IsInGameOverState(GameObject parentPiece)
    {
        GameObject highestParentPieceChild = FetchHighestChild(parentPiece);
        int currentParentPieceLine = (int)(parentPiece.transform.position.z - 0.5f);
        int currentHighestParentPieceChildLine = (int)(highestParentPieceChild.transform.position.z - 0.5f);
        return currentParentPieceLine > maxAllowedPlayableLine || currentHighestParentPieceChildLine > maxAllowedPlayableLine;
    }

    public void GameOver()
    {
        this.gameOverText.text = "Game Over";
        this.restartText.text = "Press 'Return' for restart";
        this.restart = true;
        this.gameOverText.gameObject.SetActive(true);
        this.restartText.gameObject.SetActive(true);
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
}
