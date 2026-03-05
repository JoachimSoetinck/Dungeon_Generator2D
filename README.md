# Unity 2D Dungeon Generation Project

A modular **2D procedural dungeon generation** project built with Unity.  
This project demonstrates multiple dungeon generation algorithms operating on a **grid-based Tilemap system**, allowing easy comparison, experimentation, and extension.

---

## Overview

This Unity project focuses on generating **procedural dungeons** using different algorithms while maintaining a consistent rendering pipeline:

- Grid-based layout  
- FloorTilemap for walkable tiles  
- WallTilemap for obstacles and boundaries  
- Shared rendering logic  
- Swappable generator scripts  

The goal is to explore how various dungeon generation techniques influence:

- Layout structure  
- Room connectivity  
- Corridor patterns  
- Gameplay flow  

---

## Dungeon Generation Algorithms

### Random Room Placement

**Definition**  
Random Room Placement is a procedural dungeon generation technique where rooms of varying sizes are placed at random positions within a defined grid. Overlap checks ensure rooms do not intersect, and corridors are created to connect them.

**Workflow**

1. **Initialize Map**  
   Define dungeon width and height. Prepare data structures to store floor positions and rooms.

2. **Generate Random Rooms**  
   Randomly determine room dimensions within a specified size range.

3. **Select Room Position**  
   Choose a random location inside map bounds, ensuring space for walls/margins.

4. **Check Overlap**  
   Compare the new room (often expanded by a buffer) against existing rooms.  
   - If overlapping → discard  
   - If valid → accept

5. **Store Room Data**  
   Add the room to the room list and mark its tiles as floor positions.

6. **Repeat Placement**  
   Continue until the number of rooms is placed or attempts are exhausted.

7. **Connect Rooms**  
   Create corridors between rooms. Whith this algorithm the center of each room will be used to create corridors. This will lead to L-shape corridors. 

### Binary Space Partitioning (BSP)

### Cellular Automata

### Drunkard’s Walk

### Room Graph Generation

### Minimum Spanning Tree (MST) Corridors

### Voronoi-Based Generation

### Hybrid Generation

---

## Tilemap System

The dungeon is rendered using Unity’s Tilemap system.

| Tilemap | Purpose |
|--------|---------|
| FloorTilemap | Walkable dungeon tiles |
| WallTilemap | Walls and obstacles |

### Rendering Rules
- Floor tiles are placed at generated positions  
- Walls are placed automatically around floor neighbors  
- Map bounds are respected  
---

## DungeonGenerator2D

### Features

- Configurable map size  
- Adjustable room count  
- Random room dimensions  
- Overlap prevention  
- Corridor generation  
- Automatic wall placement  
- Camera auto-framing  
- Animated dungeon generation

---

## Grid Logic

- Dungeon uses integer grid coordinates (`Vector2Int`)  
- Floor positions stored in a `HashSet<Vector2Int>`  
- Prevents duplicate tiles  
- Efficient neighbor lookup for wall detection  

---

