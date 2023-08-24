import random

class QLearning:
    def __init__(self):
        """
        Constructor: Initializes the environment states and Q-learning param
        """
        # Define initial positions for truck, harvester, and storage.
        self.truck_position = 0
        self.harvester_position = 10
        self.storage_position = 20

        # Set the truck's capacity and current load, and the harvester's storage.
        self.truck_capacity = 10
        self.truck_load = 0
        self.harvester_storage = 5

        # Compute the initial state from current positions and loads.
        self.currentState = self.get_state()

        # Initialize Q-Table and Q-learning parameters.
        self.QTable = {}
        self.learningRate = 0.1
        self.discountFactor = 0.9
        self.explorationRate = 0.1

    def get_state(self):
        """
        Calculates the state based on truck s position harvester's position, and their loads
        """
        # Calculate distances from truck to harvester and storage.
        distance_to_harvester = abs(self.truck_position - self.harvester_position)
        distance_to_storage = abs(self.truck_position - self.storage_position)

        # Return the state as a string.
        return f"{distance_to_harvester},{distance_to_storage},{self.truck_load},{self.harvester_storage}"

    def perform_action(self, action):
        """
        Performs the given action and returns a reward
        """
        reward = 0

        if action == "MoveToHarvester":
            self.truck_position += 1 if self.truck_position < self.harvester_position else -1
            if self.truck_position == self.harvester_position:
                collected_maize = min(self.truck_capacity - self.truck_load, self.harvester_storage)
                self.truck_load += collected_maize
                self.harvester_storage -= collected_maize
                reward = collected_maize
            else:
                reward = -1

        elif action == "MoveToStorage":
            self.truck_position += 1 if self.truck_position < self.storage_position else -1
            if self.truck_position == self.storage_position:
                reward = self.truck_load * 1.5
                self.truck_load = 0
            else:
                reward = -1

        elif action == "Wait":
            self.harvester_storage += random.randint(0, 3)
            reward = -0.5

        return reward

    def get_random_action(self):
        """
        Returns a random action.
        """
        actions = ["MoveToHarvester", "MoveToStorage", "Wait"]
        return random.choice(actions)

    def get_best_action(self, state):
        """
        Returns the best action based on current Q-values for the given state
        """
        if state not in self.QTable:
            self.initialize_state(state)
            return self.get_random_action()

        maxQ = float("-inf")
        best_action = ""
        for action, value in self.QTable[state].items():
            if value > maxQ:
                maxQ = value
                best_action = action

        return best_action if best_action else self.get_random_action()

    def get_max_q_value(self, state):
        """
        Returns the maximum Q-value for the given state
        """
        if state not in self.QTable:
            self.initialize_state(state)
            return 0

        return max(self.QTable[state].values())

    def initialize_state(self, state):
        """
        Initializes Q-values for a new state
        """
        actions = ["MoveToHarvester", "MoveToStorage", "Wait"]
        self.QTable[state] = {action: 0 for action in actions}

    def update(self):
        """
        Updates the Q-values based on the agent's actions and the environment's feedback
        """
        action = self.get_random_action() if random.random() < self.explorationRate else self.get_best_action(self.currentState)
        
        reward = self.perform_action(action)
        nextState = self.get_state()

        if self.currentState not in self.QTable:
            self.initialize_state(self.currentState)
        if nextState not in self.QTable:
            self.initialize_state(nextState)

        currentQ = self.QTable[self.currentState].get(action, 0)
        maxNextQ = self.get_max_q_value(nextState)
        newQValue = currentQ + self.learningRate * (reward + self.discountFactor * maxNextQ - currentQ)
        self.QTable[self.currentState][action] = newQValue

        self.currentState = nextState

# Simulating Q-learning over 1000 steps.
ql = QLearning()
for _ in range(1000):
    ql.update()

# Displaying the Q-table after the simulation.
print(ql.QTable)
"""
result :
'10,20,0,5':

Es una representación del estado actual del entorno:
10: distancia entre el camión y la cosechadora.
20: distancia entre el camión y el almacenamiento.
0: carga actual del camión.
'MoveToHarvester': 0: Mover el camión hacia la cosechadora. El valor Q para esta acción es 0, lo que indica que el agente aún no ha aprendido si es una buena o mala acción en este estado.
'MoveToStorage': 0: Mover el camión hacia el almacenamiento. El valor Q para esta acción también es 0, indicando lo mismo.
'Wait': -0.05: Hacer que el camión espere. El valor Q para esta acción es -0.05, lo que significa que, basándose en las experiencias del agente hasta ahora, esperar en este estado tiene una pequeña penalización (es ligeramente negativo).
5: almacenamiento actual de la cosechadora.
"""