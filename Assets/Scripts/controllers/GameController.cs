using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameController : MonoBehaviour
{

	public static string gameState = GameState.MY_TURN_NO_HIGHLIGHT;

	[HideInInspector] private Game game;
	[HideInInspector] public GameAction nextAction;

	public event EventController.ActionEvent onNextActionChanged;

	// Use this for initialization
	void Start()
	{
		game = GetComponent<Game>() as Game;
		nextAction = new GameAction();
	}

	public void onPlayerBeginDragBoard(Cell cell)
	{
		changeState(GameState.MY_TURN_HIGHLIGHT_MOVES);
		prepareNextAction(GameAction.MOVE, cell);
	}

	public void onPlayerEndDragBoard(Cell cell)
	{
		changeState(GameState.MY_TURN_NO_HIGHLIGHT);
		processAction(nextAction);

		nextAction = new GameAction(); // reset next action
	}

	public void momentInteractionComplete()
	{
		changeState(GameState.MY_TURN_NO_HIGHLIGHT);
	}

	public static bool isInputState()
	{
		return gameState == GameState.MY_TURN_NO_HIGHLIGHT ||
		       GameController.gameState == GameState.MY_TURN_HIGHLIGHT_MOVES;
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

	public void fightEnded()
	{
		changeState(GameState.MY_TURN_NO_HIGHLIGHT);
	}

	void changeState(string nextState)
	{
		gameState = nextState;
	}

	public void prepareNextAction(string actionName, Cell cell)
	{
		bool changed = false;
		if (nextAction.name != actionName || nextAction.cellFrom != cell)
			changed = true;
		
		nextAction.name = actionName;
		nextAction.cellFrom = cell;
		
		if(changed) onNextActionChanged(nextAction);
	}

	public void updateNextActionCell(Cell cell, string newActionName = null)
	{
		bool changed = false;
		if (newActionName != null)
		{
			if (nextAction.name != newActionName)
			{
				changed = true;
				nextAction.name = newActionName;
			}
		}

		if (nextAction.cellTo != cell)
		{
			changed = true;
			nextAction.cellTo = cell;
		}
		if (changed) onNextActionChanged(nextAction);
	}

	private void processAction(GameAction action)
	{
		game.board.clearMovingUI();
		Debug.Log("process action "+action.name +"CELLTO "+ action.cellTo);
		switch (action.name)
		{
			case GameAction.END_LEVEL:
				levelComplete();
				break;
			case GameAction.MOVE:
				if (action.cellTo)
				{
					changeState(GameState.ANIMATING_MOVE);
					game.board.movePiece(action.cellFrom, action.cellTo);
				}
				break;
			
			case GameAction.ATTACK:
				changeState(GameState.ANIMATING_FIGHT);
				game.board.attackPiece(action.cellFrom, action.cellTo);
				break;
			case GameAction.INTERACTION:
				changeState(GameState.ANIMATING_MOVE);
				PieceInteraction.interact(action.cellFrom, action.cellTo);
				break;
		}

		if (action.name != GameAction.END_LEVEL && action.cellTo )
		{
			game.centerCamera(action.cellTo.transform.position);
		}
	}

	public void movePieceTo(BasePiece piece, Vector2 pos)
	{
		changeState(GameState.ANIMATING_MOVE);
		Debug.Log(game.board.getLocal3Position(pos));
		piece.transform.DOLocalMove(game.board.getLocal3Position(pos), 0.6f).OnComplete(setPieceCoord);

		void setPieceCoord()
		{
			piece.pos = pos;
			changeState(GameState.MY_TURN_NO_HIGHLIGHT);
		}
	}

}
