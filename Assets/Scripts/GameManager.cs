using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject[] objects;
    public float startWait;
    public float spawnWait;
    public Vector3 spawnPosition;
    private bool isReadyToSpawnObject;

    private void Start()
    {
        IsReadyToSpawnObject = true;
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
