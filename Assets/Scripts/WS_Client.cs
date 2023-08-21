using WebSocketSharp; 
using UnityEngine;
using System.Text.Json;


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
}
