using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComputerPlayerBehaviour : MonoBehaviour
{

    private List<PositionCriteria> validPositionCriteriaList;

    public virtual void Awake()
    {
        this.enabled = false;
    }

    public abstract IaData CalculateAction(GameObject currentSimulatedObject, int sideId);
    public abstract Transform SimulateMovement(Vector3 direction, GameObject objectClone, int playerSide);

    public void UpdateValidPositionCriteriaList(GameObject objectClone, int playerSide)
    {
        float highestDistance = AiUtils.GetHighestDistanceBetweenPieces(playerSide, objectClone);
        PositionCriteria criteria = new PositionCriteria(objectClone.transform.position, objectClone.transform.rotation, highestDistance);
        ValidPositionCriteriaList.Add(criteria);
    }

    public List<PositionCriteria> ValidPositionCriteriaList
    {
        get
        {
            return validPositionCriteriaList;
        }

        set
        {
            validPositionCriteriaList = value;
        }
    }

}