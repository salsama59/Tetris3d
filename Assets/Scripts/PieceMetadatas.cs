using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PieceMetadatas : MonoBehaviour {

    private int? currentPieceLine = null;

    private void FixedUpdate()
    {
        this.CurrentPieceLine = (int)Math.Round(this.transform.position.z - 0.5f);
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
}
