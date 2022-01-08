using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WFCV2_CellInfo 
{
    public bool isCollapsed;
    public Vector3 cellCoordinate;
    public List<WFCV2_SingleState> superPosition = new List<WFCV2_SingleState>();
}
[System.Serializable]
public class WFCV2_SingleState
{
    public GameObject prefab;
    public int rotationIndex;
    public string front_SocketCode;
    public string back_SocketCode;
    public string right_SocketCode;
    public string left_SocketCode;
}
