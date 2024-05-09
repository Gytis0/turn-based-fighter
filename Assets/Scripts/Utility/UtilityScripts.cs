using System;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityScripts
{
    static LayerMask movementBlockingObjects = 512;
    static LayerMask charactersMask = 1152;

    public static bool IsPositionValid(Vector3 startPos, Vector2 endPos, Tuple<Vector2, Vector2> gridBoundaries)
    {
        startPos.Set(startPos.x, .5f, startPos.z);
        Vector3 endPos3d = new Vector3(endPos.x, 0.5f, endPos.y);
        float length = Vector3.Distance(startPos, endPos3d);
        RaycastHit[] objectHits = Physics.RaycastAll(startPos, (endPos3d - startPos), length, movementBlockingObjects);
        RaycastHit[] characterHits = Physics.RaycastAll(startPos, (endPos3d - startPos), length, charactersMask);

        if (endPos.x >= gridBoundaries.Item1.x &&
            endPos.x <= gridBoundaries.Item2.x &&
            endPos.y >= gridBoundaries.Item1.y &&
            endPos.y <= gridBoundaries.Item2.y &&
            objectHits.Length == 0 &&
            characterHits.Length < 1) return true;

        return false;
    }

    public static bool IsPositionValid(Vector3 startPos, Vector3 endPos, Tuple<Vector2, Vector2> gridBoundaries)
    {
        startPos.Set(startPos.x, .5f, startPos.z);
        endPos = new Vector3(endPos.x, 0.5f, endPos.z);
        float length = Vector3.Distance(startPos, endPos);
        RaycastHit[] objectHits = Physics.RaycastAll(startPos, (endPos - startPos), length, movementBlockingObjects);
        RaycastHit[] characterHits = Physics.RaycastAll(startPos, (endPos - startPos), length, charactersMask);

        if (endPos.x >= gridBoundaries.Item1.x &&
            endPos.x <= gridBoundaries.Item2.x &&
            endPos.y >= gridBoundaries.Item1.y &&
            endPos.y <= gridBoundaries.Item2.y &&
            objectHits.Length == 0 &&
            characterHits.Length < 1) return true;

        return false;
    }

    public static Dictionary<Direction, Vector3> GetFourDirections(Vector3 position, bool reversed = false, int multiplier = 1)
    {
        int reverse = 1;
        if (reversed) { reverse = -1; }
        reverse *= multiplier;
        return new Dictionary<Direction, Vector3>
        {
            { Direction.Forward, position + Vector3.forward * reverse },
            { Direction.Backward, position + Vector3.back * reverse },
            { Direction.Left, position + Vector3.left * reverse },
            { Direction.Right, position + Vector3.right * reverse }
        };
    }

    public static Direction GetFacingDirection(Quaternion rotation)
    {
        float y = rotation.eulerAngles.y;
        if (y <= 45 || y > 315) return Direction.Forward;
        else if (y <= 135 && y > 45) return Direction.Right;
        else if (y <= 225 && y > 135) return Direction.Backward;
        else return Direction.Left;
    }

    
}
