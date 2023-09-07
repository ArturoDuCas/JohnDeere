using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp; 


public class WsRequester : MonoBehaviour
{
    private bool isDataSent = false;
    private WS_Client wsClient;
    private WebSocket ws;
    
    
    void Start()
    {
        wsClient = FindObjectOfType<WS_Client>(); // Find the WebSocket client script
        ws = wsClient.ws; // Get the WebSocket client script
    }

    void Update()
    {
        if (ws == null || ws.ReadyState != WebSocketState.Open)
        {
            return;
        }

        if (!isDataSent)
        {
            // if(GlobalData.numHarvesters == 2){
            //     // string startingPointsJson = MatrixToJson(harvesterStartingPos);  
            //     int[,,] new_matrix = Common.DivideMatrix(GlobalData.fieldMatrix, harvesterStartingPos);
            // }
            wsClient.SendInitialHarvesterData();
            isDataSent = true;
        }
    }
}


