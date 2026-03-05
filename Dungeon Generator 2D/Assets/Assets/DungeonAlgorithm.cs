using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all dungeon generation algorithms.
/// Subclasses implement Generate() and return a set of floor positions.
/// No Unity rendering happens here — pure data only.
/// </summary>
public abstract class DungeonAlgorithm
{
    protected int Width { get; }
    protected int Height { get; }

    protected DungeonAlgorithm(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Run the algorithm and return all walkable floor positions.
    /// </summary>
    public abstract HashSet<Vector2Int> Generate();

    protected bool IsInBounds(Vector2Int pos) =>
        pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;

    protected static Vector2Int GetCenter(RectInt room) =>
        new Vector2Int(room.xMin + room.width / 2, room.yMin + room.height / 2);
}