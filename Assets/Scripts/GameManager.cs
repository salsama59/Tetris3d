using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject[] objects;
    public float startWait;
    public float spawnWait;
    public Vector3 spawnPosition;
    private bool isReadyToSpawnObject;
    public GameObject horizontalLine;
    public GameObject verticalLine;
    private Vector3[,] map;
    private GameObject gameField;

    private void Start()
    {
        gameField = GameObject.FindGameObjectWithTag("Background");
        IsReadyToSpawnObject = true;
        this.DefineMapSize();
        //this.BuildFieldGrid();
        this.CreatePositionMap();
    }

    // Update is called once per frame
    void Update () {
        if(IsReadyToSpawnObject)
        {
            StartCoroutine(SpawnObjects());
            IsReadyToSpawnObject = false;
        }
    }

    IEnumerator SpawnObjects()
    {
        yield return new WaitForSeconds(startWait);

       
            
        GameObject piece = objects[Random.Range(0, objects.Length)];
        PieceMovement pieceMovementScript = piece.GetComponent<PieceMovement>();

        GameObject field = GameObject.FindGameObjectWithTag("Background");
        pieceMovementScript.field = field;

        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(piece, spawnPosition, spawnRotation);
        yield return new WaitForSeconds(spawnWait);

    }

    private void BuildFieldGrid()
    {
        Vector3 maxRange = GetFieldMaxRange(gameField);

        //Calcul the halfsize of the field
        maxRange.x *= 0.5f;
        maxRange.y *= 0.5f;
        maxRange.z *= 0.5f;

        Vector3 minRange = new Vector3(maxRange.x * -1, maxRange.y * -1, maxRange.z * -1);


        for (float i = minRange.x + gameField.transform.position.x; i < maxRange.x + gameField.transform.position.x; i++)
        {

            Vector3 objectPosition = new Vector3(0f, 0f, 0f);
            GameObject line = Instantiate(verticalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(i, 0.5f, maxRange.z + gameField.transform.position.z);

            Vector3 lineVerticePosition2 = new Vector3(i, 0.5f, minRange.z + gameField.transform.position.z);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }

        for (float i = minRange.z + gameField.transform.position.z; i < maxRange.z + gameField.transform.position.z; i++)
        {
            Vector3 objectPosition = new Vector3(0f, 0f, 0f);
            GameObject line = Instantiate(horizontalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(maxRange.x + gameField.transform.position.x, 0.5f, i);

            Vector3 lineVerticePosition2 = new Vector3(minRange.x + gameField.transform.position.x, 0.5f, i);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }

    }

    private void CreatePositionMap()
    {
        Vector3 maximumValues = GetFieldMaxRange(gameField);
        for (int k = 0; k < Mathf.RoundToInt(maximumValues.z); k++)
        {
            for (int l = 0; l < Mathf.RoundToInt(maximumValues.x); l++)
            {
                GameMap[k, l] = new Vector3(l + 0.5f, 0.5f, k + 0.5f);
            }
        }
    }

    private Vector3 GetFieldMaxRange(GameObject field)
    {

        MeshFilter fieldRenderer = field.GetComponent<MeshFilter>();
        Vector3 objectSize = fieldRenderer.mesh.bounds.size;
        Vector3 objectScale = field.transform.localScale;

        Vector3 maxRange = new Vector3(
            objectSize.x * objectScale.x,
            objectSize.y * objectScale.y,
            objectSize.z * objectScale.z
            );

        return maxRange;
    }

    private void DefineMapSize()
    {
        GameObject field = GameObject.FindGameObjectWithTag("Background");
        Vector3 fieldRange = GetFieldMaxRange(field);
        GameMap = new Vector3[Mathf.RoundToInt(fieldRange.z), Mathf.RoundToInt(fieldRange.x)];
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

    public Vector3[,] GameMap
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
