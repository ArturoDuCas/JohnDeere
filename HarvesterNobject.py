# -*- coding: utf-8 -*-
"""
Created on Mon Sep  3 17:40:03 2023

@author: oscar
"""
import numpy as np
import heapq
import random

def shortest_path(matrix, start, end):
    """
    Find the shortest path between start and end points in a binary matrix using Dijkstra's algorithm.
    
    Parameters:
    - matrix: 2D list containing 0's and 1's.
    - start: Tuple of start point (row, column).
    - end: Tuple of end point (row, column).
    
    Returns:
    - Minimum distance between start and end.
    - Path as list of points (row, column) from start to end.
    """
    
    rows, cols = len(matrix), len(matrix[0])
    
    # Possible movements: right, down, left, up
    directions = [(0, 1), (1, 0), (0, -1), (-1, 0)]
    
    # Create a matrix to mark visited cells
    visited = [[False] * cols for _ in range(rows)]
    
    # Create a matrix to store shortest distances from the start to each cell
    min_distance = [[float('inf')] * cols for _ in range(rows)]
    min_distance[start[0]][start[1]] = 0
    
    # Priority queue to store vertices to be visited next
    pq = [(0, start)]
    
    # Matrix to store the previous cell in the shortest path to each cell
    paths = [[None] * cols for _ in range(rows)]

    while pq:
        # Get the next vertex with the shortest distance from start
        current_distance, current_vertex = heapq.heappop(pq)
        
        # If we've reached the end, reconstruct the path and return
        if current_vertex == end:
            path = []
            i, j = end
            while (i, j) != start:
                path.append((i, j))
                i, j = paths[i][j]
            path.append(start)
            path.reverse()
            return current_distance, path
        
        i, j = current_vertex
        if visited[i][j]:
            continue
        visited[i][j] = True

        # Explore neighbors
        for x, y in directions:
            ni, nj = i + x, j + y
            if 0 <= ni < rows and 0 <= nj < cols:
                distance = current_distance + 1  # Assuming all edges have weight 1
                if distance < min_distance[ni][nj]:
                    min_distance[ni][nj] = distance
                    heapq.heappush(pq, (distance, (ni, nj)))
                    paths[ni][nj] = (i, j)

    return float('inf'), []  # If no path is found

def tsp(matrix):
    """
    Solve the Travelling Salesman Problem for points in the matrix marked by 1's.
    
    Parameters:
    - matrix: 2D list containing 0's and 1's.
    
    Returns:
    - Minimum total distance to visit all points.
    - Order of visited points.
    - Complete path with the shortest route.
    """
    
    # Extract the points (cells with 1's)
    points = [(i, j) for i in range(len(matrix)) for j in range(len(matrix[0])) if matrix[i][j] == 1]
    num_points = len(points)
    
    # Create a matrix to store distances between each pair of points
    distance_matrix = [[0] * num_points for _ in range(num_points)]
    
    # Fill the distance matrix with shortest paths between each pair of points
    for i in range(num_points):
        for j in range(i+1, num_points):
            distance, _ = shortest_path(matrix, points[i], points[j])
            distance_matrix[i][j] = distance_matrix[j][i] = distance
    
    # Preferably choose a border point to start, if available
    border_points = [points[i] for i in range(num_points) if 
                     points[i][0] == 0 or 
                     points[i][1] == 0 or 
                     points[i][0] == len(matrix) - 1 or 
                     points[i][1] == len(matrix[0]) - 1]
                     
    # Choose the start point
    if not border_points:
        start_point = random.choice(points)
    else:
        start_point = random.choice(border_points)
    
    start_index = points.index(start_point)
    
    # Array to keep track of visited points
    visited = [False] * num_points
    visited[start_index] = True
    current_distance = 0
    order = [start_index]
    
    # Visit all points
    while len(order) < num_points:
        next_point = None
        min_next_distance = float('inf')
        
        # Find the nearest unvisited point
        for j in range(num_points):
            if not visited[j] and distance_matrix[order[-1]][j] < min_next_distance:
                min_next_distance = distance_matrix[order[-1]][j]
                next_point = j
        
        # Update distance and mark point as visited
        current_distance += min_next_distance
        visited[next_point] = True
        order.append(next_point)
    
    # Return to the start point to complete the circuit
    current_distance += distance_matrix[order[-1]][order[0]]

    min_distance = current_distance
    best_order = order
    
    # Reconstruct the full path
    path = [points[best_order[0]]]
    for i in range(1, len(best_order)):
        _, sub_path = shortest_path(matrix, points[best_order[i-1]], points[best_order[i]])
        path += sub_path[1:]
    
    return min_distance, [points[i] for i in best_order], path

# Additional utility functions and main program are included as in your provided code.


def split_matrix_for_objects(matrix, num_objects):
    """
    Splits the matrix into multiple sub-matrices, based on the number of objects specified.
    Each sub-matrix represents a segment of the original matrix.
    """
    
    cols = len(matrix[0])  # Total number of columns in the matrix
    segment_size = cols // num_objects  # Size of each segment
    matrices = []  # List to store sub-matrices
    
    # Iterating for each object
    for i in range(num_objects):
        start_col = i * segment_size  # Starting column of segment
        # If it's the last segment, take all the remaining columns
        end_col = start_col + segment_size if i != num_objects - 1 else cols 
        # Append the segment to the matrices list
        matrices.append([row[start_col:end_col] for row in matrix])
        
    return matrices

def nearest_1(matrix, point):
    """
    Finds the nearest cell containing 1 to the given point.
    Returns the nearest point and its distance from the given point.
    """
    
    nearest_point = None  # To store the nearest point
    min_distance = float('inf')  # Initialize min_distance with infinity

    # Iterating through the matrix
    for i in range(len(matrix)):
        for j in range(len(matrix[0])):
            # If the cell contains a 1
            if matrix[i][j] == 1:
                # Compute the distance to the given point
                distance, _ = shortest_path(matrix, (i, j), point)
                # Update nearest_point and min_distance if necessary
                if distance < min_distance:
                    min_distance = distance
                    nearest_point = (i, j)
                    
    return nearest_point, min_distance

# Sample matrix
matrix = [
    [1, 1, 0, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 0, 1],
]


num_objects = 10

# Split the matrix for each object
object_matrices = split_matrix_for_objects(matrix, num_objects)

object_paths = []
object_best_orders = []
object_distances = []

# For each object matrix, calculate the TSP path, order and distance
for obj_matrix in object_matrices:
    dist, order, path = tsp(obj_matrix)
    object_distances.append(dist)
    object_best_orders.append(order)
    object_paths.append(path)

# For each object, calculate the nearest point in every other object's matrix 
# and update the paths and distances accordingly
for i in range(num_objects):
    for j in range(num_objects):
        if i != j and object_distances[j] > 0:
            nearest_point, _ = nearest_1(object_matrices[j], object_paths[i][-1])
            if nearest_point:
                distance, path = shortest_path(matrix, object_paths[i][-1], nearest_point)
                object_paths[i] += path[1:]
                object_distances[i] += distance

# Print the final results for each object
for i in range(num_objects):
    print(f"Object {i+1} Path:", object_paths[i])
    print(f"Object {i+1} Total Distance:", object_distances[i])
    print(f"Object {i+1} Best Order:", object_best_orders[i])
