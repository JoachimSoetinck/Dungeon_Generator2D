using System.Collections.Generic;
using UnityEngine;

public class RandomRoomsAlgorithm : DungeonAlgorithm
{
    private readonly int roomCount;
    private readonly Vector2Int roomSizeRange;
    private readonly int maxAttempts;

    public IReadOnlyList<RectInt> Rooms => rooms;
    private readonly List<RectInt> rooms = new List<RectInt>();

    public RandomRoomsAlgorithm(
        int width,
        int height,
        int roomCount,
        Vector2Int roomSizeRange,
        int maxAttempts = 100)
        : base(width, height)
    {
        this.roomCount = roomCount;
        this.roomSizeRange = roomSizeRange;
        this.maxAttempts = maxAttempts;
    }

    public override HashSet<Vector2Int> Generate()
    {
        rooms.Clear();
        var floor = new HashSet<Vector2Int>();
        PlaceRooms(floor);
        ConnectRooms(floor);
        return floor;
    }

    private void PlaceRooms(HashSet<Vector2Int> floor)
    {
        int attempts = 0;
        while (rooms.Count < roomCount && attempts < maxAttempts)
        {
            attempts++;
            RectInt room = CreateRandomRoom();
            if (!HasOverlap(room))
            {
                rooms.Add(room);
                CarveRect(floor, room);
            }
        }
        Debug.Log($"[RandomRooms] Placed {rooms.Count} rooms in {attempts} attempts.");
    }

    private RectInt CreateRandomRoom()
    {
        int w = Random.Range(roomSizeRange.x, roomSizeRange.y + 1);
        int h = Random.Range(roomSizeRange.x, roomSizeRange.y + 1);
        int x = Random.Range(1, Width - w - 1);
        int y = Random.Range(1, Height - h - 1);
        return new RectInt(x, y, w, h);
    }

    private bool HasOverlap(RectInt candidate)
    {
        var padded = new RectInt(
            candidate.x - 1,
            candidate.y - 1,
            candidate.width + 2,
            candidate.height + 2);

        foreach (var existing in rooms)
            if (padded.Overlaps(existing)) return true;

        return false;
    }

    private void ConnectRooms(HashSet<Vector2Int> floor)
    {
        for (int i = 0; i < rooms.Count - 1; i++)
            CarveCorridor(floor, rooms[i], rooms[i + 1]);
    }

    private void CarveCorridor(HashSet<Vector2Int> floor, RectInt a, RectInt b)
    {
        Vector2Int from = GetCenter(a);
        Vector2Int to = GetCenter(b);

        if (Random.value < 0.5f)
        {
            CarveHLine(floor, from.x, to.x, from.y);
            CarveVLine(floor, from.y, to.y, to.x);
        }
        else
        {
            CarveVLine(floor, from.y, to.y, from.x);
            CarveHLine(floor, from.x, to.x, to.y);
        }
    }

    // -------------------------------------------------------------------------
    // Carve Helpers
    // -------------------------------------------------------------------------

    private static void CarveRect(HashSet<Vector2Int> floor, RectInt rect)
    {
        for (int x = rect.xMin; x < rect.xMax; x++)
            for (int y = rect.yMin; y < rect.yMax; y++)
                floor.Add(new Vector2Int(x, y));
    }

    private static void CarveHLine(HashSet<Vector2Int> floor, int xStart, int xEnd, int y)
    {
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            floor.Add(new Vector2Int(x, y));
    }

    private static void CarveVLine(HashSet<Vector2Int> floor, int yStart, int yEnd, int x)
    {
        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            floor.Add(new Vector2Int(x, y));
    }
}