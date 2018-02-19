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

    private void Start()
    {
        IsReadyToSpawnObject = true;
        map = new Vector3[10,10];
        this.BuildFieldGrid();
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
        GameObject field = GameObject.FindGameObjectWithTag("Background");
        MeshFilter fieldRenderer = field.GetComponent<MeshFilter>();
        Vector3 objectHalfSize = fieldRenderer.mesh.bounds.size * 0.5f;
        Vector3 objectScale = field.transform.localScale;

        Vector3 maxRange = new Vector3(
            objectHalfSize.x * objectScale.x,
            objectHalfSize.y * objectScale.y,
            objectHalfSize.z * objectScale.z
            );

        Vector3 minRange = new Vector3(maxRange.x * -1, maxRange.y * -1, maxRange.z * -1);


        for(int i = (int)(minRange.x + field.transform.position.x); i < (int)(maxRange.x + field.transform.position.x); i++)
        {

            Vector3 objectPosition = new Vector3(0f, 0f, 0f);
            GameObject line = Instantiate(verticalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(i, 0.5f, maxRange.z + field.transform.position.z);

            Vector3 lineVerticePosition2 = new Vector3(i, 0.5f, minRange.z + field.transform.position.z);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }

        for (int i = (int)(minRange.z + field.transform.position.z) ; i < (int)(maxRange.z + field.transform.position.z); i++)
        {
            Vector3 objectPosition = new Vector3(0f, 0f, 0f);
            GameObject line = Instantiate(horizontalLine, objectPosition, Quaternion.identity);

            Vector3 lineVerticePosition1 = new Vector3(maxRange.x + field.transform.position.x, 0.5f, i);

            Vector3 lineVerticePosition2 = new Vector3(minRange.x + field.transform.position.x, 0.5f, i);

            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetPosition(0, lineVerticePosition1);
            lineRenderer.SetPosition(1, lineVerticePosition2);
        }


        for(int k = 0; k < 10; k++)
        {
            for(int l = 0; l < 10; l++)
            {
                map[k,l] = new Vector3(l * 0.5f, 0.5f, k * 0.5f);
                Debug.Log("map[" + k + "]" + "[" + l + "] = " + map[k,l]);
                
            }
        }

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
}
