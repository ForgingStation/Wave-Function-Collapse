using System.Collections.Generic;
using UnityEngine;

public class WFC_StudyMode 
{
    public static Dictionary<Vector3, GameObject> gameObjectVsPos= new Dictionary<Vector3, GameObject>();

    public static WFC_PrototypeList studyPrefabs (int cellSize)
    {
        List<Valid_Neighbours> rightValidNeighbours = new List<Valid_Neighbours>();
        List<Valid_Neighbours> leftValidNeighbours = new List<Valid_Neighbours>();
        List<Valid_Neighbours> backValidNeighbours = new List<Valid_Neighbours>();
        List<Valid_Neighbours> frontValidNeighbours = new List<Valid_Neighbours>();
        Vector3 neighbourCoordinate;
        Valid_Neighbours v;
        WFC_PrototypeList wpl = new WFC_PrototypeList();

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Sample"))
        {
            gameObjectVsPos.Add(g.transform.position, g);
        }

        foreach (KeyValuePair<Vector3, GameObject> gvp in gameObjectVsPos)
        {
            WFC_ProtoType p = new WFC_ProtoType();
            p.prefab = gvp.Value;
            p.rotationIndex = (int)gvp.Value.transform.localEulerAngles.y;

            //Right
            neighbourCoordinate = gvp.Key + new Vector3(cellSize, 0, 0);
            v = getNeighbour(gameObjectVsPos, neighbourCoordinate);
            if (v!=null && !rightValidNeighbours.Contains(v))
            {
                rightValidNeighbours.Add(v);
            }
            if (rightValidNeighbours.Count>0)
            {
                p.ValidNeighboursList = rightValidNeighbours;
            }
            v = null;

            //Left
            neighbourCoordinate = gvp.Key + new Vector3(-cellSize, 0, 0);
            v = getNeighbour(gameObjectVsPos, neighbourCoordinate);
            if (v!=null && !leftValidNeighbours.Contains(v))
            {
                leftValidNeighbours.Add(v);
            }
            if (leftValidNeighbours.Count > 0)
            {
                p.ValidNeighboursList = leftValidNeighbours;
            }
            v = null;

            //Front
            neighbourCoordinate = gvp.Key + new Vector3(0, 0, cellSize);
            v = getNeighbour(gameObjectVsPos, neighbourCoordinate);
            if (v != null && !frontValidNeighbours.Contains(v))
            {
                frontValidNeighbours.Add(v);
            }
            if (frontValidNeighbours.Count > 0)
            {
                p.ValidNeighboursList = frontValidNeighbours;
            }
            v = null;

            //Back
            neighbourCoordinate = gvp.Key + new Vector3(0, 0, -cellSize);
            v = getNeighbour(gameObjectVsPos, neighbourCoordinate);
            if (v != null && !backValidNeighbours.Contains(v))
            {
                backValidNeighbours.Add(v);
            }
            if (backValidNeighbours.Count > 0)
            {
                p.ValidNeighboursList = backValidNeighbours;
            }
            v = null;
            wpl.wfc_prototypeList.Add(p);
        }

        return wpl;
    }

    private static Valid_Neighbours getNeighbour(Dictionary<Vector3, GameObject> gv, Vector3 v)
    {
        Valid_Neighbours vn = new Valid_Neighbours();
        if (gv.ContainsKey(v))
        {
            vn.prefab = gv[v];
            vn.rotationIndex = (int)gv[v].transform.localEulerAngles.y;
        }
        else
        {
            vn = null;
        }
        return vn;
    }
}
