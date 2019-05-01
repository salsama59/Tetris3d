using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IaData {

    Vector3 targetPosition;
    Quaternion targetRotation;

    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }

        set
        {
            targetPosition = value;
        }
    }

    public Quaternion TargetRotation
    {
        get
        {
            return targetRotation;
        }

        set
        {
            targetRotation = value;
        }
    }
}
