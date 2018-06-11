using System;
using UnityEngine;

public class ElementType {

    public const string ELEMENT_TYPE_PLANE = "Plane";
    public const float ELEMENT_TYPE_BASE_VALUE_PLANE = 10f;
    public const string ELEMENT_TYPE_QUAD = "Quad";
    public const float ELEMENT_TYPE_BASE_VALUE_QUAD = 1f;
    public const string ELEMENT_TYPE_CUBE = "Cube";
    public const float ELEMENT_TYPE_BASE_VALUE_CUBE = 1f;

    public static Vector3 CalculateGameObjectMaxRange(GameObject gameObject)
    {
        String elementType = null;
        MeshFilter objectMeshFilter = gameObject.GetComponent<MeshFilter>();
        elementType = objectMeshFilter.sharedMesh.name;
        float baseValue;

        switch (elementType)
        {
            case ELEMENT_TYPE_PLANE:
                baseValue = ELEMENT_TYPE_BASE_VALUE_PLANE;
                break;
            case ELEMENT_TYPE_CUBE:
                baseValue = ELEMENT_TYPE_BASE_VALUE_CUBE;
                break;
            case ELEMENT_TYPE_QUAD:
                baseValue = ELEMENT_TYPE_BASE_VALUE_QUAD;
                break;
            default:
                baseValue = 0f;
                break;
        }

        Vector3 objectMaxRange = new Vector3(
            gameObject.transform.parent.localScale.x * baseValue
            , 0f
            , gameObject.transform.parent.localScale.z * baseValue);

        return objectMaxRange;

    }
}