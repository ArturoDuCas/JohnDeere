using UnityEngine;

public static class Common 
{
    public static void printMatrix(int[,] matrix)
    {
        string matrixString = "";
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            matrixString += "[";
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                matrixString += matrix[row, col] + ", ";
            }
            matrixString += "]\n";
        }
        Debug.Log(matrixString);
    }
    
    public static void printArray(Vector2[] array)
    {
        string arrayString = "";
        for (int i = 0; i < array.Length; i++)
        {
            arrayString += array[i] + ", ";
        }
        Debug.Log(arrayString);
    }
    
    public static void printInt2DArray(int[,] arr)
    {
        string arrayString = "";
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            arrayString += "[";
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                arrayString += arr[i, j] + ", ";
            }
            arrayString += "]\n";
        }
        Debug.Log(arrayString);
    }
    
}
