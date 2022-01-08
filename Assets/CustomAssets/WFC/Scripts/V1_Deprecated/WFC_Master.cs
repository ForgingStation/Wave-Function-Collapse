using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WFC_Master : MonoBehaviour
{
    public List<GameObject> allPrefabs;
    public GameObject exceptionPrefab;
    public float cellSize;
    public float spawnEvery;
    public Vector3 grid;
    public int randomPositions;
    public bool studyMode;

    private Dictionary<Vector3, WFC_PrototypeList> allCells = new Dictionary<Vector3, WFC_PrototypeList>();
    private Vector3 boundingUnit;
    private List<Vector3> cellsToProcess = new List<Vector3>();
    private List<Vector3> collapsedCoordinate = new List<Vector3>();
    [SerializeField]private WFC_PrototypeList allProtoTypesList = new WFC_PrototypeList();
    [SerializeField] private WFC_Spawned_Data_List allSpawnedPrefab = new WFC_Spawned_Data_List();
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnEvery;
        boundingUnit = new Vector3(cellSize / 2, 0, cellSize / 2);
        if (!studyMode)
        {
            processPrefabs();
            WFC_Constraint.processValidNeighbours(allProtoTypesList);
        }
        else
        {
            allProtoTypesList = WFC_StudyMode.studyPrefabs((int)cellSize);
        }
        //addDefaultProtoType();
        allocatePossiblitySpace();
        //collpaseRandom();
        string json = JsonUtility.ToJson(allProtoTypesList, true);
        File.WriteAllText(Application.dataPath + "/Custom Assets/WFC/Prototype/Prototype.json", json);
    }

    // Update is called once per frame
    void Update()
    {
        /*timer = timer - Time.deltaTime;
        if (timer <= 0 && collapsedCoordinate.Count < allCells.Count)
        {
            getLowestEntropyCellAndSpawn();
            timer = spawnEvery;
        }*/
    }

    private void OnApplicationQuit()
    {
        string json = JsonUtility.ToJson(allSpawnedPrefab, true);
        File.WriteAllText(Application.dataPath + "/Custom Assets/WFC/Prototype/Spawned.json", json);
    }

    private void processPrefabs()
    {
        for (int i = 0; i < allPrefabs.Count; i++)
        {
            allPrefabs[i].transform.position = Vector3.zero;
            allPrefabs[i].transform.rotation = Quaternion.Euler(allPrefabs[i].transform.localEulerAngles.x, allPrefabs[i].transform.localEulerAngles.y + 0, allPrefabs[i].transform.localEulerAngles.z);
            processMesh(allPrefabs[i]);
            allPrefabs[i].transform.rotation = Quaternion.Euler(allPrefabs[i].transform.localEulerAngles.x, allPrefabs[i].transform.localEulerAngles.y + 90, allPrefabs[i].transform.localEulerAngles.z);
            processMesh(allPrefabs[i]);
            allPrefabs[i].transform.rotation = Quaternion.Euler(allPrefabs[i].transform.localEulerAngles.x, allPrefabs[i].transform.localEulerAngles.y + 90, allPrefabs[i].transform.localEulerAngles.z);
            processMesh(allPrefabs[i]);
            allPrefabs[i].transform.rotation = Quaternion.Euler(allPrefabs[i].transform.localEulerAngles.x, allPrefabs[i].transform.localEulerAngles.y + 90, allPrefabs[i].transform.localEulerAngles.z);
            processMesh(allPrefabs[i]);
        }
    }

    private void processMesh(GameObject prefab)
    {
        examineMesh(prefab, (int)prefab.transform.localEulerAngles.y);
    }

    //RLFB - 0123
    private void examineMesh(GameObject prefab, int rotationIndex)
    {
        MeshFilter m = prefab.GetComponentInChildren<MeshFilter>();
        List<Vector3> allPositionsx_posbnd = new List<Vector3>();
        List<Vector3> allPositionsx_negbnd = new List<Vector3>();
        List<Vector3> allPositionsz_posbnd = new List<Vector3>();
        List<Vector3> allPositionsz_negbnd = new List<Vector3>();
        Vector3 roundedVertexPosition = new Vector3();

        for (int i = 0; i < m.sharedMesh.vertices.Length; i++)
        {
            roundedVertexPosition.x = (float)System.Math.Round(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]).x, 1);
            roundedVertexPosition.y = (float)System.Math.Round(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]).y, 1);
            roundedVertexPosition.z = (float)System.Math.Round(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]).z, 1);

            if (!allPositionsx_posbnd.Contains(roundedVertexPosition) && roundedVertexPosition.x == boundingUnit.x)
            {
                allPositionsx_posbnd.Add(roundedVertexPosition);
            }

            if (!allPositionsx_negbnd.Contains(roundedVertexPosition) && roundedVertexPosition.x == -boundingUnit.x)
            {
                allPositionsx_negbnd.Add(roundedVertexPosition);
            }

            if (!allPositionsz_posbnd.Contains(roundedVertexPosition) && roundedVertexPosition.z == boundingUnit.z)
            {
                allPositionsz_posbnd.Add(roundedVertexPosition);
            }

            if (!allPositionsz_negbnd.Contains(roundedVertexPosition) && roundedVertexPosition.z == -boundingUnit.z)
            {
                allPositionsz_negbnd.Add(roundedVertexPosition);
            }
        }

        WFC_ProtoType pt = new WFC_ProtoType();
        pt.prefab = prefab;
        pt.rotationIndex = rotationIndex;
        pt.right_SocketCode = getOrCreateSocketCode(allPositionsx_posbnd);
        pt.left_SocketCode = getOrCreateSocketCode(allPositionsx_negbnd);
        pt.back_SocketCode = getOrCreateSocketCode(allPositionsz_negbnd);
        pt.front_SocketCode = getOrCreateSocketCode(allPositionsz_posbnd);

        allProtoTypesList.wfc_prototypeList.Add(pt);
    }

    private void addDefaultProtoType()
    {
        WFC_ProtoType pt = new WFC_ProtoType();
        Valid_Neighbours n = new Valid_Neighbours();
        n.prefab = exceptionPrefab;
        n.rotationIndex = 0;
        n.front_SocketCode = "-1";
        n.back_SocketCode = "-1";
        n.right_SocketCode = "-1";
        n.left_SocketCode = "-1";

        pt.prefab = exceptionPrefab;
        pt.rotationIndex = 0;
        pt.front_SocketCode = "-1";
        pt.back_SocketCode = "-1";
        pt.right_SocketCode = "-1";
        pt.left_SocketCode = "-1";
        pt.ValidNeighboursList.Add(n);
        allProtoTypesList.wfc_prototypeList.Add(pt);

        foreach (WFC_ProtoType p in allProtoTypesList.wfc_prototypeList)
        {
            pt.ValidNeighboursList.AddRange(p.ValidNeighboursList);
        }

        foreach (WFC_ProtoType p in allProtoTypesList.wfc_prototypeList)
        {
            p.ValidNeighboursList.Add(n);
        }
    }

    private string getOrCreateSocketCode(List<Vector3> source)
    {
        string socketCode = null;
        float hashSocket = 0;

        for (int i = 0; i < source.Count - 1; i++)
        {
            hashSocket += hashVertexPos(new Vector3(Mathf.Abs(source[i].x), Mathf.Abs(source[i].y), Mathf.Abs(source[i].z)));
        }
        socketCode = System.Math.Round(hashSocket, 0).ToString();
        if (socketCode != null)
        {
            return socketCode;
        }
        else
        {
            return "-1";
        }
    }

    private float hashVertexPos(Vector3 pos)
    {
        return ((pos.x * 47) + (pos.y * 53) + (pos.z * 59));
    }

    //Can have invalid neighbour cells
    private void allocatePossiblitySpace()
    {
        for (int i = 0; i <= grid.x; i += (int)cellSize)
        {
            for (int j = 0; j <= grid.z; j += (int)cellSize)
            {
                allCells.Add(new Vector3(i, 0, j), allProtoTypesList);
            }
        }
    }

    private void collpaseRandom()
    {
        List<Vector3> keys = new List<Vector3>(allCells.Keys);
        List<Vector3> randomCoordinates = new List<Vector3>();
        if (randomPositions > keys.Count)
        {
            randomPositions = keys.Count / 2;
        }
        for (int i=0; i<randomPositions; i++)
        {
            Vector3 rand = keys[Random.Range(0, keys.Count)];
            if (!randomCoordinates.Contains(rand))
            {
                randomCoordinates.Add(rand);
            }
        }
        foreach (Vector3 v in randomCoordinates)
        {
            WFC_ProtoType p = allCells[v].wfc_prototypeList[Random.Range(0, allCells[v].wfc_prototypeList.Count)];
            WFC_PrototypeList pl = new WFC_PrototypeList();
            pl.wfc_prototypeList.Add(p);
            allCells[v] = pl;
            cellsToProcess.Add(v);
            collapsedCoordinate.Add(v);
            spawn(v, allCells[v].wfc_prototypeList[Random.Range(0, allCells[v].wfc_prototypeList.Count)]);
        }

    }

    private void spawn(Vector3 position, WFC_ProtoType prototype)
    {
        GameObject pf = GameObject.Instantiate(prototype.prefab, position, Quaternion.identity);
        pf.transform.Rotate(0, prototype.rotationIndex, 0);
        WFC_Spawned_Data wsd = new WFC_Spawned_Data();
        wsd.prefabName = prototype.prefab.name;
        wsd.rotation = pf.transform.rotation;
        wsd.position = position;
        allSpawnedPrefab.wsdList.Add(wsd);
        propogateCollapse();
    }

    private void getLowestEntropyCellAndSpawn()
    {
        int lowestEntropy = allPrefabs.Count * 4;
        Vector3 lowestEntropyCoordinate = new Vector3();
        foreach (Vector3 key in allCells.Keys)
        {
            if (allCells[key].wfc_prototypeList.Count!=0 && allCells[key].wfc_prototypeList.Count < lowestEntropy && !collapsedCoordinate.Contains(key))
            {
                lowestEntropy = allCells[key].wfc_prototypeList.Count;
                lowestEntropyCoordinate = key;
            }
        }
        if (lowestEntropy!=0)
        {
            WFC_ProtoType p = allCells[lowestEntropyCoordinate].wfc_prototypeList[Random.Range(0, lowestEntropy)];
            WFC_PrototypeList pl = new WFC_PrototypeList();
            pl.wfc_prototypeList.Add(p);
            cellsToProcess.Add(lowestEntropyCoordinate);
            collapsedCoordinate.Add(lowestEntropyCoordinate);
            allCells[lowestEntropyCoordinate] = pl;
            spawn(lowestEntropyCoordinate, p);
        }
    }

    private void propogateCollapse()
    {
        List<Vector3> keys = new List<Vector3>(allCells.Keys);
        List<Vector3> processedCells = new List<Vector3>();
        for (int i = 0; i < cellsToProcess.Count; i++)
        {
            WFC_PrototypeList currentCell_WptList = new WFC_PrototypeList();
            allCells.TryGetValue(cellsToProcess[i], out currentCell_WptList);
            processedCells.Add(cellsToProcess[i]);
            for (int j = 0; j < currentCell_WptList.wfc_prototypeList.Count; j++)
            {
                //Right cell
                Vector3 rightCell = new Vector3(cellsToProcess[i].x + cellSize, 0, cellsToProcess[i].z);
                WFC_PrototypeList rightCell_WptList = new WFC_PrototypeList();
                if (!processedCells.Contains(rightCell) && keys.Contains(rightCell) && !collapsedCoordinate.Contains(rightCell) && allCells[rightCell].wfc_prototypeList.Count > 1)
                {
                    allCells.TryGetValue(rightCell, out rightCell_WptList);
                    WFC_PrototypeList newRightCell_WptList = new WFC_PrototypeList();
                    newRightCell_WptList = compareProtoType_Neighbours(currentCell_WptList.wfc_prototypeList[j].ValidNeighboursList, rightCell_WptList);
                    if (!processedCells.Contains(rightCell) && allCells[rightCell].wfc_prototypeList.Count > newRightCell_WptList.wfc_prototypeList.Count)
                    {
                        allCells[rightCell] = newRightCell_WptList;
                        cellsToProcess.Add(rightCell);
                    }
                }
                //Left cell
                Vector3 leftCell = new Vector3(cellsToProcess[i].x - cellSize, 0, cellsToProcess[i].z);
                WFC_PrototypeList leftCell_WptList = new WFC_PrototypeList();
                if (!processedCells.Contains(leftCell) && keys.Contains(leftCell) && !collapsedCoordinate.Contains(leftCell) && allCells[leftCell].wfc_prototypeList.Count > 1)
                {
                    allCells.TryGetValue(leftCell, out leftCell_WptList);
                    WFC_PrototypeList newLeftCell_WptList = new WFC_PrototypeList();
                    newLeftCell_WptList = compareProtoType_Neighbours(currentCell_WptList.wfc_prototypeList[j].ValidNeighboursList, leftCell_WptList);
                    if (!processedCells.Contains(leftCell) && allCells[leftCell].wfc_prototypeList.Count > newLeftCell_WptList.wfc_prototypeList.Count)
                    {
                        allCells[leftCell] = newLeftCell_WptList;
                        cellsToProcess.Add(leftCell);
                    }
                }
                //Front cell
                Vector3 frontCell = new Vector3(cellsToProcess[i].x, 0, cellsToProcess[i].z + cellSize);
                WFC_PrototypeList frontCell_WptList = new WFC_PrototypeList();
                if (!processedCells.Contains(frontCell) && keys.Contains(frontCell) && !collapsedCoordinate.Contains(frontCell) && allCells[frontCell].wfc_prototypeList.Count > 1)
                {
                    allCells.TryGetValue(frontCell, out frontCell_WptList);
                    WFC_PrototypeList newFrontCell_WptList = new WFC_PrototypeList();
                    newFrontCell_WptList = compareProtoType_Neighbours(currentCell_WptList.wfc_prototypeList[j].ValidNeighboursList, frontCell_WptList);
                    if (!processedCells.Contains(frontCell) && allCells[frontCell].wfc_prototypeList.Count > newFrontCell_WptList.wfc_prototypeList.Count)
                    {
                        allCells[frontCell] = newFrontCell_WptList;
                        cellsToProcess.Add(frontCell);
                    }
                }
                //Back cell
                Vector3 backCell = new Vector3(cellsToProcess[i].x, 0, cellsToProcess[i].z - cellSize);
                WFC_PrototypeList backCell_WptList = new WFC_PrototypeList();
                if (!processedCells.Contains(backCell) && keys.Contains(backCell) && !collapsedCoordinate.Contains(backCell) && allCells[backCell].wfc_prototypeList.Count > 1)
                {
                    allCells.TryGetValue(backCell, out backCell_WptList);
                    WFC_PrototypeList newBackCell_WptList = new WFC_PrototypeList();
                    newBackCell_WptList = compareProtoType_Neighbours(currentCell_WptList.wfc_prototypeList[j].ValidNeighboursList, backCell_WptList);
                    if (!processedCells.Contains(backCell) && allCells[backCell].wfc_prototypeList.Count > newBackCell_WptList.wfc_prototypeList.Count)
                    {
                        allCells[backCell] = newBackCell_WptList;
                        cellsToProcess.Add(backCell);
                    }
                }
            }
            cellsToProcess.RemoveAt(i);
        }
    }

    private WFC_PrototypeList compareProtoType_Neighbours(List<Valid_Neighbours> validNeigbours, WFC_PrototypeList prototypes)
    {
        Dictionary<string, WFC_ProtoType> keyVsProtorype = new Dictionary<string, WFC_ProtoType>();
        List<string> protokeys = new List<string>();
        List<string> neighbourKeys = new List<string>();
        WFC_PrototypeList neighboursToRetain = new WFC_PrototypeList();

        foreach (WFC_ProtoType p in prototypes.wfc_prototypeList)
        {
            string key = p.prefab.ToString() + p.rotationIndex.ToString();
            protokeys.Add(key);
            keyVsProtorype.Add(key, p);
        }

        //valid neighbours of current cell
        foreach (Valid_Neighbours v in validNeigbours)
        {
            string key = v.prefab.ToString() + v.rotationIndex.ToString();
            neighbourKeys.Add(key);
        }

        foreach (string pk in protokeys)
        {
            if (neighbourKeys.Contains(pk))
            {
                neighboursToRetain.wfc_prototypeList.Add(keyVsProtorype[pk]);
            }
        }
        return neighboursToRetain;
    }
}
