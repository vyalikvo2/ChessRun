using UnityEngine;
using DG.Tweening;

using ChessRun.GUI;
using ChessRun.Board.Pieces;

namespace ChessRun.Board.Controllers
{
	public enum GameState
	{
		MY_TURN,
		MY_TURN_HIGHLIGHT_MOVES,
		CALC_ENEMY_MOVE,
		ENEMY_MOVE,
		ANIMATING_MOVE,
		MY_TURN_FIGHTING,
		LEVEL_COMPLETE,
		GAMEOVER,
	};


	public class GameController : MonoBehaviour
	{
		public static GameState State = GameState.MY_TURN;

		[HideInInspector] private Game _game;
		[HideInInspector] private BoardAction _nextAction;

		public event GameEvents.BoardActionEvent onNextActionChanged;

		// Use this for initialization
		void Start()
		{
			_game = GetComponent<Game>() as Game;
			_nextAction = new BoardAction();
		}

		public BoardAction getNextAction()
		{
			return _nextAction;
		}

		public void onPlayerBeginDragBoard(Cell cell)
		{
			changeState(GameState.MY_TURN_HIGHLIGHT_MOVES);
			prepareNextAction(BoardAction.MOVE, cell);
		}

		public void onPlayerEndDragBoard(Cell cell)
		{
			changeState(GameState.MY_TURN);
			_processAction(_nextAction);

			_nextAction = new BoardAction(); // reset next action
		}

		public void momentInteractionComplete()
		{
			endMove();
		}

		public static bool isInputState()
		{
			return State == GameState.MY_TURN ||
			       GameController.State == GameState.MY_TURN_HIGHLIGHT_MOVES;
		}

		public void levelComplete()
		{
			State = GameState.LEVEL_COMPLETE;
			_game.gameUI.setMenu(MenuType.LEVEL_COMPLETE);
		}


		public void startLevel()
		{
			State = GameState.MY_TURN;
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
				_gameOver();
			}

		}

		void changeState(GameState nextState)
		{
			Debug.Log("STATE " + nextState);
			State = nextState;
		}

		public void prepareNextAction(string actionName, Cell cell)
		{
			bool changed = false;
			if (_nextAction.name != actionName || _nextAction.cellFrom != cell)
				changed = true;

			_nextAction.name = actionName;
			_nextAction.cellFrom = cell;

			if (changed) onNextActionChanged(_nextAction);
		}

		public void updateNextActionCell(Cell cell, string newActionName = null)
		{
			bool changed = false;
			if (newActionName != null)
			{
				if (_nextAction.name != newActionName)
				{
					changed = true;
					_nextAction.name = newActionName;
				}
			}

			if (_nextAction.cellTo != cell)
			{
				changed = true;
				_nextAction.cellTo = cell;
			}

			if (changed) onNextActionChanged(_nextAction);
		}

		private void _processAction(BoardAction action)
		{
			_game.board.clearMovingUI();
			switch (action.name)
			{
				case BoardAction.END_LEVEL:
					action.cellFrom.piece.stats.visible = false;
					action.cellFrom.piece.zIndex = ZIndex.FIGHT_UI;
					moveAndScalePieceCallback(action.cellFrom.piece, action.cellTo.pos, 0.4f, new Vector3(0, -0.25f, 0),
						levelComplete);
					break;
				case BoardAction.MOVE:
					if (action.cellTo)
					{
						changeState(GameState.ANIMATING_MOVE);
						_game.board.movePiece(action.cellFrom, action.cellTo);
					}

					break;

				case BoardAction.ATTACK:
					changeState(GameState.MY_TURN_FIGHTING);
					_game.board.attackPiece(action.cellFrom, action.cellTo);
					break;
				case BoardAction.ATTACK_HELP:
					changeState(GameState.MY_TURN_FIGHTING);
					_game.board.attackHelpPiece(action.cellFrom, action.cellTo);
					break;
				case BoardAction.DEFEND_HELP:
					changeState(GameState.MY_TURN_FIGHTING);
					_game.board.defendHelpPiece(action.cellFrom, action.cellTo);
					break;
				case BoardAction.INTERACTION:
					changeState(GameState.ANIMATING_MOVE);
					PieceInteraction.interact(action.cellFrom, action.cellTo);
					break;
			}

			if (action.name != BoardAction.END_LEVEL && action.cellTo)
			{
				if (action.cellTo.piece.hasKing() || action.cellFrom.piece.hasKing() ||
				    (action.cellTo.hasFight && action.cellTo.attackerPiece.hasKing()))
					_game.centerCamera(action.cellTo.transform.position);

			}
		}

		public void movePieceTo(BasePiece piece, Vector2 pos)
		{
			changeState(GameState.ANIMATING_MOVE);
			piece.transform.DOLocalMove(_game.board.getLocal3Position(pos), 0.6f).OnComplete(setPieceCoord);

			void setPieceCoord()
			{
				piece.pos = pos;
				endMove();
			}
		}


		public void movePieceCallback(BasePiece piece, Vector2 pos, TweenCallback callback = null)
		{
			changeState(GameState.ANIMATING_MOVE);
			piece.transform.DOLocalMove(_game.board.getLocal3Position(pos), 0.6f).OnComplete(callback);
		}

		public void moveAndScalePieceCallback(BasePiece piece, Vector2 pos, float scale, Vector3 offset = new Vector3(),
			TweenCallback callback = null)
		{
			changeState(GameState.ANIMATING_MOVE);
			piece.transform.DOScale(scale, 0.6f);
			piece.transform.DOLocalMove(_game.board.getLocal3Position(pos) + offset, 0.6f).OnComplete(callback);
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
			_game.board.botLogic.makeBotMove();
		}

		public void endMove()
		{
			if (State == GameState.ENEMY_MOVE) // enemy have made his move
			{
				changeState(GameState.MY_TURN);
			}
			else if (State == GameState.MY_TURN_FIGHTING) // we attacked enemy
			{
				myMoveComplete();
			}
			else if (State == GameState.CALC_ENEMY_MOVE) // not found enemy move
			{
				changeState(GameState.MY_TURN);
			}
			else
			{
				myMoveComplete();
			}
		}

		private void _gameOver()
		{
			_game.gameUI.setMenu(MenuType.REPLAY);
			changeState(GameState.GAMEOVER);
		}
	}
}
