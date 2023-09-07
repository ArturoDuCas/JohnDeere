using System; 


[Serializable]
public class Msg_SendInitialHarvesterData
{
    public string type;
    public string harvesterId; 
    public string startingPoints;
    public string  fieldMatrix;
}