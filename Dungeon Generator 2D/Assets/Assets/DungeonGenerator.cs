using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator2D : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;

    [Header("Room Settings")]
    [SerializeField] private int roomCount = 15;
    [SerializeField] private Vector2Int roomSize = new Vector2Int(4, 10);

    [Header("Rendering")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private List<RectInt> rooms = new List<RectInt>();

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        GenerateDungeon();
        SetupCamera();
    }

    void GenerateDungeon()
    {
        CreateRooms();
        ConnectAllRooms();
        RenderDungeon();
    }

    void CreateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            RectInt room = GenerateRandomRoom();

            if (!HasOverlap(room))
            {
                AddRoomToMap(room);
                rooms.Add(room);
            }
        }
    }

    RectInt GenerateRandomRoom()
    {
        int w = Random.Range(roomSize.x, roomSize.y + 1);
        int h = Random.Range(roomSize.x, roomSize.y + 1);
        int x = Random.Range(1, width - w - 1);
        int y = Random.Range(1, height - h - 1);

        return new RectInt(x, y, w, h);
    }

    bool HasOverlap(RectInt room)
    {
        RectInt expandedRoom = new RectInt(
            room.x - 1,
            room.y - 1,
            room.width + 2,
            room.height + 2
        );

        foreach (var existingRoom in rooms)
        {
            if (expandedRoom.Overlaps(existingRoom))
                return true;
        }
        return false;
    }

    void AddRoomToMap(RectInt room)
    {
        for (int x = room.xMin; x < room.xMax; x++)
        {
            for (int y = room.yMin; y < room.yMax; y++)
            {
                floorPositions.Add(new Vector2Int(x, y));
            }
        }
    }

    void ConnectAllRooms()
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            CreateCorridor(rooms[i], rooms[i + 1]);
        }
    }

    void CreateCorridor(RectInt roomA, RectInt roomB)
    {
        Vector2Int pointA = GetCenter(roomA);
        Vector2Int pointB = GetCenter(roomB);

        if (Random.value < 0.5f)
        {
            CreateHorizontalLine(pointA.x, pointB.x, pointA.y);
            CreateVerticalLine(pointA.y, pointB.y, pointB.x);
        }
        else
        {
            CreateVerticalLine(pointA.y, pointB.y, pointA.x);
            CreateHorizontalLine(pointA.x, pointB.x, pointB.y);
        }
    }

    Vector2Int GetCenter(RectInt room)
    {
        return new Vector2Int(
            room.xMin + room.width / 2,
            room.yMin + room.height / 2
        );
    }

    void CreateHorizontalLine(int xStart, int xEnd, int y)
    {
        int min = Mathf.Min(xStart, xEnd);
        int max = Mathf.Max(xStart, xEnd);

        for (int x = min; x <= max; x++)
        {
            floorPositions.Add(new Vector2Int(x, y));
        }
    }

    void CreateVerticalLine(int yStart, int yEnd, int x)
    {
        int min = Mathf.Min(yStart, yEnd);
        int max = Mathf.Max(yStart, yEnd);

        for (int y = min; y <= max; y++)
        {
            floorPositions.Add(new Vector2Int(x, y));
        }
    }

    void RenderDungeon()
    {
        // Clear beide tilemaps
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // Teken floors op floor tilemap
        foreach (var pos in floorPositions)
        {
            floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);
        }

        // Teken walls op wall tilemap
        foreach (var floorPos in floorPositions)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int neighborPos = floorPos + new Vector2Int(x, y);

                    if (!floorPositions.Contains(neighborPos) &&
                        IsInBounds(neighborPos))
                    {
                        wallTilemap.SetTile(
                            new Vector3Int(neighborPos.x, neighborPos.y, 0),
                            wallTile
                        );
                    }
                }
            }
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    void SetupCamera()
    {
        if (mainCamera == null) return;

        // Centreer camera op dungeon
        float centerX = width / 2f;
        float centerY = height / 2f;

        mainCamera.transform.position = new Vector3(centerX, centerY, -10f);

        // Bereken orthographic size om hele dungeon te laten zien
        float aspectRatio = (float)Screen.width / Screen.height;
        float verticalSize = height / 2f + 5f;
        float horizontalSize = (width / 2f + 5f) / aspectRatio;

        mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);

        // Zorg dat camera orthographic is
        mainCamera.orthographic = true;
    }
}