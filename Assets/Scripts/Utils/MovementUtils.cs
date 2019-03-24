using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementUtils : MonoBehaviour {

    public const float rotationMaxValue = 360f;
    public const float rotationMinValue = 0f;
    public const float rotationAmount = 90f;

    public static bool IsMovementPossible(Vector3 movementDirection, GameObject objectToMove)
    {
        List<bool> rayCastHits = new List<bool>();
        float spherePositionAdjustment = movementDirection.x * 0.5f;

        Transform[] childrenTransform = objectToMove.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childrenTransform)
        {
            RaycastHit infos;
            Vector3 sphereOrigin = new Vector3(childTransform.position.x - spherePositionAdjustment, childTransform.position.y, childTransform.position.z);
            bool hasHitten = Physics.SphereCast(sphereOrigin, 0.5f, movementDirection, out infos, 1f, LayerMask.GetMask(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE, LayerConstants.LAYER_NAME_ARENA_WALL));
            rayCastHits.Add(hasHitten);
        }

        //Check if all raycast hit are false (return true if all hit are false but return false otherwise) 
        bool movementAllowed = rayCastHits.ToArray().All(hit => hit == false);

        return movementAllowed;
    }

    public static bool IsRotationPossible(GameObject objectToRotate)
    {
        PieceMetadatas pieceMetadatasScript =  PieceUtils.FetchPieceMetadataScript(objectToRotate);
        List<Vector3> nodes = CalculatePoints(pieceMetadatasScript.MaxRotateAmplitude, objectToRotate);
        return !SweepHasHit(nodes);
    }

    private static  List<Vector3> CalculatePoints(float maxRotateAmplitude, GameObject objectToRotate)
    {
        float radius = maxRotateAmplitude;
        List<Vector3> nodes = new List<Vector3>();
        float calcAngle = 0;
        int segments = 12;
        float curveAmount = rotationMaxValue;

        // Calculate Arc on X-Z    
        for (int i = 0; i < segments + 1; i++)
        {
            float posX = Mathf.Cos(calcAngle * Mathf.Deg2Rad) * radius;
            float posZ = Mathf.Sin(calcAngle * Mathf.Deg2Rad) * radius;
            nodes.Add(objectToRotate.transform.position + (objectToRotate.transform.right * posX) + (objectToRotate.transform.forward * posZ));
            calcAngle += curveAmount / (float)segments;
        }

        return nodes;
    }

    private static bool SweepHasHit(List<Vector3> nodes)
    {
        RaycastHit hit;

        for (int i = 0; i < nodes.Count - 1; i++)
        {
            if (Physics.Linecast(nodes[i], nodes[i + 1], out hit, LayerMask.GetMask(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE, LayerConstants.LAYER_NAME_ARENA_WALL), QueryTriggerInteraction.Collide))
            {
                return true;
            }

        }

        return false;
    }
}
