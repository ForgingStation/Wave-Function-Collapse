using UnityEngine;
using System.Collections.Generic;

public class WFC_Constraint
{
    //RLFB - 0123
    public static void processValidNeighbours(WFC_PrototypeList allProtoTypes)
    {
        
        for (int i = 0; i < allProtoTypes.wfc_prototypeList.Count; i++)
        {
            //Compare oppossing sockets
            for (int j = 0; j < allProtoTypes.wfc_prototypeList.Count; j++)
            {
                if (allProtoTypes.wfc_prototypeList[i].right_SocketCode == allProtoTypes.wfc_prototypeList[j].left_SocketCode ||
                    allProtoTypes.wfc_prototypeList[i].left_SocketCode == allProtoTypes.wfc_prototypeList[j].right_SocketCode ||
                    allProtoTypes.wfc_prototypeList[i].front_SocketCode == allProtoTypes.wfc_prototypeList[j].back_SocketCode ||
                    allProtoTypes.wfc_prototypeList[i].back_SocketCode == allProtoTypes.wfc_prototypeList[j].front_SocketCode)
                {
                    Valid_Neighbours neighbour = new Valid_Neighbours();
                    neighbour.prefab = allProtoTypes.wfc_prototypeList[j].prefab;
                    neighbour.front_SocketCode = allProtoTypes.wfc_prototypeList[j].front_SocketCode;
                    neighbour.back_SocketCode = allProtoTypes.wfc_prototypeList[j].back_SocketCode;
                    neighbour.right_SocketCode = allProtoTypes.wfc_prototypeList[j].right_SocketCode;
                    neighbour.left_SocketCode = allProtoTypes.wfc_prototypeList[j].left_SocketCode;
                    neighbour.rotationIndex = allProtoTypes.wfc_prototypeList[j].rotationIndex;

                    if (allProtoTypes.wfc_prototypeList[i].ValidNeighboursList!=null && !allProtoTypes.wfc_prototypeList[i].ValidNeighboursList.Contains(neighbour))
                    {
                        allProtoTypes.wfc_prototypeList[i].ValidNeighboursList.Add(neighbour);
                    }
                }
            }
        }
    }
}
