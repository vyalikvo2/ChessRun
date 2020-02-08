using UnityEngine;
using System.Collections;

public class GameStateController : MonoBehaviour {
	
	public static string gameState = GameState.MY_TURN_NO_HIGHLIGHT;

	[HideInInspector]
	private Game game;

	private BasePiece _currentMovingPiece;

	// Use this for initialization
	void Start () {
		game = GetComponent<Game>() as Game;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameState == GameState.MY_TURN_HIGHLIGHT_MOVES) {
			game.board.updateHighlightMoveTo(_currentMovingPiece);
		}
	}

	public void onPlayerBeginDragPiece(BasePiece piece)
	{
		changeState(GameState.MY_TURN_HIGHLIGHT_MOVES);
		_currentMovingPiece = piece;
		game.board.setDraggingPiece(piece);
	}

	public void onPlayerEndDragPiece(BasePiece piece)
	{
		game.board.dragAndDropIfCan(piece);
		changeState(GameState.MY_TURN_NO_HIGHLIGHT);
		game.board.setDraggingPiece(null);
		_currentMovingPiece = null;
	}

	public void levelComplete()
	{
		gameState = GameState.LEVEL_COMPLETE;
		game.board.clearBoard ();
		game.nextLevel ();
	}


	public void startLevel()
	{
		gameState = GameState.MY_TURN_NO_HIGHLIGHT;
	}

	void changeState(string nextState)
	{
		gameState = nextState;
	}
	

}
