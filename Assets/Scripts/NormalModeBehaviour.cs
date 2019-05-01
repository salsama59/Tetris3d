using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NormalModeBehaviour : ComputerPlayerBehaviour
{

    public override void Awake()
    {
        base.Awake();
        ValidPositionCriteriaList = new List<PositionCriteria>();
    }

    public override IaData CalculateAction(GameObject currentSimulatedObject, int sideId)
    {
        GameObject simulatedObjectClone = PieceUtils.ClonePieceObject(currentSimulatedObject);

        IaData iaInformations = new IaData();

        Vector3 finalCalculatedPosition;
        Quaternion finalCalculatedRotation;

        this.SimulateMovement(Vector3.right, simulatedObjectClone, sideId);

        simulatedObjectClone.transform.SetPositionAndRotation(currentSimulatedObject.transform.position, currentSimulatedObject.transform.rotation);

        bool isMovePossible = MovementUtils.IsMovementPossible(Vector3.left, simulatedObjectClone);

        if (isMovePossible)
        {
            MovementGeneratorUtils.SimulateNextTranslation(simulatedObjectClone, Vector3.left);
        }

        this.SimulateMovement(Vector3.left, simulatedObjectClone, sideId);

        if(ValidPositionCriteriaList.Count != 0)
        {
            //Order position criterias by distance descending then take the one with the highest distance property(which mean the one with less peace on ground)
            PositionCriteria positionCriteria = ValidPositionCriteriaList.OrderByDescending(criteria => criteria.Distance).First();
            finalCalculatedPosition = positionCriteria.ValidPosition;
            finalCalculatedRotation = positionCriteria.ValidRotation;
        }
        else
        {
            finalCalculatedPosition = currentSimulatedObject.transform.position;
            finalCalculatedRotation = currentSimulatedObject.transform.rotation;
        }
        
        iaInformations.TargetPosition = finalCalculatedPosition;
        iaInformations.TargetRotation = finalCalculatedRotation;

        Destroy(simulatedObjectClone);
        return iaInformations;
    }

    public override Transform SimulateMovement(Vector3 direction, GameObject objectClone, int playerSide)
    {

        Transform pieceCloneTransform = null;
        Quaternion startRotation = objectClone.transform.rotation;
        Vector3 startPosition = objectClone.transform.position;

        do
        {
            while (true)
            {
                bool isGapPossible = AiUtils.IsLineGapPossible(objectClone, AiUtils.GetBottomPiecePositions(playerSide, objectClone), playerSide);

                if (!isGapPossible)
                {
                    this.UpdateValidPositionCriteriaList(objectClone, playerSide);
                }

                if (!MovementUtils.IsMovementPossible(direction, objectClone))
                {
                    break;
                }
                else
                {
                    MovementGeneratorUtils.SimulateNextTranslation(objectClone, direction);
                }
            }

            objectClone.transform.SetPositionAndRotation(startPosition, objectClone.transform.rotation);

            if (MovementUtils.IsRotationPossible(objectClone))
            {
                MovementGeneratorUtils.SimulateNextRotation(objectClone, true);
                startPosition = objectClone.transform.position;
            }
            else
            {
                break;
            }

        }
        while (objectClone.transform.rotation.eulerAngles.y != startRotation.eulerAngles.y);

        return pieceCloneTransform;
    }
}


