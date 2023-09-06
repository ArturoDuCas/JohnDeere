using System; 


[Serializable]
public class Msg_HarvesterUnloadRequest
{
    public string type;
    public string harvesterId; 
    public string finalPos;
    public string  fieldMatrix;
    public string trucksInitialPos;
    public string trucksIds; 
}