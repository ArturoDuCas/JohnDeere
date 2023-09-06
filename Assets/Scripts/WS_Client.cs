using System;
using WebSocketSharp; 
using UnityEngine;
using Newtonsoft.Json;
using System.Text; 
using UnityEngine.UI;
using System.Text.Json;
using TMPro;

using System.Collections.Generic; // Add this using directive
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class WS_Client : MonoBehaviour
{
        public string jsonString; // Assign the JSON string received from Python


    public WebSocket ws;
    FieldController fieldController;
    public Harvester harvester; 
    public TextMeshProUGUI idText;

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

                idText.text = GlobalData.selfID;

            } else if (message.type == "connect")
            {
                Debug.Log("Established connection with: " + message.sender);
            } else if (message.type == "config_field-dimensionsX")
            {
                Debug.Log("config_field-dimensionsX" + message.data);

            }  else if (message.type == "config_field-dimensionsY")
            {
                Debug.Log("config_field-dimensionsY" + message.data);

            }else if (message.type == "config_harvester_number" ){
                
                Debug.Log("config_harvester_number" + message.data);
                GlobalData.numTrucks = int.Parse(message.data);

            }else if(message.type == "config_field-density"){
                Debug.Log("config_field"+message.data);

            }
            else if(message.type == "config_gas_capacity"){
                Debug.Log("config_gas_capacity" + message.data);
                harvester.fuel = int.Parse(message.data);

            } else if(message.type == "config_harvester_speed"){
                Debug.Log("config_harvester_speed" + message.data);

            }else if (message.type == "python_harvester") {
                //ansljkasndflkadjsn
            }
            else if (message.type == "starting_harvester_data"){
                // fieldController.AssignRouteToHarvesters(message.data); // TODO: message este tipo Mensaje, ver como se va a mandar

            } else if (message.type == "python_harvester")
            {
                // message.data = [(0, 0), (0, 1), (0, 2), (1, 2), (1, 1), (1, 0), (2, 0), (2, 1), (2, 2), (3, 2), (3, 1), (3, 0), (4, 0), (4, 1), (4, 2), (5, 2), (5, 1), (5, 0)] [(0, 1), (0, 0), (1, 0), (1, 1), (1, 2), (0, 2), (1, 2), (2, 2), (2, 1), (2, 0), (3, 0), (3, 1), (3, 2), (4, 2), (4, 1), (4, 0), (5, 0), (5, 1), (5, 2)]
                List<List<Vector2>> paths = new List<List<Vector2>>();
                
                string[] splitPaths = message.data.Split(']');
                List<string> correctSplitPaths = new List<string>();
                for(int i = 0; i < splitPaths.Length; i++)
                {
                    if(splitPaths[i].Length > 3)
                    {
                        string path = splitPaths[i]; 
                        path = path.Replace("[", "");
                        path = path.Replace(" ", "");
                        correctSplitPaths.Add(path);
                    }
                }
                
                foreach(string path in correctSplitPaths)
                {
                    List<Vector2> pathList = new List<Vector2>();
                    for(int i = 0; i < path.Length; i++)
                    {
                        if(path[i] == '(')
                        {
                            string coord = "";
                            i++;
                            while(path[i] != ')')
                            {
                                coord += path[i];
                                i++;
                            }
                            string[] splitCoord = coord.Split(',');
                            Vector2 vectorCoord = new Vector2(int.Parse(splitCoord[0]), int.Parse(splitCoord[1]));
                            pathList.Add(vectorCoord);
                        }
                    }
                    paths.Add(pathList);
                }
                
                for(int i = 0; i < paths.Count; i++)
                {
                    GlobalData.harvesters[i].path = paths[i];   
                }
            } else if (message.type == "truck_python")
            {
                string[] lines = message.data.Split('\n');
                Debug.Log(lines[0]);
                Debug.Log(lines[1]);
                Debug.Log(lines[2]);
            }
            
        };
        
        ws.Connect();
    }
    
    void PrintListOfVector2(List<Vector2> list)
    {
        foreach (Vector2 coord in list)
        {
            Debug.Log(coord);
        }
    }
    
    void PrintListOfListOfVector2(List<List<Vector2>> list)
    {
        foreach (List<Vector2> path in list)
        {
            foreach (Vector2 coord in path)
            {
                Debug.Log(coord);
            }
        }
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
    
    string ListOfVector2ToJson(List<Vector2> list)
    {
        StringBuilder jsonBuilder = new StringBuilder("[");
        
        for (int i = 0; i < list.Count; i++)
        {
            jsonBuilder.Append("[" + list[i].x + "," + list[i].y + "]");
            
            if (i < list.Count - 1)
            {
                jsonBuilder.Append(",");
            }
        }
        
        jsonBuilder.Append("]");
        
        return jsonBuilder.ToString();
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

    public void SendCapacidad(int capacidad)
    {
        var message = new Message
        {
            type = "harvester_capacity",
            data = capacidad.ToString()
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

    public void SendHarvesterUnloadRequest(int finalRow, int finalCol, int id)
    {
        
        
        string fieldMatrixJson = MatrixToJson(GlobalData.fieldMatrix);
        string finalPosition =  "[" + finalRow.ToString() + "," + finalCol.ToString() + "]";
        
        // Create the trucks initial positions matrix
        List<Vector2> truckInitialPosMatrix = new List<Vector2>();
        int[] initialPos; 
        for (int i = 0; i < GlobalData.numTrucks; i++)
        {
            if (!GlobalData.trucks[i].isAviable)
                continue;
            
            initialPos = Common.FixTruckPositions(GlobalData.trucks[i].currentRow, GlobalData.trucks[i].currentCol);
            truckInitialPosMatrix.Add(new Vector2(initialPos[0], initialPos[1]));
        }
        
        string truckInitialPos = ListOfVector2ToJson(truckInitialPosMatrix); 
        
        var message = new Msg_HarvesterUnloadRequest
        {
            type = "harvester_unload_request",
            harvesterId = id.ToString(), 
            fieldMatrix = fieldMatrixJson,
            finalPos = finalPosition,
            trucksInitialPos = truckInitialPos
        };
        
        var jsonMessage = JsonUtility.ToJson(message);

        Debug.Log(jsonMessage); 
        
        ws.Send(jsonMessage); 
    }

    public static Vector2[] ParsePath(string pathString)
    {
        List<Vector2> path = new List<Vector2>();
        // Define a regular expression pattern to match (x, y) pairs
        string pattern = @"\((-?\d+),\s*(-?\d+)\)";
        MatchCollection matches = Regex.Matches(pathString, pattern);

        foreach (Match match in matches)
        {
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);
            path.Add(new Vector2(x, y));
        }

        return path.ToArray();
    }



}



