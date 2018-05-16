using UnityEngine;
using System;

public class PositionMapElement {

    private Vector3 position;
    private bool isOccupied;
    private GameObject currentMapElement;

    public PositionMapElement(Vector3 position, bool isOccupied)
    {
        Position = position;
        IsOccupied = isOccupied;
    }

    public static void WriteMapContentOnConsole(PositionMapElement[,] positionMap)
    {

        String line = "";

        for (int k = positionMap.GetLength(0) - 1; k >= 0; k--)
        {
            String lineNumber = "";

            if (k.ToString().Length == 1)
            {
                lineNumber = 0 + k.ToString() + ",";
            }
            else
            {
                lineNumber = k.ToString() + ",";
            }

            line += lineNumber;

            for (int l = 0; l < positionMap.GetLength(1); l++)
            {
                PositionMapElement currentElement = positionMap[k, l];

                if (currentElement.IsOccupied)
                {
                    line += "O";
                }
                else
                {
                    line += "X";
                }

                if (l == positionMap.GetLength(1) - 1)
                {
                    line += (";" + Environment.NewLine);
                }
                else
                {
                    line += ",";
                }
            }

        }

        Debug.Log(line);

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

    public GameObject CurrentMapElement
    {
        get
        {
            return currentMapElement;
        }

        set
        {
            currentMapElement = value;
        }
    }
}
