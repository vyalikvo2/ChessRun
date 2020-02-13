using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    
    private bool isDragged = false;
    private bool mouseDown = false;
    private Vector3 mouseBeginDragPos;
    private Vector3 objectBeginDragPos;
    private int beforeDragZIndex;
    private Cell currentCell;

    private Game game;
    
    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<Game>();
    }
    
    public void beginTouch (Vector2 touchPosition)
    {
        if (!GameController.isInputState()) return;
        
        mouseBeginDragPos = Camera.main.ScreenToWorldPoint(touchPosition);
        mouseBeginDragPos = new Vector3(mouseBeginDragPos.x * Game.TO_PIXELS, mouseBeginDragPos.y * Game.TO_PIXELS, 0);

        int posX = (int) Mathf.Round(mouseBeginDragPos.x / Game.CELL_SIZE);
        int posY = (int) Mathf.Round(mouseBeginDragPos.y / Game.CELL_SIZE);

        if (posX > -1 && posY > -1 && posY < game.board.cells.Length && posX < 50)
        {
            Cell cell = game.board.cells[posY, posX];

            if (cell && cell.piece && cell.piece.relation == Relation.SELF)
            {
                isDragged = true;
                currentCell = cell;
                BasePiece piece = cell.piece;
                beforeDragZIndex = piece.zIndex;
                piece.zIndex = ZIndex.DRAGGING_PIECE;
                Debug.Log("begin drag at "+posX + " " +posY);

                if (Game.gameController) {
                    Game.gameController.onPlayerBeginDragBoard (cell);
                }
            }
            
        }
    }

    // update touch position
    public void updateTouch(Vector2 touchPosition)
    {
        if (!GameController.isInputState()) return;
        Vector3 pos3 = Camera.main.ScreenToWorldPoint(touchPosition);
        pos3 = new Vector3(pos3.x * Game.TO_PIXELS, pos3.y * Game.TO_PIXELS, 0);
        game.board.updateDraggingAction(pos3);
    }

    public void endTouch ()
    {
        if (!GameController.isInputState()) return;
        isDragged = false;
        if (currentCell && currentCell.piece)
        {
            currentCell.piece.zIndex = beforeDragZIndex;
            if (Game.gameController)
            {
                Debug.Log("End drag piece");
                Game.gameController.onPlayerEndDragBoard(currentCell);
                currentCell = null;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        // handle global mouse
        if (Input.GetMouseButton(0))
        {
            if (!mouseDown)
            {
                mouseDown = true;
                beginTouch(Input.mousePosition);
            }
        }
        else
        {
            if (mouseDown)
            {
                mouseDown = false;
                endTouch();
            }
        }

        if (isDragged)
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
