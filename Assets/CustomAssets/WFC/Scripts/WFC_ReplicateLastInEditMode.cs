using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class WFC_ReplicateLastInEditMode : MonoBehaviour
{
    public bool replicate;
    public List<GameObject> allPrefabs;
    private Dictionary<string, GameObject> instanceNameVsPrefab = new Dictionary<string, GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (replicate && allPrefabs.Count>0)
        {
            replicate = false;
            foreach (GameObject go in allPrefabs)
            {
                if (!instanceNameVsPrefab.ContainsKey(go.name))
                {
                    instanceNameVsPrefab.Add(go.name, go);
                }
            }
            replicate = false;
            WFC_Spawned_Data_List wsdList = new WFC_Spawned_Data_List();
            wsdList = JsonUtility.FromJson<WFC_Spawned_Data_List>(File.ReadAllText(Application.dataPath + "/CustomAssets/WFC/Prototype/Spawned.json"));
            foreach (WFC_Spawned_Data wsd in wsdList.wsdList)
            {
                GameObject objectToInstantiate;
                instanceNameVsPrefab.TryGetValue(wsd.prefabName, out objectToInstantiate);
                GameObject g = GameObject.Instantiate(objectToInstantiate);
                g.transform.position = wsd.position;
                g.transform.rotation = wsd.rotation;
                g.transform.SetParent(this.transform);
            }
        }
    }
}
