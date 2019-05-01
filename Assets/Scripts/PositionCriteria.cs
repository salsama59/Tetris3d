using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCriteria
{
    private Vector3 validPosition;
    private Quaternion validRotation;
    private float distance;

    public PositionCriteria(Vector3 validPosition, Quaternion validRotation, float distance)
    {
        ValidPosition = validPosition;
        ValidRotation = validRotation;
        Distance = distance;
    }

    public float Distance
    {
        get
        {
            return distance;
        }

        set
        {
            distance = value;
        }
    }

    public Vector3 ValidPosition
    {
        get
        {
            return validPosition;
        }

        set
        {
            validPosition = value;
        }
    }

    public Quaternion ValidRotation
    {
        get
        {
            return validRotation;
        }

        set
        {
            validRotation = value;
        }
    }
}
