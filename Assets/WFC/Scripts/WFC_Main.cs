using System.Collections.Generic;
using UnityEngine;

public class WFC_Main : MonoBehaviour
{
    public List<GameObject> allPrefabs;
    public float cellSize;
    public float spawnEvery;

    private Dictionary<string, List<Vector3>> sockets = new Dictionary<string, List<Vector3>>();
    private List<ProtoType> allProtoTypes = new List<ProtoType>();
    private Dictionary<Vector3, List<ProtoType>> possbilitySpace = new Dictionary<Vector3, List<ProtoType>>();
    private List<Vector3> allValidCoordinates = new List<Vector3>();
    private Dictionary<Vector3, ProtoType> collapsedPrototypes = new Dictionary<Vector3, ProtoType>();
    private int totalCells;
    private float timer;
    private int collapsedCellCount;
    private Vector3 boundingUnit;

    private void Start()
    {
        totalCells = 0;
        timer = spawnEvery;
        collapsedCellCount = 0;
        boundingUnit = new Vector3(cellSize / 2, 0, -cellSize / 2);
        processPrefabs(allPrefabs);
        allocatePossibilitySpace(5, 5);
        //processValidNeightbours(allProtoTypes);
        //collapseRandom();
        //propogateCollapse();
        //selectAndSpawnFromLowestEntropy();
    }

    private void Update()
    {
        timer = timer - Time.deltaTime;
        if (timer <= 0 && collapsedCellCount < allValidCoordinates.Count)
        {
            collapseAdjacentCells();
            selectAndSpawnFromLowestEntropy();
            timer = spawnEvery;
        }
    }

    private void processPrefabs(List<GameObject> allPrefabs)
    {
        for (int i = 0; i < allPrefabs.Count; i++)
        {
            processMesh(allPrefabs[i]);
        }
    }

    private void processMesh(GameObject prefab)
    {
        prefab.transform.position = Vector3.zero;
        prefab.transform.rotation = Quaternion.identity;
        for (int rot = 0; rot < 4; rot++)
        {
            if (rot == 0)
            {
                examineMesh(prefab, 0);
            }
            if (rot == 1)
            {
                prefab.transform.Rotate(0, 90, 0);
                examineMesh(prefab, 90);
            }
            if (rot == 2)
            {
                prefab.transform.Rotate(0, 180, 0);
                examineMesh(prefab, 180);
            }
            if (rot == 3)
            {
                prefab.transform.Rotate(0, 270, 0);
                examineMesh(prefab, 270);
            }
        }
    }

