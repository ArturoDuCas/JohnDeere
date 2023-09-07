using UnityEngine;
using System.Collections.Generic;

public static class Common 
{
    public static bool isGoingToCrash(Vector2 nextPos)
    {
        int numTrucks = GlobalData.numTrucks;
        int actualTruck = 0; 
        for(int i = 0; i < GlobalData.numHarvesters; i++)
        {
            // verify collision with harvester
            if(GlobalData.harvesters[i].currentRow == nextPos.x && GlobalData.harvesters[i].currentCol == nextPos.y)
            {
                return true;
            }
            
            // verifiy collision with truck
            if(actualTruck < numTrucks)
            {
                if(GlobalData.trucks[actualTruck].currentRow == nextPos.x && GlobalData.trucks[actualTruck].currentCol == nextPos.y)
                {
                    return true;
                }
                actualTruck++;
            }
        }

        return false; 
    }
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

    public static void DeleteAllHarvesterPaths()
    {
        for(int i = 0; i < GlobalData.harvesters.Length; i++)
        {
            GlobalData.harvesters[i].path = new List<Vector2>(); 
        }
    }
    
    public static void DivideMatrixRecursively(int[,] sourceMatrix, int[,] matrix1, int[,] matrix2, int harvester1Row, int harvester1Col, int harvester2Row, int harvester2Col, int row, int col, int numRows, int numCols)
    {
        if (row >= numRows || col >= numCols || row < 0 || col < 0 || matrix1[row, col] != 0 || matrix2[row, col] != 0)
        {
            // Invalid or already assigned cell
            return;
        }

        if (row == harvester1Row && col == harvester1Col)
        {
            // Assign the cell to harvester 1
            matrix1[row, col] = 1;
        }
        else if (row == harvester2Row && col == harvester2Col)
        {
            // Assign the cell to harvester 2
            matrix2[row, col] = 1;
        }
        else
        {
            // Assign the cell to one of the matrices
            if (matrix1[harvester1Row, harvester1Col] == 0)
            {
                matrix1[row, col] = 1;
            }
            else
            {
                matrix2[row, col] = 1;
            }
        }

        // Recursively divide the matrix in all directions
        DivideMatrixRecursively(sourceMatrix, matrix1, matrix2, harvester1Row, harvester1Col, harvester2Row, harvester2Col, row + 1, col, numRows, numCols);
        DivideMatrixRecursively(sourceMatrix, matrix1, matrix2, harvester1Row, harvester1Col, harvester2Row, harvester2Col, row - 1, col, numRows, numCols);
        DivideMatrixRecursively(sourceMatrix, matrix1, matrix2, harvester1Row, harvester1Col, harvester2Row, harvester2Col, row, col + 1, numRows, numCols);
        DivideMatrixRecursively(sourceMatrix, matrix1, matrix2, harvester1Row, harvester1Col, harvester2Row, harvester2Col, row, col - 1, numRows, numCols);
    }
    
    public static List<int[,]> DivideMatrix(int[,] fieldMatrix, int[,] harvesterStartingPos)
    {
        int numRows = fieldMatrix.GetLength(0);
        int numCols = fieldMatrix.GetLength(1);

        int harvester1Row = harvesterStartingPos[0, 0];
        int harvester1Col = harvesterStartingPos[0, 1];
        int harvester2Row = harvesterStartingPos[1, 0];
        int harvester2Col = harvesterStartingPos[1, 1];

        // Create the result matrices
        int[,] halfMatrix1 = new int[numRows, numCols];
        int[,] halfMatrix2 = new int[numRows, numCols];

        // Recursively divide the matrix
        DivideMatrixRecursively(fieldMatrix, halfMatrix1, halfMatrix2, harvester1Row, harvester1Col, harvester2Row, harvester2Col, 0, 0, numRows, numCols);

        // Create a List<int[,]> to hold both halves
        List<int[,]> resultList = new List<int[,]>();

        resultList.Add(halfMatrix1);
        resultList.Add(halfMatrix2);

        return resultList;

    }

    
}





    
    

    