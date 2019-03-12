using UnityEngine;
using System.Linq;

public class MovementGeneratorUtils : MonoBehaviour {

    public static void SimulateNextTranslation(GameObject currentSimulatedObject, Vector3 direction)
    {
        currentSimulatedObject.transform.SetPositionAndRotation(new Vector3(currentSimulatedObject.transform.position.x + direction.x
            , currentSimulatedObject.transform.position.y + direction.y
            , currentSimulatedObject.transform.position.z + direction.z), currentSimulatedObject.transform.rotation);
    }

    public static void SimulateNextRotation(GameObject currentSimulatedObject, bool isClockwise)
    {

        PieceMetadatas pieceMetadatas = currentSimulatedObject.GetComponent<PieceMetadatas>();

        float yAxeRotation = MovementUtils.rotationAmount;

        if (!isClockwise)
        {
            yAxeRotation *= -1;
        }

        yAxeRotation += Mathf.Round(currentSimulatedObject.transform.rotation.eulerAngles.y);
        float maxRotateAmplitude = 360f;

        if (isClockwise)
        {
            maxRotateAmplitude *= -1;
        }

        yAxeRotation = Mathf.Clamp(yAxeRotation, 0f, maxRotateAmplitude);

        if(yAxeRotation == 360f || yAxeRotation == -360f)
        {
            yAxeRotation = 0f;
        }

        currentSimulatedObject.transform.rotation = Quaternion.AngleAxis(yAxeRotation, Vector3.up);

        if (pieceMetadatas.HasSpecificRotationBehaviour)
        {
            float currentYRotationValue = currentSimulatedObject.transform.rotation.eulerAngles.y;

            if (currentYRotationValue == 90f || currentYRotationValue == 270f)
            {
                currentSimulatedObject.transform.position = currentSimulatedObject.transform.position + (Vector3.right / 2);
            }
            else
            {
                currentSimulatedObject.transform.position = currentSimulatedObject.transform.position + (Vector3.left / 2);
            }
        }
    }
}
