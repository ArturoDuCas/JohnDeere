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
                GlobalData.fieldCols = int.Parse(message.data);

            }  else if (message.type == "config_field-dimensionsY")
            {
                Debug.Log("config_field-dimensionsY" + message.data);
                GlobalData.fieldRows = int.Parse(message.data);

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
                // first line: harvester id
                // second line: trucks ids
                // the rest: paths of the trucks 

                string[] lines = message.data.Split('\n');
                int harvesterId = int.Parse(lines[0]);
                List<int> truckOptions = JsonConvert.DeserializeObject<List<int>>(lines[1]);
                List<string> truckOptionsPaths = new List<string>();



                // create the list of the paths
                for (int i = 2; i < lines.Length; i++)
                {
                    truckOptionsPaths.Add(lines[i]);
                }


                // fix the truck options for eliminating those with path == [] 
                List<int> validTruckOptions = new List<int>();
                for (int i = 0; i < truckOptions.Count; i++)
                {
                    if (truckOptionsPaths[truckOptions[i]].Length > 4)
                    {
                        validTruckOptions.Add(truckOptions[i]);
                    }
                }


                // get the shortest path
                int truckWithMinRoute = validTruckOptions[0];
                int minRouteSize = truckOptionsPaths[truckWithMinRoute].Length;

                for (int i = 0; i < validTruckOptions.Count; i++)
                {
                    if (truckOptionsPaths[validTruckOptions[i]].Length < minRouteSize)
                    {
                        truckWithMinRoute = validTruckOptions[i];
                        minRouteSize = truckOptionsPaths[validTruckOptions[i]].Length;
                    }
                }

                validTruckOptions.Remove(truckWithMinRoute);

                // if is not aviable, find the next min route
                while (!GlobalData.trucks[truckWithMinRoute].isAviable)
                {
                    if (validTruckOptions.Count == 0) // No hay trucks disponibles
                    {
                        break;
                    }

                    truckWithMinRoute = validTruckOptions[0];
                    minRouteSize = truckOptionsPaths[truckWithMinRoute].Length;
                    for (int i = 0; i < validTruckOptions.Count; i++)
                    {
                        if (truckOptionsPaths[validTruckOptions[i]].Length < minRouteSize)
                        {
                            truckWithMinRoute = validTruckOptions[i];
                            minRouteSize = truckOptionsPaths[validTruckOptions[i]].Length;
                        }
                    }

                    validTruckOptions.Remove(truckWithMinRoute);
                }


                if (GlobalData.trucks[truckWithMinRoute].isAviable)
                {
                    GlobalData.trucks[truckWithMinRoute].targetHarvester = harvesterId;
                    List<Vector2> path = ConvertStringToVector2List(truckOptionsPaths[truckWithMinRoute]);

                    if (GlobalData.trucks[truckWithMinRoute].isAviable)
                    {
                        GlobalData.trucks[truckWithMinRoute].isAviable = false;
                        GlobalData.trucks[truckWithMinRoute].path = path;
                    }
                    else // TODO: Si no esta disponible, que el harvester pida otra ruta
                    {
                        
                    }
                }
                else // TODO: Si no esta disponible, que el harvester pida otra ruta
                {
                    Debug.Log("No hay ningun harvester disponible");
                }
                
            } else if (message.type == "truck_to_silos")
            {
                // first line = truck id
                // second line = path
                
                string[] lines = message.data.Split('\n');
                int truckId = int.Parse(lines[0]);
                List<Vector2> path = ConvertStringToVector2List(lines[1]);
                
                // probar si funciona
                GlobalData.trucks[truckId].path = path;
                GlobalData.trucks[truckId].isAviable = false;
            }

        };
        
        ws.Connect();
    }

    List<Vector2> ConvertStringToVector2List(string list)
    {
        // example of string: [[0, 0], [0, 1]]
        
        // remove the spaces
        list = list.Replace(" ", "");
        
        // remove the first and last character
        list = list.Substring(1, list.Length - 3);
        
        
        // on this point the string looks like this: [0,0],[0,1]
        string[] segments = list.Replace("[", "").Replace("]", "").Split(',');
        
        List<Vector2> vectorList = new List<Vector2>();
        
        for (int i = 0; i < segments.Length; i += 2)
        {
            Vector2 vector = new Vector2(int.Parse(segments[i]), int.Parse(segments[i + 1]));
            vectorList.Add(vector);
        }
        
        
        return  vectorList;
    }
    
    void PrintListOfInt(List<int> list)
    {
        // Create a string builder to store the json
        StringBuilder jsonBuilder = new StringBuilder("[");
        
        // Add each element to the json
        
for (int i = 0; i < list.Count; i++)
        {
            jsonBuilder.Append(list[i]);
            if (i < list.Count - 1)
            {
                jsonBuilder.Append(",");
            }
        }

        // Close the json
        jsonBuilder.Append("]");
        
        // Print the json
        Debug.Log(jsonBuilder.ToString());
    }
    
    void PrintListOfString(List<string> list)
    {
        // Create a string builder to store the json
        StringBuilder jsonBuilder = new StringBuilder("[");
        
        // Add each element to the json
        
for (int i = 0; i < list.Count; i++)
        {
            jsonBuilder.Append(list[i]);
            if (i < list.Count - 1)
            {
                jsonBuilder.Append(",");
            }
        }

        // Close the json
        
        jsonBuilder.Append("]");

        // Print the json
        Debug.Log(jsonBuilder.ToString());
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

    string ListOfIntToJson(List<int> lst)
    {
        StringBuilder jsonBuilder = new StringBuilder("[");
        
        for (int i = 0; i < lst.Count; i++)
        {
            jsonBuilder.Append(lst[i]);
            
            if (i < lst.Count - 1)
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

    public void SendTruckToSilos(int row, int col, int id)
    {
        string fieldMatrixJson = MatrixToJson(GlobalData.fieldMatrix);
        string finalPosition =  "[" + GlobalData.storageRow.ToString() + "," + GlobalData.storageCol.ToString() + "]";
        string truckInitialPos = "[" + (row+1) + "," + (col+1) + "]";
    
        
        var message = new Msg_SendTruckToSilos
        {
            type = "send-truck-to-silos",
            finalPos = finalPosition,
            fieldMatrix = fieldMatrixJson,
            truckInitialPos = truckInitialPos, 
            truckId = id.ToString()
            
        };
        
        var jsonMessage = JsonUtility.ToJson(message);

        Debug.Log(jsonMessage); 
        
        ws.Send(jsonMessage); 
    }

    public void SendHarvesterUnloadRequest(int finalRow, int finalCol, int id)
    {
        string fieldMatrixJson = MatrixToJson(GlobalData.fieldMatrix);
        string finalPosition =  "[" + finalRow.ToString() + "," + finalCol.ToString() + "]";
        
        // Create the trucks initial positions matrix
        List<Vector2> truckInitialPosMatrix = new List<Vector2>();
        List<int> trucksIds = new List<int>(); 
        int[] initialPos; 
        for (int i = 0; i < GlobalData.numTrucks; i++)
        {
            if (!GlobalData.trucks[i].isAviable)
                continue;
            
            initialPos = Common.FixTruckPositions(GlobalData.trucks[i].currentRow, GlobalData.trucks[i].currentCol);
            truckInitialPosMatrix.Add(new Vector2(initialPos[0], initialPos[1]));
            trucksIds.Add(i); 
        }
        
        string truckInitialPos = ListOfVector2ToJson(truckInitialPosMatrix);
        string trucksIdsJson = ListOfIntToJson(trucksIds);
        
        var message = new Msg_HarvesterUnloadRequest
        {
            type = "harvester_unload_request",
            harvesterId = id.ToString(), 
            fieldMatrix = fieldMatrixJson,
            finalPos = finalPosition,
            trucksInitialPos = truckInitialPos,
            trucksIds = trucksIdsJson
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



