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

    private Game _game;
    
    // Start is called before the first frame update
    void Start()
    {
        _game = GetComponent<Game>();
        _state = GameInputState.WAIT_MOVE;
    }
    
    public void beginTouch (Vector2 touchPosition)
    {
        if (!GameController.isInputState()) return;
        if (_state != GameInputState.WAIT_MOVE) return;
        
        _mouseBeginDragPos = Camera.main.ScreenToWorldPoint(touchPosition);
        _mouseBeginDragPos = new Vector3(_mouseBeginDragPos.x * Game.TO_PIXELS, _mouseBeginDragPos.y * Game.TO_PIXELS, 0);

        int posX = (int) Mathf.Round(_mouseBeginDragPos.x / Game.CELL_SIZE);
        int posY = (int) Mathf.Round(_mouseBeginDragPos.y / Game.CELL_SIZE);

        if (posX > -1 && posY > -1 && posY < _game.board.cells.Length && posX < ChessBoard.W)
        {
            Cell cell = _game.board.cells[posY, posX];
            
            if (cell && !cell.hasFight && cell.piece && cell.piece.relation == Relation.SELF)
            {
                _state = GameInputState.MOVING;
                _isDragged = true;
                _currentCell = cell;
                BasePiece piece = cell.piece;
                _beforeDragZIndex = piece.zIndex;
                piece.zIndex = ZIndex.DRAGGING_PIECE;
                Debug.Log("begin drag at "+posX + " " +posY);

                if (Game.gameController != null) {
                    Game.gameController.onPlayerBeginDragBoard (cell);
                }
            }
            else if (cell && cell.hasFight)
            {
                _state = GameInputState.FIGHT_CELL;
                _currentCell = cell;
            }
        }
    }

    // update touch position
    public void updateTouch(Vector2 touchPosition)
    {
        if (!GameController.isInputState()) return;
        if (_state != GameInputState.MOVING) return;
        Vector3 pos3 = Camera.main.ScreenToWorldPoint(touchPosition);
        pos3 = new Vector3(pos3.x * Game.TO_PIXELS, pos3.y * Game.TO_PIXELS, 0);
        _game.board.updateDraggingAction(pos3);
    }

    public void endTouch ()
    {
        if (!GameController.isInputState()) return;
        _isDragged = false;

        if (_state == GameInputState.MOVING)
        {
            if (_currentCell && _currentCell.piece)
            {
                _currentCell.piece.zIndex = _beforeDragZIndex;
                if (Game.gameController !=null)
                {
                    Debug.Log("End drag piece");
                    Game.gameController.onPlayerEndDragBoard(_currentCell);
                    _currentCell = null;
                }
            }

            _state = GameInputState.WAIT_MOVE;
            
        } else if (_state == GameInputState.FIGHT_CELL)
        {
            if (_currentCell && _currentCell.hasFight)
            {
                _game.fightUI.displayFightInput(_currentCell);
                setUIInput(true);
            }
        }
       
    }

    public void setUIInput(bool enabled)
    {
        _state = enabled ? GameInputState.UI_INPUT : GameInputState.WAIT_MOVE;
        _currentCell = null;
    }

    // Update is called once per frame
    void Update()
    {
        // handle global mouse
        if (Input.GetMouseButton(0))
        {
            if (!_mouseDown)
            {
                _mouseDown = true;
                beginTouch(Input.mousePosition);
            }
        }
        else
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                endTouch();
            }
        }

        if (_isDragged)
        {
            updateTouch(Input.mousePosition);
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
