using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class DungeonGenerator2D : MonoBehaviour
{

    [Header("Map Settings")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;

    [Header("Room Settings")]
    [SerializeField] private int roomCount = 15;
    [SerializeField] private Vector2Int roomSizeRange = new Vector2Int(4, 10);
    [SerializeField] private int maxRoomAttempts = 100;

    [Header("Rendering")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Animated Generation")]
    [SerializeField] private bool animateGeneration = true;
    [SerializeField] private float roomDelay = 0.3f;
    [SerializeField] private float corridorDelay = 0.1f;
    [SerializeField] private float tileDelay = 0.01f;


    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private DungeonAnimator animator;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        SetupCamera();
        Regenerate();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Regenerate();
    }

    private void Regenerate()
    {
        StopAllCoroutines();

        animator = new DungeonAnimator(
            floorTilemap, wallTilemap,
            floorTile, wallTile,
            roomDelay, corridorDelay, tileDelay,
            width, height);

        if (animateGeneration)
            StartCoroutine(GenerateAnimated());
        else
            GenerateInstant();
    }

    private void GenerateInstant()
    {
        var algorithm = new RandomRoomsAlgorithm(width, height, roomCount, roomSizeRange, maxRoomAttempts);
        floorPositions = algorithm.Generate();
        RenderAll();
    }

    private IEnumerator GenerateAnimated()
    {
        if (!ValidateTilemaps()) yield break;

        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorPositions.Clear();

        yield return null;

        var algorithm = new RandomRoomsAlgorithm(width, height, roomCount, roomSizeRange, maxRoomAttempts);
        algorithm.Generate();
        var rooms = new List<RectInt>(algorithm.Rooms);

        yield return StartCoroutine(animator.AnimateGeneration(rooms, floorPositions));
    }


    private void RenderAll()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        foreach (var pos in floorPositions)
            floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);

        foreach (var pos in floorPositions)
            CarveWallsAround(pos);
    }

    private void CarveWallsAround(Vector2Int center)
    {
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                var neighbor = center + new Vector2Int(dx, dy);

                if (!floorPositions.Contains(neighbor) && IsInBounds(neighbor))
                    wallTilemap.SetTile(new Vector3Int(neighbor.x, neighbor.y, 0), wallTile);
            }
    }

 
    private bool IsInBounds(Vector2Int pos) =>
        pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;

    private bool ValidateTilemaps()
    {
        if (floorTilemap != null && wallTilemap != null) return true;
        Debug.LogError("Tilemaps not assigned!");
        return false;
    }

    private void SetupCamera()
    {
        if (mainCamera == null) return;

        mainCamera.orthographic = true;
        mainCamera.transform.position = new Vector3(width / 2f, height / 2f, -10f);

        float aspect = (float)Screen.width / Screen.height;
        float verticalSize = height / 2f + 5f;
        float horizontalSize = (width / 2f + 5f) / aspect;

        mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}