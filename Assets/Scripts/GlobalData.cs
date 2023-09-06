using UnityEngine;
using System.Collections.Generic;
public static class GlobalData
{
    // Field data
    public static int fieldCols = 6 ; //Se asigna en WS_Client
    public static int fieldRows = 6; //Se asigna en WS_Client
    public static int[,] fieldMatrix; // 0 = harvested, 1 = not-harvested, 2 = harvesting
    
    
    // Unit data
    public static int unit_xSize = 6;
    public static int unit_zSize = 6;
    public static int grainsPerUnit = 5; 
    
    // Connection data
    public static string selfID = "";    

    
    // Harvesters data 
    public static int numHarvesters = 4;
    public static int[,] harvestersStartingPoints;

    public static Harvester[] harvesters;
    
    // Truck data
    public static int numTrucks = 4; //Se asigna en WS_Client
    public static Truck[] trucks;
}