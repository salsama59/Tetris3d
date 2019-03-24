using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : GenericElementController
{

    protected Rigidbody gameObJectRigidBody;
    protected GameObject field;
    protected float pieceRotationSpeed;
    public GameObject pieceSwingEffect;
    private bool isRotationLocked;

    public override void Awake()
    {
        base.Awake();
    }

    public GameObject Field
    {
        get
        {
            return field;
        }

        set
        {
            field = value;
        }
    }

    public float PieceRotationSpeed
    {
        get
        {
            return pieceRotationSpeed;
        }

        set
        {
            pieceRotationSpeed = value;
        }
    }

    protected bool IsRotationLocked
    {
        get
        {
            return isRotationLocked;
        }

        set
        {
            isRotationLocked = value;
        }
    }
}
