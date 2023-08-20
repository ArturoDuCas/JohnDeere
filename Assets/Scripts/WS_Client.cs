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
}
