using UnityEngine;

using ChessRun.Board;
using ChessRun.Board.Pieces;
using ChessRun.Board.Controllers;

namespace ChessRun.GInput
{
public enum GameInputState
{
    WAIT_MOVE,
    MOVING,
    FIGHT_CELL,
    UI_INPUT, // disabled GameInput controller
}

public class GameInput : MonoBehaviour
{
    private GameInputState _state;
    
    private bool _isDragged = false;
    private bool _mouseDown = false;
    private Vector3 _mouseBeginDragPos;
    private Vector3 _objectBeginDragPos;
    private int _beforeDragZIndex;
    private Cell _currentCell;

    private GameEngine _gameEngine;
    
    // Start is called before the first frame update
    void Start()
    {
        _gameEngine = GetComponent<GameEngine>();
        _state = GameInputState.WAIT_MOVE;
    }
    
    private void _beginTouch (Vector2 touchPosition)
    {
        if (!СGameController.IsInputState()) return;
        if (_state != GameInputState.WAIT_MOVE) return;
        
        _mouseBeginDragPos = Camera.main.ScreenToWorldPoint(touchPosition);
        _mouseBeginDragPos = new Vector3(_mouseBeginDragPos.x * GameEngine.TO_PIXELS, _mouseBeginDragPos.y * GameEngine.TO_PIXELS, 0);

        int posX = (int) Mathf.Round(_mouseBeginDragPos.x / GameEngine.CELL_SIZE);
        int posY = (int) Mathf.Round(_mouseBeginDragPos.y / GameEngine.CELL_SIZE);

        if (posX > -1 && posY > -1 && posY < _gameEngine.Board.Cells.Length && posX < ChessBoard.W)
        {
            Cell cell = _gameEngine.Board.Cells[posY, posX];
            
            if (cell && !cell.HasFight && cell.Piece && cell.Piece.Relation == PieceRelation.SELF)
            {
                _state = GameInputState.MOVING;
                _isDragged = true;
                _currentCell = cell;
                BasePiece piece = cell.Piece;
                _beforeDragZIndex = piece.zIndex;
                piece.zIndex = ZIndex.DRAGGING_PIECE;
                Debug.Log("begin drag at "+posX + " " +posY);

                if (GameEngine.GameController != null) {
                    GameEngine.GameController.OnPlayerBeginDragBoard (cell);
                }
            }
            else if (cell && cell.HasFight)
            {
                _state = GameInputState.FIGHT_CELL;
                _currentCell = cell;
            }
        }
    }

    private void _updateTouch(Vector2 touchPosition)
    {
        if (!СGameController.IsInputState()) return;
        if (_state != GameInputState.MOVING) return;
        Vector3 pos3 = Camera.main.ScreenToWorldPoint(touchPosition);
        pos3 = new Vector3(pos3.x * GameEngine.TO_PIXELS, pos3.y * GameEngine.TO_PIXELS, 0);
        _gameEngine.Board.updateDraggingAction(pos3);
    }

    private void _endTouch ()
    {
        if (!СGameController.IsInputState()) return;
        _isDragged = false;

        if (_state == GameInputState.MOVING)
        {
            if (_currentCell && _currentCell.Piece)
            {
                _currentCell.Piece.zIndex = _beforeDragZIndex;
                if (GameEngine.GameController !=null)
                {
                    Debug.Log("End drag piece");
                    GameEngine.GameController.OnPlayerEndDragBoard(_currentCell);
                    _currentCell = null;
                }
            }

            _state = GameInputState.WAIT_MOVE;
            
        } else if (_state == GameInputState.FIGHT_CELL)
        {
            if (_currentCell && _currentCell.HasFight)
            {
                _gameEngine.FightUi.DisplayFightInput(_currentCell);
                SetUiInput(true);
            }
        }
       
    }

    public void SetUiInput(bool isEnabled)
    {
        _state = isEnabled ? GameInputState.UI_INPUT : GameInputState.WAIT_MOVE;
        _currentCell = null;
    }
    
    void Update()
    {
        // handle global mouse
        if (Input.GetMouseButton(0))
        {
            if (!_mouseDown)
            {
                _mouseDown = true;
                _beginTouch(Input.mousePosition);
            }
        }
        else
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                _endTouch();
            }
        }

        if (_isDragged)
        {
            _updateTouch(Input.mousePosition);
        }
        
        return; // no touch now
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch begin");
            }
            
        }
    }
}

}
