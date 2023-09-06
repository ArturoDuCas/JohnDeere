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


    public static int[] FixTruckPositions(int row, int col)
    {
        if (col == -1) // if starting on left side
        {
            col += 1; 
        } else if (row == -1) // if starting on the bottom
        {
            row += 1; 
        } else if (col == GlobalData.fieldRows) // if starting on the right side
        {
            col -= 1; 
        } else if (row == GlobalData.fieldCols) // if starting on the top
        {
            row -= 1; 
        } 
        return new int[] {row, col};
    }

    public static int[] FixHarvesterPositions(int row, int col)
    {
        if (col == -1) // if starting on left side
        {
            col += 1; 
        } else if (row == -1) // if starting on the bottom
        {
            row += 1; 
        } else if (col == GlobalData.fieldRows) // if starting on the right side
        {
            col -= 1; 
        } else if (row == GlobalData.fieldCols) // if starting on the top
        {
            row -= 1; 
        }
        
        return new int[] {row, col};
    }

    public static int GetNumberOfNotHarvestedUnits()
    {
        int res = 0; 
        for(int i = 0; i < GlobalData.fieldRows; i++)
        {
            for(int j = 0; j < GlobalData.fieldCols; j++)
            {
                if (GlobalData.fieldMatrix[i, j] == 1)
                {
                    res += 1;
                }
            }
        }


        return res; 
    }
    
}
