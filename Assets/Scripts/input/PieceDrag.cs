using UnityEngine;
using System.Collections;

public class PieceDrag : MonoBehaviour {
	
	private bool isDragged = false;
	private Vector3 mouseBeginDragPos;
	private Vector3 objectBeginDragPos;
	private int beforeDragZIndex;

	public BasePiece piece;

	public void OnMouseDown ()
	{
		isDragged = true;
		mouseBeginDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		objectBeginDragPos = transform.position;

		beforeDragZIndex = piece.zIndex;
		piece.zIndex = ZIndex.DRAGGING_PIECE;

		if (Game.gameStateController) {
			Game.gameStateController.onPlayerBeginDragPiece (piece);
		}

	}

	public void OnMouseUp ()
	{
		isDragged = false;
		piece.zIndex = beforeDragZIndex;

		if (Game.gameStateController) {
			Game.gameStateController.onPlayerEndDragPiece(piece);
		}
	}

	void Update()
	{
		/*if (isDragged) {
			Vector3 mousePos = Input.mousePosition;
			mousePos = Camera.main.ScreenToWorldPoint(mousePos);
			gameObject.transform.position = objectBeginDragPos+(mousePos-mouseBeginDragPos);
		}*/
	}
}
