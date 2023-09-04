# -*- coding: utf-8 -*-
"""
Created on Mon Sep  4 17:40:03 2023

@author: oscar
"""

import numpy as np
import heapq
import random

def shortest_path(matrix, start, end):
    rows, cols = len(matrix), len(matrix[0])
    directions = [(0, 1), (1, 0), (0, -1), (-1, 0)]
    visited = [[False] * cols for _ in range(rows)]
    min_distance = [[float('inf')] * cols for _ in range(rows)]
    min_distance[start[0]][start[1]] = 0
    pq = [(0, start)]
    paths = [[None] * cols for _ in range(rows)]

    while pq:
        current_distance, current_vertex = heapq.heappop(pq)
        
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

        for x, y in directions:
            ni, nj = i + x, j + y
            if 0 <= ni < rows and 0 <= nj < cols:
                distance = current_distance + 1
                if distance < min_distance[ni][nj]:
                    min_distance[ni][nj] = distance
                    heapq.heappush(pq, (distance, (ni, nj)))
                    paths[ni][nj] = (i, j)

    return float('inf'), []
def tsp(matrix):
    points = [(i, j) for i in range(len(matrix)) for j in range(len(matrix[0])) if matrix[i][j] == 1]
    num_points = len(points)
    distance_matrix = [[0] * num_points for _ in range(num_points)]
    
    for i in range(num_points):
        for j in range(i+1, num_points):
            distance, _ = shortest_path(matrix, points[i], points[j])
            distance_matrix[i][j] = distance_matrix[j][i] = distance
    
    border_points = [points[i] for i in range(num_points) if 
                     points[i][0] == 0 or 
                     points[i][1] == 0 or 
                     points[i][0] == len(matrix) - 1 or 
                     points[i][1] == len(matrix[0]) - 1]

    # Si aucun point de bord n'est disponible, choisissez un point aléatoire
    if not border_points:
        start_point = random.choice(points)
    else:
        start_point = random.choice(border_points)
    
    start_index = points.index(start_point)
    
    visited = [False] * num_points
    visited[start_index] = True
    current_distance = 0
    order = [start_index]
    
    while len(order) < num_points:
        next_point = None
        min_next_distance = float('inf')
        for j in range(num_points):
            if not visited[j] and distance_matrix[order[-1]][j] < min_next_distance:
                min_next_distance = distance_matrix[order[-1]][j]
                next_point = j
        current_distance += min_next_distance
        visited[next_point] = True
        order.append(next_point)
    
    current_distance += distance_matrix[order[-1]][order[0]]

    min_distance = current_distance  # no need to check again as there's only one route here
    best_order = order
    
    path = [points[best_order[0]]]
    for i in range(1, len(best_order)):
        _, sub_path = shortest_path(matrix, points[best_order[i-1]], points[best_order[i]])
        path += sub_path[1:]
    
    return min_distance, [points[i] for i in best_order], path


def split_matrix_for_objects(matrix, num_objects):
    cols = len(matrix[0])
    segment_size = cols // num_objects
    matrices = []
    for i in range(num_objects):
        start_col = i * segment_size
        end_col = start_col + segment_size if i != num_objects - 1 else cols
        matrices.append([row[start_col:end_col] for row in matrix])
    return matrices

def nearest_1(matrix, point):
    nearest_point = None
    min_distance = float('inf')
    for i in range(len(matrix)):
        for j in range(len(matrix[0])):
            if matrix[i][j] == 1:
                distance, _ = shortest_path(matrix, (i, j), point)
                if distance < min_distance:
                    min_distance = distance
                    nearest_point = (i, j)
    return nearest_point, min_distance

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

num_objects =5
object_matrices = split_matrix_for_objects(matrix, num_objects)

object_paths = []
object_best_orders = []
object_distances = []

for obj_matrix in object_matrices:
    dist, order, path = tsp(obj_matrix)
    object_distances.append(dist)
    object_best_orders.append(order)
    object_paths.append(path)

for i in range(num_objects):
    for j in range(num_objects):
        if i != j and object_distances[j] > 0:
            nearest_point, _ = nearest_1(object_matrices[j], object_paths[i][-1])
            if nearest_point:
                distance, path = shortest_path(matrix, object_paths[i][-1], nearest_point)
                object_paths[i] += path[1:]
                object_distances[i] += distance

for i in range(num_objects):
    print(f"Object {i+1} Path:", object_paths[i])
    print(f"Object {i+1} Total Distance:", object_distances[i])
    print(f"Object {i+1} Best Order:", object_best_orders[i])
