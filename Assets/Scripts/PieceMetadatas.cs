using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PieceMetadatas : MonoBehaviour {

    private int? currentPieceLine = null;
    private Quaternion currentRotation;

    private void FixedUpdate()
    {
        this.CurrentPieceLine = (int)Math.Round(this.transform.position.z - 0.5f);
    }

    private void LateUpdate()
    {
        this.CurrentRotation = this.transform.rotation;
    }

    public int? CurrentPieceLine
    {
        get
        {
            return currentPieceLine;
        }

        set
        {
            currentPieceLine = value;
        }
    }

    public Quaternion CurrentRotation
    {
        get
        {
            return currentRotation;
        }

        set
        {
            currentRotation = value;
        }
    }
}
