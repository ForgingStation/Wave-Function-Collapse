using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WFC_Spawned_Data 
{
    public string prefabName;
    public Quaternion rotation;
    public Vector3 position;
}
[System.Serializable]
public class WFC_Spawned_Data_List
{
    public List<WFC_Spawned_Data> wsdList = new List<WFC_Spawned_Data>();
}
