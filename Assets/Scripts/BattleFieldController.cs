using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
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
    public Vector3 HalfGridOffset => new Vector3(GridCellSize, GridCellSize) / 2f;
    [SerializeField]
    private float _gridCellSize;
    public Grid Grid => _grid;
    private Grid _grid;
    Dictionary<Vector2Int, UnitController> _unitCoord = new Dictionary<Vector2Int, UnitController>();
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
    bool[,] _walkable;
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
    public Vector3 GetCellWorldPos(Vector2Int v)
    {
        return GetCellWorldPos(v.x, v.y);
    }
    public Vector3 GetCellWorldPos(int x, int y)
    {
        return new Vector3(x, y) * GridCellSize + offset;
    }
    public Vector2Int GetCellFromWordPos(Vector3 pos)
    {
        var core = ((pos - offset) / GridCellSize);
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
                _tiles[i, j].color = _walkable[i, j] ? empty : clear;
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
        _unitCoord.Remove(unit.Pos);
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
                if (BattleController.CurrentBattle.SelectedUnit != null &&
                BattleController.CurrentBattle.SelectedUnit.CanMoveTo(x, center.y + r, out List<Vector2Int> path))
                    DrawRangeTile(x, center.y + r);
            }
        }
    }
    public bool ValidPos(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
    public void DrawRangeTile(int x, int y)
    {
        if (!ValidPos(x, y) || !_walkable[x, y]) return;
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
        _walkable = new bool[_width, _height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                _walkable[i, j] = true;
            }
        }
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
                tile.transform.localPosition = GetCellWorldPos(i, j) + HalfGridOffset;
                tile.transform.localScale = Vector3.one * (GridCellSize - 0.15f);
                _tiles[i, j] = tile;
                if (!_walkable[i, j])
                    tile.color = clear;
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

        if (!BattleController.CurrentBattle.PlayerTurn) return;
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BattleController.CurrentBattle.SelectedUnit?.Deselect();
            BattleController.CurrentBattle.SelectedCard?.Deselect();
        }
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            var destroyCell = GetCellFromCursor();
            if (ValidPos(destroyCell.x, destroyCell.y))
            {
                _walkable[destroyCell.x, destroyCell.y] = !_walkable[destroyCell.x, destroyCell.y];
                GenerateTiles();
            }
        }
        if (eventData.button != PointerEventData.InputButton.Left) return;
        var cell = GetCellFromCursor();
        if (!ValidPos(cell.x, cell.y))
        {
            {
                BattleController.CurrentBattle.SelectedUnit?.Deselect();
                BattleController.CurrentBattle.SelectedCard?.Deselect();
            }
        }
        Debug.Log($"Click on battle field { cell }");
        if (!_walkable[cell.x, cell.y]) return;
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
            if (BattleController.CurrentBattle.SelectedUnit == null)
            {
                if (unitc.Friendly)
                {
                    unitc.Select();
                }
            }
            else
            {
                if (unitc == BattleController.CurrentBattle.SelectedUnit)
                {
                    if (unitc.InAction)
                    {
                        unitc.CompleteAction();
                    }
                }
                else
                {
                    if (BattleController.CurrentBattle.SelectedUnit.CanAttack(unitc))
                    {
                        BattleController.CurrentBattle.SelectedUnit.Attack(unitc);
                        BattleController.CurrentBattle.SelectedUnit.CompleteAction();
                    }
                }
            }
        }
        else if (BattleController.CurrentBattle.SelectedUnit != null && BattleController.CurrentBattle.SelectedUnit.InAction)
        {
            if (BattleController.CurrentBattle.SelectedUnit.CanMoveTo(cell.x, cell.y, out List<Vector2Int> path))
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(GetCellWorldPos(path[i]) + HalfGridOffset, GetCellWorldPos(path[i + 1]) + HalfGridOffset, Color.blue, 5f);
                }
            }

            // if (BattleController.CurrentBattle.SelectedUnit.CanMoveTo(cell.x, cell.y))
            // {
            //     UpdateUnitPos(BattleController.CurrentBattle.SelectedUnit, cell.x, cell.y);
            // }
        }
    }
    int GetHeuristic(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }
    void ConstructPath(Vector2Int curr, ref Stack<Vector2Int> record, Dictionary<Vector2Int, Vector2Int> map)
    {
        if (map.TryGetValue(curr, out Vector2Int result) && result != -Vector2Int.one)
        {
            record.Push(result);
            ConstructPath(result, ref record, map);
        }
    }
    List<Vector2Int> GetUnblockedNeighbours(Vector2Int cell)
    {
        return GetWalkableNeighbours(cell).Where(_ => !_unitCoord.ContainsKey(cell) || _unitCoord[cell].Friendly).ToList();
    }
    List<Vector2Int> GetWalkableNeighbours(Vector2Int cell)
    {
        return GetNeighbours(cell).Where(_ => _walkable[_.x, _.y]).ToList();
    }
    List<Vector2Int> GetNeighbours(Vector2Int cell)
    {
        return new List<Vector2Int>(){
                new Vector2Int(cell.x+1,cell.y),
                new Vector2Int(cell.x-1,cell.y),
                new Vector2Int(cell.x, cell.y-1),
                new Vector2Int(cell.x, cell.y+1)
            }.Where(_ => ValidPos(_.x, _.y)).ToList();
    }
    public bool TryFindShortestPath(Vector2Int from, Vector2Int to, out List<Vector2Int> path)
    {
        path = new List<Vector2Int>();
        var closed = new List<Vector2Int>();
        var open = new List<Vector2Int>() { from };
        var g = new Dictionary<Vector2Int, int>();
        var h = new Dictionary<Vector2Int, int>();
        var f = new Dictionary<Vector2Int, int>();
        g[from] = 0;
        h[from] = GetHeuristic(from, to);
        f[from] = h[from];
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[from] = -Vector2Int.one;
        while (open.Count > 0)
        {
            var x = open.Aggregate((curmin, _) => h[_] < h[curmin] ? _ : curmin);
            if (x == to)
            {
                var record = new Stack<Vector2Int>();
                record.Push(to);
                ConstructPath(to, ref record, cameFrom);
                path = record.ToList();
                return true;
            }
            open.Remove(x);
            closed.Add(x);
            foreach (var n in GetUnblockedNeighbours(x))
            {
                if (closed.Contains(n)) continue;
                var temp_g = g[x] + Mathf.Abs(x.x - n.x) + Mathf.Abs(x.y - n.y);
                var temp_better = false;
                if (!open.Contains(n))
                {
                    temp_better = true;
                }
                else if (temp_g < g[n])
                {
                    temp_better = true;
                }
                else
                {
                    temp_better = false;
                }
                if (temp_better)
                {
                    cameFrom[n] = x;
                    g[n] = temp_g;
                    h[n] = GetHeuristic(n, to);
                    f[n] = g[n] + h[n];
                    open.Add(n);
                }
            }
        }

        return false;
    }
}