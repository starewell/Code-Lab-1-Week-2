using UnityEngine;

public class UtilScript  {
   
    public static Vector3 Vector3Modify(Vector3 initVec, float xMod, float yMod, float zMod) {
        Vector3 returnVec = new Vector3(initVec.x + xMod,
                                        initVec.y + yMod,
                                        initVec.z + zMod);
        return returnVec;
    }

}
