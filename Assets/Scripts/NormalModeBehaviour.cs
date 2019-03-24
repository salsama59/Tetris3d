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
                    this.UpdateValidPositionCriteriaList(objectClone, playerSide);

                    if (MovementUtils.IsRotationPossible(objectClone))
                    {
                        MovementGeneratorUtils.SimulateNextRotation(objectClone, true);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            while (objectClone.transform.rotation.eulerAngles.y != startRotation.eulerAngles.y);

            bool isMovePossible = MovementUtils.IsMovementPossible(direction, objectClone);
            if (isMovePossible)
            {
                MovementGeneratorUtils.SimulateNextTranslation(objectClone, direction);
            }
            else
            {

                startRotation = objectClone.transform.rotation;
                bool isSkipTranslationBack = false;

                do
                {
                    if (!isSkipTranslationBack)
                    {
                        //Go in the opposite direction once
                        MovementGeneratorUtils.SimulateNextTranslation(objectClone, direction * -1);
                    }
                    
                    //Rotate piece
                    MovementGeneratorUtils.SimulateNextRotation(objectClone, true);

                    //Verify if movement to former position is possible
                    isMovePossible = MovementUtils.IsMovementPossible(direction, objectClone);
                    if (isMovePossible)
                    {
                        //Move to former position
                        MovementGeneratorUtils.SimulateNextTranslation(objectClone, direction);
                        //Veify if gap possible
                        possible = AiUtils.IsLineGapPossible(objectClone, AiUtils.GetBottomPiecePositions(playerSide, objectClone), playerSide);

                        if (!possible)
                        {
                            //Save the valid position and rotation
                            this.UpdateValidPositionCriteriaList(objectClone, playerSide);
                        }

                        isSkipTranslationBack = false;

                    }
                    else
                    {
                        isSkipTranslationBack = true;
                    }
                }
                while (objectClone.transform.rotation.eulerAngles.y != startRotation.eulerAngles.y);

                return null;
            }

        }

        return pieceCloneTransform;
    }
}


