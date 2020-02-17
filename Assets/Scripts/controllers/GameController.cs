using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameController : MonoBehaviour
{

	public static string state = GameState.MY_TURN;

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
		changeState(GameState.MY_TURN);
		processAction(nextAction);

		nextAction = new GameAction(); // reset next action
	}

	public void momentInteractionComplete()
	{
		endMove();
	}

	public static bool isInputState()
	{
		return state == GameState.MY_TURN ||
		       GameController.state == GameState.MY_TURN_HIGHLIGHT_MOVES;
	}

	public void levelComplete()
	{
		state = GameState.LEVEL_COMPLETE;
		game.nextLevel ();
	}


	public void startLevel()
	{
		state = GameState.MY_TURN;
	}

	public void fightEnded(bool isGameOver)
	{
		Debug.Log("FIGHT ENDED " + isGameOver);
		if (!isGameOver)
		{
			endMove();
		}
		else
		{
			gameOver();
		}
		
	}

	void changeState(string nextState)
	{
		Debug.Log("STATE " +nextState);
		state = nextState;
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
				changeState(GameState.MY_TURN_FIGHTING);
				game.board.attackPiece(action.cellFrom, action.cellTo);
				break;
			case GameAction.ATTACK_HELP:
				changeState(GameState.MY_TURN_FIGHTING);
				game.board.attackHelpPiece(action.cellFrom, action.cellTo);
				break;
			case GameAction.DEFEND_HELP:
				changeState(GameState.MY_TURN_FIGHTING);
				game.board.defendHelpPiece(action.cellFrom, action.cellTo);
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
		piece.transform.DOLocalMove(game.board.getLocal3Position(pos), 0.6f).OnComplete(setPieceCoord);

		void setPieceCoord()
		{
			piece.pos = pos;
			endMove();
		}
	}


	public void beginBotMove()
	{
		changeState(GameState.ENEMY_MOVE);
	}
	
	public void myMoveAttackCellContinue()
	{
		changeState(GameState.MY_TURN_FIGHTING);
	}

	public void myMoveComplete()
	{
		changeState(GameState.CALC_ENEMY_MOVE);
		game.board.botLogic.makeBotMove();
	}
	
	public void endMove()
	{
		if (state == GameState.ENEMY_MOVE) // enemy have made his move
		{
			changeState(GameState.MY_TURN);
		} 
		else if (state == GameState.MY_TURN_FIGHTING) // we attacked enemy
		{
			myMoveComplete();
		}
		else if (state == GameState.CALC_ENEMY_MOVE) // not found enemy move
		{
			changeState(GameState.MY_TURN);
		} else 
		{
			myMoveComplete();
		}
	}

	private void gameOver()
	{
		game.gameInput.setUIInput(true);
		game.gameUI.setMenuVisible(true);
		changeState(GameState.GAMEOVER);
	}
}
