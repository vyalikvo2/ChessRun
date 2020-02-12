using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	public static string gameState = GameState.MY_TURN_NO_HIGHLIGHT;

	[HideInInspector] private Game game;
	[HideInInspector] public GameAction nextAction;
	
	public event EventController.ActionEvent onNextActionChanged;

	// Use this for initialization
	void Start () {
		game = GetComponent<Game>() as Game;
		nextAction = new GameAction();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void onPlayerBeginDragBoard(Cell cell)
	{
		changeState(GameState.MY_TURN_HIGHLIGHT_MOVES);
		prepareNextAction(GameAction.MOVE, cell);
		
		Debug.Log("begin drag");
	}

	public void onPlayerEndDragBoard(Cell cell)
	{
		changeState(GameState.MY_TURN_NO_HIGHLIGHT);
		processAction(nextAction);
		
		nextAction = new GameAction(); // reset next action
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
					game.board.movePiece(action.cellFrom, action.cellTo);
				}
				break;
			
			case GameAction.ATTACK:
				game.board.attackPiece(action.cellFrom, action.cellTo);
				break;
			
			case GameAction.INTERACTION:
				PieceInteraction.interact(action.cellFrom, action.cellTo);
				break;
		}

		if (action.name != GameAction.END_LEVEL && action.cellTo )
		{
			game.centerCamera(action.cellTo.transform.position);
		}
	}
	

}
