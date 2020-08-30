using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BattleFieldController : MonoBehaviour, IPointerClickHandler
{
    public static BattleFieldController ActiveBattleField;
    public int Width => _width;

    [SerializeField]
    private int _width;
    public int Height => _height;
    [SerializeField]
    private int _height;
    public float GridCellSize => _gridCellSize;
    [SerializeField]
    private float _gridCellSize;
    public Grid Grid => _grid;
    private Grid _grid;
    Dictionary<Vector2Int, UnitController> _unitCoord;
    private Vector3 offset =>
         -new Vector3(_width * GridCellSize / 2f, _height * GridCellSize / 2f);

    [SerializeField]
    BoxCollider2D _gridPhysics;
    [SerializeField]
    SpriteRenderer _tilePrefab;
    [SerializeField]
    Color green;
    [SerializeField]
    Color empty;
    [SerializeField]
    Color red;
    [SerializeField]
    Color clear;
    SpriteRenderer[,] _tiles;
    [SerializeField]
    Transform _tileHolder;
    public UnitController GetUnitControllerFromCoord(Vector2Int coord)
    {
        if (_unitCoord.TryGetValue(coord, out UnitController _unitc))
        {
            return _unitc;
        }
        return null;
    }

    public Vector3 GetCellWorldPos(int x, int y)
    {
        return new Vector3(x, y) * _gridCellSize + offset;
    }
    public Vector2Int GetCellFromWordPos(Vector3 pos)
    {
        var core = ((pos - offset) / _gridCellSize);
        return new Vector2Int(Mathf.FloorToInt(core.x), Mathf.FloorToInt(core.y));
    }
    public Vector2Int GetCellFromCursor()
    {
        return GetCellFromWordPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    public void ClearCurrentRangeTiles()
    {
        for (int i = 0; i < _tiles.GetLength(0); i++)
        {
            for (int j = 0; j < _tiles.GetLength(1); j++)
            {
                _tiles[i, j].color = empty;
            }
        }
    }
    public void AddUnitToBattleField(UnitController unit, int x, int y)
    {
        _unitCoord[new Vector2Int(x, y)] = unit;
        unit.SetPos(x, y);
    }
    public void UpdateUnitPos(UnitController unit, int x, int y)
    {
        _unitCoord[unit.Pos] = null;
        _unitCoord[new Vector2Int(x, y)] = unit;
        unit.SetPos(x, y);
    }
    public void DrawBoxRange(Vector2Int from, Vector2Int to)
    {
        ClearCurrentRangeTiles();

        var min = new Vector2Int(Mathf.Min(from.x, to.x), Mathf.Min(from.y, to.y));
        var max = new Vector2Int(Mathf.Max(from.x, to.x), Mathf.Max(from.y, to.y));
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                DrawRangeTile(x, y);
            }
        }
    }
    public void DrawSphereRange(Vector2Int center, int radius)
    {
        ClearCurrentRangeTiles();
        for (int r = radius; r >= -radius; r--)
        {
            var off = radius - Mathf.Abs(r);
            for (int x = center.x - off; x <= center.x + off; x++)
            {
                DrawRangeTile(x, center.y + r);
            }
        }

    }
    public void DrawRangeTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;
        var tile = _tiles[x, y];
        tile.color = green;
    }
    void Awake()
    {
        ActiveBattleField = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        _grid = new Grid(_width, _height);
        _gridPhysics.size = new Vector2(_width, _height) * GridCellSize;
        _unitCoord = new Dictionary<Vector2Int, UnitController>();
        GenerateTiles();
    }

    void DestroyAllTiles()
    {
        if (_tiles == null) return;
        for (int i = 0; i < _tiles.GetLength(0); i++)
        {
            for (int j = 0; j < _tiles.GetLength(1); j++)
            {
                if (_tiles[i, j] == null) continue;
                Destroy(_tiles[i, j]);
            }
        }
    }
    void GenerateTiles()
    {
        DestroyAllTiles();
        _tiles = new SpriteRenderer[_width, _height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var tile = Instantiate(_tilePrefab, Vector3.zero, Quaternion.identity);
                tile.transform.SetParent(_tileHolder, false);
                tile.transform.localPosition = GetCellWorldPos(i, j) + new Vector3(GridCellSize / 2f, GridCellSize / 2f);
                tile.transform.localScale = Vector3.one * (_gridCellSize - 0.15f);
                _tiles[i, j] = tile;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Gizmos.DrawLine(GetCellWorldPos(i, j), GetCellWorldPos(i, j + 1));
                Gizmos.DrawLine(GetCellWorldPos(i, j), GetCellWorldPos(i + 1, j));
                if (i == _width - 1)
                {
                    Gizmos.DrawLine(GetCellWorldPos(i + 1, j), GetCellWorldPos(i + 1, j + 1));
                }
                if (j == _height - 1)
                {
                    Gizmos.DrawLine(GetCellWorldPos(i, j + 1), GetCellWorldPos(i + 1, j + 1));
                }
            }
        }
        Gizmos.color = Color.green;
        var cell = GetCellFromCursor();
        Gizmos.DrawLine(GetCellWorldPos(cell.x, cell.y), GetCellWorldPos(cell.x, cell.y + 1));
        Gizmos.DrawLine(GetCellWorldPos(cell.x, cell.y), GetCellWorldPos(cell.x + 1, cell.y));
        Gizmos.DrawLine(GetCellWorldPos(cell.x + 1, cell.y), GetCellWorldPos(cell.x + 1, cell.y + 1));
        Gizmos.DrawLine(GetCellWorldPos(cell.x, cell.y + 1), GetCellWorldPos(cell.x + 1, cell.y + 1));
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BattleController.CurrentBattle.PlayerTurn || eventData.button != PointerEventData.InputButton.Left) return;
        var cell = GetCellFromCursor();
        Debug.Log($"Click on battle field { cell }");
        var unitc = GetUnitControllerFromCoord(cell);
        if (BattleController.CurrentBattle.SelectedCard != null)
        {
            if (BattleController.CurrentBattle.SelectedCard is UIUnitCard && unitc == null)
            {
                BattleController.CurrentBattle.SelectedCard.Use(cell.x, cell.y);
            }
            return;
        }

        if (unitc != null)
        {
            if (unitc == BattleController.CurrentBattle.SelectedUnit)
            {
                unitc.Deselect();
            }
            else
            {
                unitc.Select();
            }
        }
        else
        {
            if (BattleController.CurrentBattle.SelectedUnit != null
            && BattleController.CurrentBattle.SelectedUnit.Ready
            && BattleController.CurrentBattle.SelectedUnit.Friendly
            && BattleController.CurrentBattle.SelectedUnit.CanMoveTo(cell.x, cell.y))
            {
                UpdateUnitPos(BattleController.CurrentBattle.SelectedUnit, cell.x, cell.y);
                DrawSphereRange(cell, BattleController.CurrentBattle.SelectedUnit.LoadedUnit.Speed);
                BattleController.CurrentBattle.SelectedUnit.Ready = false;
                BattleController.CurrentBattle.SelectedUnit.Deselect();
            }
        }
    }
}
