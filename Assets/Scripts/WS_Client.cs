using WebSocketSharp; 
using UnityEngine;
using Newtonsoft.Json;
using System.Text.Json;
using System.Collections.Generic; // Add this using directive



public class WS_Client : MonoBehaviour
{
    public WebSocket ws;

    [System.Serializable]
    private class FieldMatrixMessage
    {
        public string type;
        public int[,] data;
    }

    void Awake()
    {
        ws = new WebSocket("ws://localhost:8080");
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

            } else
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
    
    public static string ConvertMatrixToJson(int[,] matrix)
    {
        // Create a class to hold your matrix
        var matrixData = new
        {
            Data = matrix
        };

        // Convert the matrix data to a JSON string
        string json = JsonConvert.SerializeObject(matrixData);

        return json;
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
        string startingPointsJson = ConvertMatrixToJson(GlobalData.harvestersStartingPoints);    
        string fieldMatrixJson = ConvertMatrixToJson(GlobalData.fieldMatrix);
        var message = new Msg_SendStartingPoints
        {
            type = "starting_harvester_data",
            startingPoints = startingPointsJson,
            fieldMatrix = fieldMatrixJson
        };
        
        var jsonMessage = JsonUtility.ToJson(message);
        
        ws.Send(jsonMessage); 
    }



}



