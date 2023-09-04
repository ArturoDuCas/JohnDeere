using UnityEngine;
using System.Collections.Generic;
public static class GlobalData
{
    // Field data
    public static int fieldCols = 3;
    public static int fieldRows = 3;
    public static int[,] fieldMatrix; // 0 = harvested, 1 = not-harvested, 2 = harvesting
    
    
    // Unit data
    public static int unit_xSize = 6;
    public static int unit_zSize = 6;
    public static int grainsPerUnit = 5; 
    
    // Connection data
    public static string selfID = "";    

    
    // Harvesters data 
    public static Harvester[] harvesters;


}