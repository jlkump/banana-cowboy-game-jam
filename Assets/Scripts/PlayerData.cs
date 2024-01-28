using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public static Vector3 levelCoords; // base level respawn coords
    public static Vector3 checkpt1Coords;
    public static Vector3 checkpt2Coords;
    public static int checkpointsReached; // latest checkpoint reached

    public static void resetData() {
        checkpointsReached = 0;
    }
}
