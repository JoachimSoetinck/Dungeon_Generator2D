using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonAnimator
{
    private readonly Tilemap floorTilemap;
    private readonly Tilemap wallTilemap;
    private readonly TileBase floorTile;
    private readonly TileBase wallTile;
    private readonly float roomDelay;
    private readonly float corridorDelay;
    private readonly float tileDelay;
    private readonly int width;
    private readonly int height;

    public DungeonAnimator(
        Tilemap floorTilemap,
        Tilemap wallTilemap,
        TileBase floorTile,
        TileBase wallTile,
        float roomDelay,
        float corridorDelay,
        float tileDelay,
        int width,
        int height)
    {
        this.floorTilemap = floorTilemap;
        this.wallTilemap = wallTilemap;
        this.floorTile = floorTile;
        this.wallTile = wallTile;
        this.roomDelay = roomDelay;
        this.corridorDelay = corridorDelay;
        this.tileDelay = tileDelay;
        this.width = width;
        this.height = height;
    }

    public IEnumerator AnimateGeneration(
        List<RectInt> rooms,
        HashSet<Vector2Int> floorPositions)
    {
        yield return AnimateRooms(rooms, floorPositions);

        if (floorPositions.Count == 0)
        {
            Debug.LogWarning("No rooms were created.");
            yield break;
        }

        yield return AnimateCorridors(rooms, floorPositions);

        yield return null;
        wallTilemap.ClearAllTiles();

        yield return AnimateWalls(floorPositions);
    }

    private IEnumerator AnimateRooms(List<RectInt> rooms, HashSet<Vector2Int> floorPositions)
    {
        foreach (var room in rooms)
        {
            yield return AnimateRect(room, floorPositions);
            yield return WaitIf(roomDelay);
        }
    }

    private IEnumerator AnimateCorridors(List<RectInt> rooms, HashSet<Vector2Int> floorPositions)
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            yield return AnimateCorridor(rooms[i], rooms[i + 1], floorPositions);
            yield return WaitIf(corridorDelay);
        }
    }

    private IEnumerator AnimateWalls(HashSet<Vector2Int> floorPositions)
    {
        var snapshot = new List<Vector2Int>(floorPositions);

        foreach (var pos in snapshot)
        {
            CarveWallsAround(pos, floorPositions);

            if (tileDelay > 0)
                yield return new WaitForSeconds(tileDelay * 0.5f);
        }
    }

    private IEnumerator AnimateCorridor(RectInt a, RectInt b, HashSet<Vector2Int> floorPositions)
    {
        Vector2Int from = GetCenter(a);
        Vector2Int to = GetCenter(b);

        if (Random.value < 0.5f)
        {
            yield return AnimateHLine(from.x, to.x, from.y, floorPositions);
            yield return AnimateVLine(from.y, to.y, to.x, floorPositions);
        }
        else
        {
            yield return AnimateVLine(from.y, to.y, from.x, floorPositions);
            yield return AnimateHLine(from.x, to.x, to.y, floorPositions);
        }
    }

    private IEnumerator AnimateRect(RectInt room, HashSet<Vector2Int> floorPositions)
    {
        for (int x = room.xMin; x < room.xMax; x++)
            for (int y = room.yMin; y < room.yMax; y++)
                yield return AnimateFloorTile(new Vector2Int(x, y), floorPositions);
    }

    private IEnumerator AnimateHLine(int xStart, int xEnd, int y, HashSet<Vector2Int> floorPositions)
    {
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            yield return AnimateFloorTile(new Vector2Int(x, y), floorPositions);
    }

    private IEnumerator AnimateVLine(int yStart, int yEnd, int x, HashSet<Vector2Int> floorPositions)
    {
        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            yield return AnimateFloorTile(new Vector2Int(x, y), floorPositions);
    }

    private IEnumerator AnimateFloorTile(Vector2Int pos, HashSet<Vector2Int> floorPositions)
    {
        floorPositions.Add(pos);
        floorTilemap.SetTile(ToV3(pos), floorTile);
        wallTilemap.SetTile(ToV3(pos), null);

        if (tileDelay > 0)
            yield return new WaitForSeconds(tileDelay);
    }

    private void CarveWallsAround(Vector2Int center, HashSet<Vector2Int> floorPositions)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                var neighbor = center + new Vector2Int(dx, dy);

                if (!floorPositions.Contains(neighbor) && IsInBounds(neighbor))
                    wallTilemap.SetTile(ToV3(neighbor), wallTile);
            }
    }

    private bool IsInBounds(Vector2Int pos) =>
        pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;

    private static Vector2Int GetCenter(RectInt room) =>
        new Vector2Int(room.xMin + room.width / 2, room.yMin + room.height / 2);

    private static Vector3Int ToV3(Vector2Int pos) =>
        new Vector3Int(pos.x, pos.y, 0);

    private static YieldInstruction WaitIf(float seconds) =>
        seconds > 0 ? new WaitForSeconds(seconds) : null;
}