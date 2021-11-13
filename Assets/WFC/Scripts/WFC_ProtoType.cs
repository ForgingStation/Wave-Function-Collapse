using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WFC_ProtoType
{
    public GameObject prefab;
    public int rotationIndex;
    public string front_SocketCode;
    public string back_SocketCode;
    public string right_SocketCode;
    public string left_SocketCode;
    public List<Valid_Neighbours> ValidNeighboursList = new List<Valid_Neighbours>();
}

[System.Serializable]
public class Valid_Neighbours
{
    public GameObject prefab;
    public int rotationIndex;
    public string front_SocketCode;
    public string back_SocketCode;
    public string right_SocketCode;
    public string left_SocketCode;
}
public class WFC_PrototypeList
{
    public List<WFC_ProtoType> wfc_prototypeList = new List<WFC_ProtoType>();
}
