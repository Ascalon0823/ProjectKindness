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
    public Vector2Int GetCellFromCursor()
    {
        return GetCellFromWordPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!BattleController.CurrentBattle.PlayerTurn) return;
        var cell = GetCellFromCursor();
        Debug.Log($"Click on battle field { cell }");
        if (BattleController.CurrentBattle.SelectedCard != null)
        {
            if (BattleController.CurrentBattle.SelectedCard is UIUnitCard && GetUnitControllerFromCoord(cell) == null)
            {

                var unit = (BattleController.CurrentBattle.SelectedCard as UIUnitCard).LoadedUnit;
                if (BattleController.CurrentBattle.PlayerHero.CurrentCommandPoint < unit.DeployCost) return;
                var controller = BattleController.CurrentBattle.DeployUnit(unit, cell.x, cell.y, true);
                _unitCoord[cell] = controller;
            }
        }
    }
}
