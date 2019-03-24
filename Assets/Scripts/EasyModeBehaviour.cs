using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyModeBehaviour : ComputerPlayerBehaviour
{

    public override void Awake()
    {
        base.Awake();
    }

    public override IaData CalculateAction(GameObject currentSimulatedObject, int sideId)
    {
        GameObject simulatedObjectClone = PieceUtils.ClonePieceObject(currentSimulatedObject);

        IaData iaInformations = new IaData();

        Transform transform;
        //position map elements => [lines, collumns]
        transform = SimulateMovement(Vector3.right, simulatedObjectClone, sideId);

        if (transform != null)
        {
            iaInformations.TargetPosition = transform.position;
            iaInformations.TargetRotation = transform.rotation;
            Destroy(simulatedObjectClone);
            return iaInformations;
        }

        simulatedObjectClone.transform.SetPositionAndRotation(currentSimulatedObject.transform.position, currentSimulatedObject.transform.rotation);

        transform = SimulateMovement(Vector3.left, simulatedObjectClone, sideId);

        if (transform == null)
        {
            iaInformations.TargetPosition = currentSimulatedObject.transform.position;
            iaInformations.TargetRotation = currentSimulatedObject.transform.rotation;
        }
        else
        {
            iaInformations.TargetPosition = transform.position;
            iaInformations.TargetRotation = transform.rotation;
        }

        Destroy(simulatedObjectClone);
        return iaInformations;
    }

    public override Transform SimulateMovement(Vector3 direction, GameObject objectClone, int playerSide)
    {
        Transform pieceCloneTransform = null;
        bool isCurrentSimulationInProgress = true;
        while (isCurrentSimulationInProgress)
        {
            bool possible = false;
            Quaternion startRotation = objectClone.transform.rotation;

            do
            {

                possible = AiUtils.IsLineGapPossible(objectClone, AiUtils.GetBottomPiecePositions(playerSide, objectClone), playerSide);
                if (possible)
                {
                    if (MovementUtils.IsRotationPossible(objectClone))
                    {
                        MovementGeneratorUtils.SimulateNextRotation(objectClone, true);
                    }
                    else
                    {
                        break;
                    }

                }
                else
                {
                    break;
                }
            }
            while (objectClone.transform.rotation.eulerAngles.y != startRotation.eulerAngles.y);

            if (possible)
            {
                bool isMovePossible = MovementUtils.IsMovementPossible(direction, objectClone);
                if (isMovePossible)
                {
                    MovementGeneratorUtils.SimulateNextTranslation(objectClone, direction);
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return objectClone.transform;
            }

        }

        return pieceCloneTransform;
    }
}