    private void examineMesh(GameObject prefab, int rotationIndex)
    {
        MeshFilter m = prefab.GetComponentInChildren<MeshFilter>();
        List<Vector3> allPositionsx_posbnd = new List<Vector3>();
        List<Vector3> allPositionsx_negbnd = new List<Vector3>();
        List<Vector3> allPositionsz_posbnd = new List<Vector3>();
        List<Vector3> allPositionsz_negbnd = new List<Vector3>();

        for (int i = 0; i < m.sharedMesh.vertices.Length; i++)
        {
            if (!allPositionsx_posbnd.Contains(prefab.transform.TransformPoint(m.sharedMesh.vertices[i])) && System.Math.Round(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]).x, 1) == boundingUnit.x)
            {
                allPositionsx_posbnd.Add(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]));
            }

            if (!allPositionsx_negbnd.Contains(prefab.transform.TransformPoint(m.sharedMesh.vertices[i])) && System.Math.Round(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]).x, 1) == -boundingUnit.x)
            {
                allPositionsx_negbnd.Add(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]));
            }

            if (!allPositionsz_posbnd.Contains(prefab.transform.TransformPoint(m.sharedMesh.vertices[i])) && System.Math.Round(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]).z, 1) == boundingUnit.z)
            {
                allPositionsz_posbnd.Add(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]));
            }

            if (!allPositionsz_negbnd.Contains(prefab.transform.TransformPoint(m.sharedMesh.vertices[i])) && System.Math.Round(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]).z, 1) == -boundingUnit.z)
            {
                allPositionsz_negbnd.Add(prefab.transform.TransformPoint(m.sharedMesh.vertices[i]));
            }
        }

        for (int i = 0; i < allPositionsx_posbnd.Count; i++)
        {
            Debug.Log(allPositionsx_posbnd[i]);
        }

        ProtoType pt = new ProtoType();
        pt.prefab = prefab;
        pt.rotationIndex = rotationIndex;

        for (int i = 0; i < 4; i++)
        {
            bool someSocketFound = false;
            string someSocketKey = "-1";
            string tempSocketKey = "-1";
            foreach (KeyValuePair<string, List<Vector3>> socket in sockets)
            {
                if (i == 0)
                {
                    if (getOrCreateSocketCode(allPositionsx_posbnd) == socket.Key)
                    {
                        someSocketFound = true;
                        someSocketKey = socket.Key;
                        break;
                    }
                }

                if (i == 1)
                {
                    if (getOrCreateSocketCode(allPositionsx_negbnd) == socket.Key)
                    {
                        someSocketFound = true;
                        someSocketKey = socket.Key;
                        break;
                    }
                }

                if (i == 2)
                {
                    if (getOrCreateSocketCode(allPositionsz_posbnd) == socket.Key)
                    {
                        someSocketFound = true;
                        someSocketKey = socket.Key;
                        break;
                    }
                }

                if (i == 3)
                {
                    if (getOrCreateSocketCode(allPositionsz_negbnd) == socket.Key)
                    {
                        someSocketFound = true;
                        someSocketKey = socket.Key;
                        break;
                    }
                }
            }

            if (i == 0)
            {
                if (!someSocketFound)
                {
                    tempSocketKey = getOrCreateSocketCode(allPositionsx_posbnd);
                    sockets.Add(tempSocketKey, allPositionsx_posbnd);
                    pt.right_SocketCode = tempSocketKey;
                }
                else
                {
                    pt.right_SocketCode = someSocketKey;
                }
            }

            if (i == 1)
            {
                if (!someSocketFound)
                {
                    tempSocketKey = getOrCreateSocketCode(allPositionsx_negbnd);
                    sockets.Add(tempSocketKey, allPositionsx_negbnd);
                    pt.left_SocketCode = tempSocketKey;
                }
                else
                {
                    pt.left_SocketCode = someSocketKey;
                }
            }

            if (i == 2)
            {
                if (!someSocketFound)
                {
                    tempSocketKey = getOrCreateSocketCode(allPositionsz_posbnd);
                    sockets.Add(tempSocketKey, allPositionsz_posbnd);
                    pt.front_SocketCode = tempSocketKey;
                }
                else
                {
                    pt.front_SocketCode = someSocketKey;
                }
            }

            if (i == 3)
            {
                if (!someSocketFound)
                {
                    tempSocketKey = getOrCreateSocketCode(allPositionsz_negbnd);
                    sockets.Add(tempSocketKey, allPositionsz_negbnd);
                    pt.back_SocketCode = tempSocketKey;
                }
                else
                {
                    pt.back_SocketCode = someSocketKey;
                }
            }
        }

        //string json = JsonUtility.ToJson(pt);
        //Debug.Log(json);
        allProtoTypes.Add(pt);
    }

    public class ProtoType
    {
        public GameObject prefab;
        public int rotationIndex;
        public string front_SocketCode;
        public string back_SocketCode;
        public string right_SocketCode;
        public string left_SocketCode;
        public List<ProtoType> front_ValidNeighbours = new List<ProtoType>();
        public List<ProtoType> back_ValidNeighbours = new List<ProtoType>();
        public List<ProtoType> right_ValidNeighbours = new List<ProtoType>();
        public List<ProtoType> left_ValidNeighbours = new List<ProtoType>();
    }

    private bool compareLists(List<Vector3> a, List<Vector3> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }
        for (int i = 0; i < a.Count; i++)
        {
            if (
                Mathf.Abs(a[i].x) != Mathf.Abs(b[i].x) ||
                Mathf.Abs(a[i].y) != Mathf.Abs(b[i].y) ||
                Mathf.Abs(a[i].z) != Mathf.Abs(b[i].z)
                )
            {
                return false;
            }
        }
        return true;
    }

    private string getOrCreateSocketCode(List<Vector3> source)
    {
        string socketCode = null;
        List<Vector3> negSource = new List<Vector3>();

        //Mirror
        for (int i = 0; i < source.Count; i++)
        {
            negSource.Add(-source[i]);
        }

        //Unique source key
        for (int i = 0; i < source.Count - 1; i++)
        {
            socketCode += System.Math.Round((source[i + 1] - source[i]).sqrMagnitude, 0).ToString();
        }

        if (compareLists(source, negSource))
        {
            socketCode += "_1";
        }
        else
        {
            socketCode += "_0";
        }

        if (socketCode != null)
        {
            return socketCode;
        }
        else
        {
            return "-1";
        }

    }

    private void allocatePossibilitySpace(int xGridSize, int zGridSize)
    {
        for (int i = 0; i <= xGridSize; i += 5)
        {
            for (int j = 0; j <= zGridSize; j += 5)
            {
                possbilitySpace.Add(new Vector3(i, 0, j), allProtoTypes);
                allValidCoordinates.Add(new Vector3(i, 0, j));
                totalCells++;
            }
        }
    }

    private void collapseRandom()
    {
        List<ProtoType> prots;
        Vector3 randcord = allValidCoordinates[Random.Range(0, allValidCoordinates.Count)];
        possbilitySpace.TryGetValue(randcord, out prots);
        ProtoType randprot = new ProtoType();
        randprot = prots[Random.Range(0, prots.Count)];
        spawnPrefab(randcord, randprot);
    }

    private void spawnPrefab(Vector3 position, ProtoType prot)
    {
        GameObject prefab = prot.prefab;
        GameObject pf = GameObject.Instantiate(prefab, position, Quaternion.identity);
        pf.transform.Rotate(0, prot.rotationIndex, 0);
        collapsedPrototypes.Add(position, prot);
        possbilitySpace.Remove(position);
        collapsedCellCount++;
    }

    private void processValidNeightbours(List<ProtoType> allProtoTypes)
    {
        for (int i = 0; i < allProtoTypes.Count; i++)
        {
            for (int j = 0; j < allProtoTypes.Count; j++)
            {
                if (
                    allProtoTypes[i].front_SocketCode == allProtoTypes[j].front_SocketCode ||
                    allProtoTypes[i].front_SocketCode == allProtoTypes[j].back_SocketCode ||
                    allProtoTypes[i].front_SocketCode == allProtoTypes[j].left_SocketCode ||
                    allProtoTypes[i].front_SocketCode == allProtoTypes[j].right_SocketCode
                    )
                {
                    if (!allProtoTypes[i].front_ValidNeighbours.Contains(allProtoTypes[j]))
                    {
                        allProtoTypes[i].front_ValidNeighbours.Add(allProtoTypes[j]);
                    }
                }

                if (
                    allProtoTypes[i].back_SocketCode == allProtoTypes[j].front_SocketCode ||
                    allProtoTypes[i].back_SocketCode == allProtoTypes[j].back_SocketCode ||
                    allProtoTypes[i].back_SocketCode == allProtoTypes[j].left_SocketCode ||
                    allProtoTypes[i].back_SocketCode == allProtoTypes[j].right_SocketCode
                    )
                {
                    if (!allProtoTypes[i].back_ValidNeighbours.Contains(allProtoTypes[j]))
                    {
                        allProtoTypes[i].back_ValidNeighbours.Add(allProtoTypes[j]);
                    }
                }

                if (
                    allProtoTypes[i].left_SocketCode == allProtoTypes[j].front_SocketCode ||
                    allProtoTypes[i].left_SocketCode == allProtoTypes[j].back_SocketCode ||
                    allProtoTypes[i].left_SocketCode == allProtoTypes[j].left_SocketCode ||
                    allProtoTypes[i].left_SocketCode == allProtoTypes[j].right_SocketCode
                    )
                {
                    if (!allProtoTypes[i].left_ValidNeighbours.Contains(allProtoTypes[j]))
                    {
                        allProtoTypes[i].left_ValidNeighbours.Add(allProtoTypes[j]);
                    }
                }

                if (
                    allProtoTypes[i].right_SocketCode == allProtoTypes[j].front_SocketCode ||
                    allProtoTypes[i].right_SocketCode == allProtoTypes[j].back_SocketCode ||
                    allProtoTypes[i].right_SocketCode == allProtoTypes[j].left_SocketCode ||
                    allProtoTypes[i].right_SocketCode == allProtoTypes[j].right_SocketCode
                    )
                {
                    if (!allProtoTypes[i].right_ValidNeighbours.Contains(allProtoTypes[j]))
                    {
                        allProtoTypes[i].right_ValidNeighbours.Add(allProtoTypes[j]);
                    }
                }
            }
        }
    }

    //Should be recursive
    private void collapseAdjacentCells()
    {
        //Remove any prototype which is not present in the collapsed cells valid neighbours, from neighbours
        foreach (KeyValuePair<Vector3, ProtoType> collapsedPrototype in collapsedPrototypes)
        {
            //Remove from Right
            Vector3 rightCellCoordinate = new Vector3(collapsedPrototype.Key.x + cellSize, collapsedPrototype.Key.y, collapsedPrototype.Key.z);
            collapseCell(rightCellCoordinate, collapsedPrototype.Value, 0);
            //Remove from Left
            Vector3 leftellCoordinate = new Vector3(collapsedPrototype.Key.x - cellSize, collapsedPrototype.Key.y, collapsedPrototype.Key.z);
            collapseCell(leftellCoordinate, collapsedPrototype.Value, 1);
            //Remove from Front
            Vector3 frontellCoordinate = new Vector3(collapsedPrototype.Key.x, collapsedPrototype.Key.y, collapsedPrototype.Key.z + cellSize);
            collapseCell(frontellCoordinate, collapsedPrototype.Value, 2);
            //Remove from back
            Vector3 backellCoordinate = new Vector3(collapsedPrototype.Key.x, collapsedPrototype.Key.y, collapsedPrototype.Key.z - cellSize);
            collapseCell(backellCoordinate, collapsedPrototype.Value, 3);
        }
    }

    //Should be recursive
    private void collapseCell(Vector3 coordinate, ProtoType collapsedProtoType, int directionCode)
    {
        if (allValidCoordinates.Contains(coordinate) && !collapsedPrototypes.ContainsKey(coordinate))
        {
            Debug.Log("coordinate to collapse>> " + coordinate);
            List<ProtoType> cpsValidNeighbours = new List<ProtoType>();
            List<ProtoType> neighboringCellPrototypes = new List<ProtoType>();
            List<ProtoType> neighboursToRemove = new List<ProtoType>();
            if (directionCode == 0)
            {
                cpsValidNeighbours = collapsedProtoType.right_ValidNeighbours;
            }
            if (directionCode == 1)
            {
                cpsValidNeighbours = collapsedProtoType.left_ValidNeighbours;
            }
            if (directionCode == 2)
            {
                cpsValidNeighbours = collapsedProtoType.front_ValidNeighbours;
            }
            if (directionCode == 3)
            {
                cpsValidNeighbours = collapsedProtoType.back_ValidNeighbours;
            }
            possbilitySpace.TryGetValue(coordinate, out neighboringCellPrototypes);
            foreach (ProtoType p in neighboringCellPrototypes)
            {
                if (!cpsValidNeighbours.Contains(p))
                {
                    neighboursToRemove.Add(p);
                }
            }
            foreach (KeyValuePair<Vector3, List<ProtoType>> possibilitySpace in possbilitySpace)
            {
                if (possibilitySpace.Key == coordinate)
                {
                    foreach (ProtoType p in neighboursToRemove)
                    {
                        possibilitySpace.Value.Remove(p);
                    }
                }
            }
        }
    }

    //Should be called after all possible collapses
    private void selectAndSpawnFromLowestEntropy()
    {
        Dictionary<Vector3, int> entropyValues = new Dictionary<Vector3, int>();
        Vector3 lowestEntropyCoordinate = new Vector3();
        int lowestEntropyValue = totalCells;
        foreach (KeyValuePair<Vector3, List<ProtoType>> possibilitySpace in possbilitySpace)
        {
            entropyValues.Add(possibilitySpace.Key, possibilitySpace.Value.Count);
        }
        foreach (KeyValuePair<Vector3, int> entropyValue in entropyValues)
        {
            if (lowestEntropyValue > entropyValue.Value)
            {
                lowestEntropyValue = entropyValue.Value;
                lowestEntropyCoordinate = entropyValue.Key;
            }
        }
        List<ProtoType> p = new List<ProtoType>();
        if (lowestEntropyValue>1 && lowestEntropyValue!= totalCells)
        {
            Debug.Log("Spawning from many");
            Debug.Log("Entropy>> " + lowestEntropyValue);
            Debug.Log("Entropy cord>> " + lowestEntropyCoordinate);

            possbilitySpace.TryGetValue(lowestEntropyCoordinate, out p);
            Debug.Log("Entropy verify size>> "+p.Count);
            spawnPrefab(lowestEntropyCoordinate, p[Random.Range(0, p.Count)]); //Should be based on weight
        }
        else if(lowestEntropyValue != totalCells)
        {
            Debug.Log("Spawning from one>> " + lowestEntropyValue);
            possbilitySpace.TryGetValue(lowestEntropyCoordinate, out p);
            spawnPrefab(lowestEntropyCoordinate, p[0]);
        }
    }
}
