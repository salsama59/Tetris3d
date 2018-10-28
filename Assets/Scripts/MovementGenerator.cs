using UnityEngine;

public class MovementGenerator : MonoBehaviour {

    private GameObject currentSimulatedObject;

    public MovementGenerator(GameObject currentSimulatedObject)
    {
        CurrentSimulatedObject = currentSimulatedObject;
    }

    public void SimulateNextTranslation(Vector3 direction)
    {
        this.CurrentSimulatedObject.transform.Translate(this.CurrentSimulatedObject.transform.position + direction);
    }

    public void SimulateNextRotation(bool isClockwise)
    {
        float yAxeRotation = MovementUtils.rotationAmount;
        PieceMetadatas pieceMetadatas = CurrentSimulatedObject.GetComponent<PieceMetadatas>();

        if (!isClockwise)
        {
            yAxeRotation *= -1;
        }

        //Wanted rotation calculation 
        Quaternion newrotation = Quaternion.Euler(new Vector3(Quaternion.identity.x, yAxeRotation, Quaternion.identity.z));
        //The from rotation
        Quaternion originRotation = CurrentSimulatedObject.transform.rotation;
        //The to rotation
        Quaternion destinationRotation = originRotation * newrotation;
        this.CurrentSimulatedObject.transform.Rotate(destinationRotation.eulerAngles);
    }

    public GameObject CurrentSimulatedObject
    {
        get
        {
            return currentSimulatedObject;
        }

        set
        {
            currentSimulatedObject = value;
        }
    }
}
