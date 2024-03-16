using BnavBoard;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace BnavBoard
{
    #region enums
    public enum MapEdge : byte
    {
        Top = 0b1000,
        Right = 0b0100,
        Bottom = 0b0010,
        Left = 0b0001,
        All = 0b1111,
        None = 0b0000,
        Cade = Top | Bottom,
        SeaBattle = None,
    }

    enum TileGroups : byte
    {
        cell,
        safezone,
        wind,
        whirl,
        rocks_big,
        rocks_small,
        buoy,
    }

    enum TileIds : byte
    {
        wind = 5,
        whirl = 9,
        buoy = 21,
        rocks_big = 50,
        rocks_small = 51,
    }
    #endregion

    public class TilemapBuilder : MonoBehaviour
    {
        #region unity editor
        [Tooltip("The layer that draws the water tiles")]
        public Tilemap baseMap;

        [Range(1, 100), Tooltip("The width of the map in tiles")]
        public int mapWidth = 20;

        [Range(1, 100), Tooltip("The height of the map in tiles")]
        public int mapHeight = 36;

        public MapEdge safeZones = MapEdge.Top | MapEdge.Bottom;
        #endregion
        private AssetBundle tileBundle;
        private TileBase[][] tiles;
        private PrefixLogger logger;
        private new Camera camera;
        private int rotationSeed;
        private string mapData =
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAyAAoLAAAAAAAAAAAAAAAAAAAAAAAACQwAAAAAAAAAAAAAAAAACAUAAAAAAAAAAAAAAAAyAAAAAAAABQAAAAAyCgsACAAIAAAAAAAAAAAFCAAAAAAJDAAAAAAAAAgAMwAAAAAAAAAAAAAyMgAAAAAAAAAAAAAyAAAAAAAAMgAAAAAACAgAMgAAAAAAAAAAAAAAAAAAMgAFAAAAMwAVBgYAMwAGBjIAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAAAAAAAAAAAAAAAAAADMVAAAGAAAAAAAAMwYACgsAFQAAMgAAAAAAMgAAAAAAAAAJDAAAMwYGAAAAAAAAAAAAMgAAAAUAAAgAAAAAAAAAAAAICAAAAAAABgAAAAAAADIAAAAAAAAAAAAAAAAAADIAAAAAAAAAADMACgsyAAAAAAYAADIABgAAAAAAAAAJDAAAAAAAAAAAAAAAAAAAAAAyAAUAAAAzABUAADIAFQAKCwAAMgAKCwAyBgAIAAgAAAAAAAkMAAAAAAkMAAAKCwYAAAAAAAAAMgoLAAoLCAAAAAkMAAAAAAAAAAAACQwACQwAAAAAAAAAAAAAAAoLCgsAAAAAAAAAAAAAAAAAAAAACQwJDAAAMgAAAAAAAAAAAAAACBcAAAAAAAAAAAgAAAAXAAAAAAAAAAAAAAAAAAgAMgAAAAAAAAAAAAAAAAAGAAAAAAAAAAAAAAAAAAAAMgAAAAAAAAAAMwAACgsAAAAAADMAAAAyAAAAAAAAAAgJDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

        private void OnEnable()
        {
            camera = Camera.main != null ? Camera.main : camera;
            logger = new PrefixLogger(name);
            tileBundle = AssetBundle.LoadFromFile("AssetBundles/BnavBoard");
            tiles = ParseTiles(tileBundle);

            rotationSeed = Random.Range(0, int.MaxValue - 1);
            BuildMap();
            SetMapBase64(mapData);
        }

        private void OnDisable()
        {
            if (tileBundle != null)
            {
                tileBundle.Unload(false);
            }
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
                return;

            var worldPoint = camera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                var tilemap = hit.collider.GetComponent<Tilemap>();
                var cellPosition = tilemap.WorldToCell(hit.point);
                cellPosition.y = mapHeight - 1 - cellPosition.y;
                logger.Log("Clicked tile: " + cellPosition);
            }
        }

        private void SetMapBase64(string base64)
        {
            mapData = base64;
            BuildMap();
        }

        public void BuildMap()
        {
            DrawBaseLayer();
            DrawObstacles();
        }

        private static TileBase[][] ParseTiles(AssetBundle tileBundle)
        {
            var tileSet = tileBundle.LoadAllAssets<TileBase>();
            var tiles = new TileBase[System.Enum.GetValues(typeof(TileGroups)).Length][];
            tiles[(int)TileGroups.cell] = new TileBase[5];
            tiles[(int)TileGroups.safezone] = new TileBase[5];
            tiles[(int)TileGroups.wind] = new TileBase[4];
            tiles[(int)TileGroups.whirl] = new TileBase[8];
            tiles[(int)TileGroups.rocks_big] = new TileBase[4];
            tiles[(int)TileGroups.rocks_small] = new TileBase[4];
            tiles[(int)TileGroups.buoy] = new TileBase[24];

            for (int i = 0; i < tileSet.Length; i++)
            {
                var tile = tileSet[i];
                var lastUnderscore = tile.name.LastIndexOf('_');
                var groupName = tile.name[..lastUnderscore];
                var tileId = tile.name[(lastUnderscore + 1)..];
                var group = (TileGroups)System.Enum.Parse(typeof(TileGroups), groupName);
                tiles[(byte)group][int.Parse(tileId)] = tile;
            }
            return tiles;
        }

        private void DrawSafezones()
        {
            if (safeZones == MapEdge.None)
                return;

            var safeZoneTiles = new TileBase[Mathf.Max(mapHeight, mapWidth) * 3];
            for (int x = 0; x < safeZoneTiles.Length; x++)
            {
                var rand = rotationSeed / (x + 1) % 5;

                safeZoneTiles[x] = tiles[(byte)TileGroups.safezone][rand];
            }

            if ((safeZones & MapEdge.Top) == MapEdge.Top)
            {
                baseMap.SetTilesBlock(
                    new BoundsInt(0, mapHeight - 3, 0, mapWidth, 3, 1),
                    safeZoneTiles
                );
            }

            if ((safeZones & MapEdge.Bottom) == MapEdge.Bottom)
            {
                baseMap.SetTilesBlock(new BoundsInt(0, 0, 0, mapWidth, 3, 1), safeZoneTiles);
            }

            if ((safeZones & MapEdge.Left) == MapEdge.Left)
            {
                baseMap.SetTilesBlock(new BoundsInt(0, 0, 0, 3, mapHeight, 1), safeZoneTiles);
            }

            if ((safeZones & MapEdge.Right) == MapEdge.Right)
            {
                baseMap.SetTilesBlock(
                    new BoundsInt(mapWidth - 3, 0, 0, 3, mapHeight, 1),
                    safeZoneTiles
                );
            }
        }

        private void DrawBaseLayer()
        {
            baseMap.ClearAllTiles();
            var tilesArray = new TileBase[mapWidth * mapHeight];
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    var rand = rotationSeed / (x + 1) / (y + 1) % 5;
                    tilesArray[y * mapWidth + x] = tiles[(byte)TileGroups.cell][rand];
                }
            }

            var bounds = new BoundsInt(0, 0, 0, mapWidth, mapHeight, 1);
            baseMap.SetTilesBlock(bounds, tilesArray);
            DrawSafezones();

            var cellCenter = new Vector3Int(mapWidth / 2, mapHeight / 2, 0);
            var worldCenter = baseMap.GetCellCenterWorld(cellCenter);
            transform.position -= worldCenter;
        }

        private void DrawObstacles()
        {
            var bytes = System.Convert.FromBase64String(mapData);
            for (int i = 0; i < bytes.Length; i++)
            {
                var tileId = bytes[i];
                if (tileId == 0)
                    continue;

                var x = i % mapWidth;
                var y = mapHeight - 1 - i / mapWidth;
                if (tileId == (byte)TileIds.rocks_big)
                {
                    // var rand = rotationSeed / (i + 1) % 4;
                    // tile = tiles[(byte)TileGroups.rocks_big][rand];
                }
                else if (tileId == (byte)TileIds.rocks_small)
                {
                    // var rand = rotationSeed / (i + 1) % 4;
                    // tile = tiles[(byte)TileGroups.rocks_small][rand];
                }
                else if (tileId >= (byte)TileIds.buoy)
                {
                    // tile = tiles[(byte)TileGroups.buoy][Random.Range(0, 24)];
                }
                else if (tileId >= (byte)TileIds.whirl)
                {
                    var tile = tiles[(byte)TileGroups.whirl][tileId - (byte)TileIds.whirl];
                    baseMap.SetTile(new Vector3Int(x, y, 0), tile);
                }
                else if (tileId >= (byte)TileIds.wind)
                {
                    var tile = tiles[(byte)TileGroups.wind][tileId - (byte)TileIds.wind];
                    baseMap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(TilemapBuilder))]
public class MapBuilderEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TilemapBuilder myScript = (TilemapBuilder)target;
        if (GUILayout.Button("Build Map"))
        {
            myScript.BuildMap();
        }
    }
}
#endif
