using UnityEngine;
using System.Collections.Generic;
public static class GlobalData
{
    // Field data
    public static int fieldCols = 5 ; //Se asigna en WS_Client
    public static int fieldRows = 5; //Se asigna en WS_Client
    public static int[,] fieldMatrix; // 0 = harvested, 1 = not-harvested, 2 = harvesting

    

    // Storage Data
    public static int storageRow; // This rows and col are relative, it doesnt contain the real indexs
    public static int storageCol;
    
    // Unit data
    public static int unit_xSize = 6;
    public static int unit_zSize = 6;
    public static int grainsPerUnit = 5; 
    
    // Connection data
    public static string selfID = "";    

    
    // Harvesters data
    public static int numHarvesters = 2;
    public static int[,] harvestersStartingPoints;

    public static Harvester[] harvesters;
    
    // Truck data
    public static int numTrucks = 2; //Se asigna en WS_Client
    public static Truck[] trucks;
    
    
    // Density related
    public static int cornDensity = 100; // 0 - 100% 
    public static bool isFirstDensityChange = true; 
}