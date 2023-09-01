using WebSocketSharp; 
using UnityEngine;
using System.Text.Json;
using System.Collections.Generic; // Add this using directive



public class WS_Client : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");

        ws.OnMessage += (sender, e) =>
        {
            Message message = JsonUtility.FromJson<Message>(e.Data);
            if (message.type == "connection_info")
            {
                Debug.Log("ID: " + message.data);
            } else if (message.type == "connect")
            {
                Debug.Log("Established connection with: " + message.sender);
            } else if (message.type == "field-dimensions")
            {
                getFieldDimensions(message.data);
                Debug.Log(GlobalData.field_xSize.ToString() + GlobalData.field_zSize.ToString());

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

    void getFieldDimensions(string data)
    {
        string[] parts = data.Split(',');
        foreach (string part in parts)
        {
            if (part.StartsWith("x:"))
            {
                string xValue = part.Substring(2);
                GlobalData.field_xSize = int.Parse(xValue);
            }
            else if (part.StartsWith("z:"))
            {
                string zValue = part.Substring(2);
                GlobalData.field_zSize = int.Parse(zValue);
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

    public void SendCampo(int[,] fieldMatrix) {
        var message = new Message {
            type = "field_matrix",
            data = ConvertFieldMatrixToJson(fieldMatrix)
        };

        var jsonMessage = JsonUtility.ToJson(message);

        ws.Send(jsonMessage);
    }

    private string ConvertFieldMatrixToJson(int[,] fieldMatrix) {
        int rows = fieldMatrix.GetLength(0);
        int cols = fieldMatrix.GetLength(1);

        var jsonArray = new List<List<int>>();

        for (int i = 0; i < rows; i++) {
            var rowList = new List<int>();
            for (int j = 0; j < cols; j++) {
                rowList.Add(fieldMatrix[i, j]);
            }
        }

        return JsonUtility.ToJson(jsonArray);
    }
}
