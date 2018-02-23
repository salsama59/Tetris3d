using UnityEngine;

public class PositionMapElement {

    private Vector3 position;
    private bool isOccupied;

    public PositionMapElement(Vector3 position, bool isOccupied)
    {
        Position = position;
        IsOccupied = isOccupied;
    }

    public bool IsOccupied
    {
        get
        {
            return isOccupied;
        }

        set
        {
            isOccupied = value;
        }
    }

    public Vector3 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }
}
