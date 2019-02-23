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
       
        float yAxeRotation = MovementUtils.rotationAmount;

        if (!isClockwise)
        {
            yAxeRotation *= -1;
        }

        //Wanted rotation calculation 
        Quaternion newrotation = Quaternion.Euler(new Vector3(Quaternion.identity.x, yAxeRotation, Quaternion.identity.z));
        //The from rotation
        Quaternion originRotation = currentSimulatedObject.transform.rotation;
        //The to rotation
        Quaternion destinationRotation = originRotation * newrotation;
        currentSimulatedObject.transform.Rotate(destinationRotation.eulerAngles);
    }

}
