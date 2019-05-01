using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PieceMetadatas : MonoBehaviour {

    private int? currentPieceLine = null;
    public bool isExcentered;
    public bool hasSpecificRotationBehaviour;
    public float maxRotateAmplitude;
    private bool isSparkling;
    private bool isPieceReady;

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

    public Quaternion CurrentRotation { get; set; }

    public bool IsExcentered
    {
        get
        {
            return isExcentered;
        }

        set
        {
            isExcentered = value;
        }
    }

    public bool HasSpecificRotationBehaviour
    {
        get
        {
            return hasSpecificRotationBehaviour;
        }

        set
        {
            hasSpecificRotationBehaviour = value;
        }
    }

    public bool IsSparkling
    {
        get
        {
            return isSparkling;
        }

        set
        {
            isSparkling = value;
        }
    }

    public bool IsPieceReady
    {
        get
        {
            return isPieceReady;
        }

        set
        {
            isPieceReady = value;
        }
    }

    public float MaxRotateAmplitude
    {
        get
        {
            return maxRotateAmplitude;
        }

        set
        {
            maxRotateAmplitude = value;
        }
    }
}
