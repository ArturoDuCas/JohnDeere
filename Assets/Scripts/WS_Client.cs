using WebSocketSharp; 
using UnityEngine;
using Newtonsoft.Json;
using System.Text; 
using System.Text.Json;
using System.Collections.Generic; // Add this using directive



public class WS_Client : MonoBehaviour
{
    public WebSocket ws;
    FieldController fieldController;

    [System.Serializable]
    private class FieldMatrixMessage
    {
        public string type;
        public int[,] data;
    }

    void Awake()
    {
        ws = new WebSocket("ws://localhost:8080");
        fieldController = FindObjectOfType<FieldController>();
    }
    void Start()
    {
        

        ws.OnMessage += (sender, e) =>
        {
            Message message = JsonUtility.FromJson<Message>(e.Data);
            if (message.type == "connection_info")
            {
                Debug.Log("ID: " + message.data);
                GlobalData.selfID = message.data;
            } else if (message.type == "connect")
            {
                Debug.Log("Established connection with: " + message.sender);
            } else if (message.type == "field-dimensions")
            {
                getFieldDimensions(message.data);
                Debug.Log(GlobalData.fieldCols.ToString() + GlobalData.fieldRows.ToString());

            } else if (message.type == "starting_harvester_data")
            {
                // fieldController.AssignRouteToHarvesters(message.data); // TODO: message este tipo Mensaje, ver como se va a mandar
            }
            else
            {
                Debug.Log("Mensaje recibido: " + message.data); 
            }
        };
        
        ws.Connect();
    }
    
    void Update()
    {
        if (ws == null)
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ws.Send("Hello");
        }
    }
    
    string MatrixToJson(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        StringBuilder jsonBuilder = new StringBuilder("[");

        for (int i = 0; i < rows; i++)
        {
            jsonBuilder.Append("[");

            for (int j = 0; j < cols; j++)
            {
                jsonBuilder.Append(matrix[i, j]);

                if (j < cols - 1)
                {
                    jsonBuilder.Append(",");
                }
            }

            jsonBuilder.Append("]");

            if (i < rows - 1)
            {
                jsonBuilder.Append(",");
            }
        }

        jsonBuilder.Append("]");

        return jsonBuilder.ToString();
    }
    
    
    void getFieldDimensions(string data)
    {
        string[] parts = data.Split(',');
        foreach (string part in parts)
        {
            if (part.StartsWith("x:"))
            {
                string xValue = part.Substring(2);
                GlobalData.fieldCols = int.Parse(xValue);
            }
            else if (part.StartsWith("z:"))
            {
                string zValue = part.Substring(2);
                GlobalData.fieldRows = int.Parse(zValue);
            }
        }
    }

    public void SendGasCapacity(float gas)
    {
        
        var message = new Message
        {
            type = "gas_capacity", 
            data = gas.ToString()
        }; 

        var jsonMessage = JsonUtility.ToJson(message); 

        ws.Send(jsonMessage);
    }

    public void SendSpeed(float speed)
    {
        var message = new Message
        {
            type = "harvester_speed",
            data = speed.ToString()
        };

        var jsonMessage = JsonUtility.ToJson(message); 

        ws.Send(jsonMessage); 
    }

   // public void SendCampo(string fieldMatrix)
   //  {
   //      var message = new Message
   //      {
   //          type = "field_matrix",
   //          data = fieldMatrix
   //      };
   //
   //      var jsonMessage = JsonUtility.ToJson(message); 
   //
   //      ws.Send(jsonMessage); 
   //  }

    public void SendInitialHarvesterData()
    {
        // fix the starting points matrix
        int[,] harvesterStartingPos = new int[GlobalData.numHarvesters, 2];
        int[] pos = new int[2]; 
        for (int i = 0; i < GlobalData.numHarvesters; i++)
        {
            pos = Common.FixHarvesterPositions(GlobalData.harvesters[i].currentRow, GlobalData.harvesters[i].currentCol);
            harvesterStartingPos[i, 0] = pos[0];
            harvesterStartingPos[i, 1] = pos[1];
        }
        
        string startingPointsJson = MatrixToJson(harvesterStartingPos);    
        string fieldMatrixJson = MatrixToJson(GlobalData.fieldMatrix);
        var message = new Msg_SendInitialHarvesterData
        {
            type = "starting_harvester_data",
            startingPoints = startingPointsJson,
            fieldMatrix = fieldMatrixJson
        };
        
        var jsonMessage = JsonUtility.ToJson(message);
        
        ws.Send(jsonMessage); 
    }

    public void SendHarvesterUnloadRequest(int finalRow, int finalCol)
    {
        string fieldMatrixJson = MatrixToJson(GlobalData.fieldMatrix);
        string finalPosition =  "[" + finalRow.ToString() + "," + finalCol.ToString() + "]";
        
        // Create the trucks initial positions matrix
        int[,] truckInitialPosMatrix = new int[GlobalData.numTrucks, 2];
        int[] initialPos; 
        for (int i = 0; i < GlobalData.numTrucks; i++)
        {
            initialPos = Common.FixTruckPositions(GlobalData.trucks[i].currentRow, GlobalData.trucks[i].currentCol);
            truckInitialPosMatrix[i, 0] = initialPos[0];
            truckInitialPosMatrix[i, 1] = initialPos[1];
        }
        
        string truckInitialPos = MatrixToJson(truckInitialPosMatrix); 
        
        var message = new Msg_HarvesterUnloadRequest
        {
            type = "harvester_unload_request",
            fieldMatrix = fieldMatrixJson,
            finalPos = finalPosition,
            trucksInitialPos = truckInitialPos
        };

        
        var jsonMessage = JsonUtility.ToJson(message);
        
        Debug.Log(jsonMessage); 
        ws.Send(jsonMessage); 
    }



}



