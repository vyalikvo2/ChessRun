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


	public class СGameController : MonoBehaviour
	{
		private static GameState _state = GameState.MY_TURN;

		[HideInInspector] private GameEngine _gameEngine;
		[HideInInspector] private BoardAction _nextAction;

		public event GameEvents.BoardActionEvent OnNextActionChanged;

		// Use this for initialization
		void Start()
		{
			_gameEngine = GetComponent<GameEngine>() as GameEngine;
			_nextAction = new BoardAction();
		}

		public void StartLevel()
		{
			_state = GameState.MY_TURN;
		}
		
		public void LevelComplete()
		{
			_state = GameState.LEVEL_COMPLETE;
			_gameEngine.GameUI.SetMenu(MenuType.LEVEL_COMPLETE);
		}

		
		public void FightEnded(bool isGameOver)
		{
			Debug.Log("FIGHT ENDED " + isGameOver);
			if (!isGameOver)
			{
				EndMove();
			}
			else
			{
				_gameOver();
			}
		}

		
		public BoardAction GetNextAction()
		{
			return _nextAction;
		}

		public void OnPlayerBeginDragBoard(Cell cell)
		{
			changeState(GameState.MY_TURN_HIGHLIGHT_MOVES);
			_prepareNextAction(BoardAction.MOVE, cell);
		}

		public void OnPlayerEndDragBoard(Cell cell)
		{
			changeState(GameState.MY_TURN);
			_processAction(_nextAction);

			_nextAction = new BoardAction(); // reset next action
		}

		public void MomentInteractionComplete()
		{
			EndMove();
		}

		public static bool IsInputState()
		{
			return _state == GameState.MY_TURN ||
			       СGameController._state == GameState.MY_TURN_HIGHLIGHT_MOVES;
		}

		void changeState(GameState nextState)
		{
			Debug.Log("STATE " + nextState);
			_state = nextState;
		}

		public void UpdateNextActionCell(Cell cell, string newActionName = null)
		{
			bool changed = false;
			if (newActionName != null)
			{
				if (_nextAction.Name != newActionName)
				{
					changed = true;
					_nextAction.Name = newActionName;
				}
			}

			if (_nextAction.CellTo != cell)
			{
				changed = true;
				_nextAction.CellTo = cell;
			}

			if (changed && OnNextActionChanged!=null) OnNextActionChanged(_nextAction);
		}
		
		public void MovePieceTo(BasePiece piece, Vector2 pos)
		{
			changeState(GameState.ANIMATING_MOVE);
			piece.transform.DOLocalMove(_gameEngine.Board.GetLocal3Position(pos), 0.6f).OnComplete(delegate {
				piece.pos = pos;
				EndMove();
			});
	}
		public void MovePieceCallback(BasePiece piece, Vector2 pos, TweenCallback callback = null)
		{
			changeState(GameState.ANIMATING_MOVE);
			piece.transform.DOLocalMove(_gameEngine.Board.GetLocal3Position(pos), 0.6f).OnComplete(callback);
		}

		public void MoveAndScalePieceCallback(BasePiece piece, Vector2 pos, float scale, Vector3 offset = new Vector3(),
			TweenCallback callback = null)
		{
			changeState(GameState.ANIMATING_MOVE);
			piece.transform.DOScale(scale, 0.6f);
			piece.transform.DOLocalMove(_gameEngine.Board.GetLocal3Position(pos) + offset, 0.6f).OnComplete(callback);
		}

		public void BeginBotMove()
		{
			changeState(GameState.ENEMY_MOVE);
		}

		public void MyMoveAttackCellContinue()
		{
			changeState(GameState.MY_TURN_FIGHTING);
		}

		public void MyMoveComplete()
		{
			changeState(GameState.CALC_ENEMY_MOVE);
			_gameEngine.Board.BotLogic.MakeBotMove();
		}

		public void EndMove()
		{
			if (_state == GameState.ENEMY_MOVE) // enemy have made his move
			{
				changeState(GameState.MY_TURN);
			}
			else if (_state == GameState.MY_TURN_FIGHTING) // we attacked enemy
			{
				MyMoveComplete();
			}
			else if (_state == GameState.CALC_ENEMY_MOVE) // not found enemy move
			{
				changeState(GameState.MY_TURN);
			}
			else
			{
				MyMoveComplete();
			}
		}
		
		private void _processAction(BoardAction action)
		{
			_gameEngine.Board.ClearMovingUI();
			switch (action.Name)
			{
				case BoardAction.END_LEVEL:
					action.CellFrom.Piece.Stats.visible = false;
					action.CellFrom.Piece.zIndex = ZIndex.FIGHT_UI;
					MoveAndScalePieceCallback(action.CellFrom.Piece, action.CellTo.Pos, 0.4f, new Vector3(0, -0.25f, 0),
						LevelComplete);
					break;
				case BoardAction.MOVE:
					if (action.CellTo)
					{
						changeState(GameState.ANIMATING_MOVE);
						_gameEngine.Board.MovePiece(action.CellFrom, action.CellTo);
					}

					break;

				case BoardAction.ATTACK:
					changeState(GameState.MY_TURN_FIGHTING);
					_gameEngine.Board.AttackPiece(action.CellFrom, action.CellTo);
					break;
				case BoardAction.ATTACK_HELP:
					changeState(GameState.MY_TURN_FIGHTING);
					_gameEngine.Board.AttackHelpPiece(action.CellFrom, action.CellTo);
					break;
				case BoardAction.DEFEND_HELP:
					changeState(GameState.MY_TURN_FIGHTING);
					_gameEngine.Board.DefendHelpPiece(action.CellFrom, action.CellTo);
					break;
				case BoardAction.INTERACTION:
					changeState(GameState.ANIMATING_MOVE);
					PieceInteraction.Interact(action.CellFrom, action.CellTo);
					break;
			}

			if (action.Name != BoardAction.END_LEVEL && action.CellTo)
			{
				if (action.CellTo.Piece.HasKing() || action.CellFrom.Piece.HasKing() ||
				    (action.CellTo.HasFight && action.CellTo.AttackerPiece.HasKing()))
					_gameEngine.CenterCamera(action.CellTo.transform.position);

			}
		}

		
		private void _prepareNextAction(string actionName, Cell cell)
		{
			bool changed = _nextAction.Name != actionName || _nextAction.CellFrom != cell;

			_nextAction.Name = actionName;
			_nextAction.CellFrom = cell;

			if (changed && OnNextActionChanged != null) OnNextActionChanged(_nextAction);
		}

		private void _gameOver()
		{
			_gameEngine.GameUI.SetMenu(MenuType.REPLAY);
			changeState(GameState.GAMEOVER);
		}
	}
}
