using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public static Vector3 respawnCoords;
    public static Vector3 checkpointCoords;
    public static int checkpointsReached;

    public static void resetData() {
        checkpointsReached = 0;
        respawnCoords = Vector3.zero;
        checkpointCoords = Vector3.zero;
    }
}
