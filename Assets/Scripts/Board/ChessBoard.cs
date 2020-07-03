using System;
using UnityEngine;
using System.Collections.Generic;

using ChessRun.Board.Pieces;
using ChessRun.Board.Pieces.Highlight;
using ChessRun.Board.Controllers;
using ChessRun.Board.Bot;

using ChessRun.GUI;

namespace ChessRun.Board
{

	public class ChessBoard : MonoBehaviour
	{
		public static int W = 50;
		public static int H = 50;

		private GameEngine _gameEngine;

		public Cell[,] Cells = new Cell[H, W];

		[SerializeField] private GameObject _cellPrefab;

		private List<HighlightPiece> _piecesHighlighted = new List<HighlightPiece>();
		private HighlightPiece _currentPosPieceHighligh;
		private ScalableArrow _currentArrow;
		
		[HideInInspector] public FightController Fight;
		[HideInInspector] public BotLogic BotLogic;
		
		
		public void Setup()
		{
			_gameEngine = GetComponent<GameEngine>();
			
			Fight = GetComponent<FightController>();
			Fight.Setup(this);

			BotLogic = GetComponent<BotLogic>();

			GameEngine.GameController.OnNextActionChanged += _onNextActionChanged;
		}
		
		public void CreateByMap(string[] map)
		{
			int maxY = map.Length;
			for (int i = 0; i < maxY; i++)
			{
				for (var j = 0; j < map[i].Length; j++)
				{
					if (map[i][j] == TypePiece.NONE) continue;
					GameObject obj = Instantiate(_cellPrefab, gameObject.transform) as GameObject;
					Cell cell = obj.GetComponent<Cell>();

					int i2 = maxY - i - 1;
					cell.Setup(new Vector2(j, i2));

					Cells[i2, j] = cell;

					_createPieceFromMap(map[i][j], new Vector2(j, i2));

					if (map[i][j] == TypePiece.KING)
					{
						_gameEngine.CenterCamera(cell.transform.position, false);
					}
				}
			}
		}
		
		
		public void updateDraggingAction(Vector3 pos)
		{
			Cell cell = GameEngine.GameController.GetNextAction().CellFrom;
			Vector3 pos1 = new Vector3(cell.Pos.x * GameEngine.CELL_SIZE, cell.Pos.y * GameEngine.CELL_SIZE, 0);
			_currentArrow.width = (int) Vector3.Distance(pos1, pos);

			Vector3 direction = pos - pos1;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			_currentArrow.setRotation(angle);

			RecalculateDraggingAction(pos);
		}
		
		
		public void MovePiece(Cell cell1, Cell cell2)
		{
			BasePiece piece = cell1.Piece;

			GameEngine.GameController.MovePieceTo(piece, cell2.Pos);

			cell2.Piece = cell1.Piece;
			cell1.Piece = null;
		}
		
		public void AttackPiece(Cell cell1, Cell cell2)
		{
			Fight.BeginAttack(cell1, cell2);
		}
		
		public void ContinueFight(Cell cell2)
		{
			GameEngine.GameController.MyMoveAttackCellContinue();
			if (cell2.AttackerPiece.Relation == PieceRelation.SELF)
			{
				Fight.AnimateFightCellAttackerAttack(cell2);
			}
			else if(cell2.Piece.Relation == PieceRelation.SELF)
			{
				Fight.AnimateFightCellDefenderAttack(cell2);
			}
		}

		public void AttackHelpPiece(Cell cell1, Cell cell2)
		{
			Fight.AnimateAllyHelpAttack(cell1, cell2);
		}
		
		public void DefendHelpPiece(Cell cell1, Cell cell2)
		{
			Fight.AnimateAllyHelpDefend(cell1, cell2);
		}
		
		public bool ProcessKilling(Cell fightCell, FightCellResult fightCellResult)
		{
			bool gameOver = false;
			
			if (fightCellResult == FightCellResult.KILLED_DEFENDER)
			{
				if (fightCell.Piece.Relation == PieceRelation.SELF && (fightCell.Piece.Type == TypePiece.KING || fightCell.Piece.Type == TypePiece.KING_HORSE)) // check gameover
					gameOver = true;

				_destroyPiece (fightCell.Piece);
				fightCell.Piece = fightCell.AttackerPiece;
			} 
			else if (fightCellResult == FightCellResult.KILLED_ATTACKER)
			{
				if (fightCell.AttackerPiece.Relation == PieceRelation.SELF && (fightCell.AttackerPiece.Type == TypePiece.KING || fightCell.AttackerPiece.Type == TypePiece.KING_HORSE)) // check gameover
					gameOver = true;
				
				_destroyPiece (fightCell.AttackerPiece);
			}
			
			fightCell.AttackerPiece = null;
			fightCell.HasFight = false;
			
			return gameOver;
		}

		public void CreateFightAtCell(Cell attacker, Cell defender)
		{
			defender.AttackerPiece = attacker.Piece;
			defender.AttackerPiece.pos = defender.Pos;
			defender.HasFight = true;
			attacker.Piece = null;
		}
		
		
		public Vector3 GetLocal3Position(Vector2 pos)
		{
			return new Vector3(pos.x * GameEngine.POS_TO_COORDS , pos.y * GameEngine.POS_TO_COORDS, 0);
		}
		
		
		public bool IsFreeCellsToMove(Vector2 startPos, Vector2 endPos)
		{
			int xDir = Math.Sign((endPos - startPos).x);
			int yDir = Math.Sign((endPos - startPos).y);

			Vector2 curPos = startPos;
			for (int i = 0; i < 5; i++)
			{
				curPos += new Vector2(xDir, yDir);
				if (curPos == endPos) break;
				
				Cell cell = GetCellAt(curPos);
				if (!cell) return false;
				if (cell.Piece) return false;
			}

			return true;
		}

		public Cell GetCellAt(Vector2 pos){
			return Cells[(int)pos.y,(int)pos.x] as Cell;
		}

		
		// lighing next cell that we can put in a piece
		public void RecalculateDraggingAction(Vector3 pos)
		{
			// find highlighet piece by pos
			bool foundNextCell = false;
			for (int i = 0; i < _piecesHighlighted.Count; i++)
			{
				Vector2 nextBoardPos = new Vector2(Mathf.Round(pos.x/GameEngine.CELL_SIZE), Mathf.Round(pos.y/GameEngine.CELL_SIZE));
				HighlightPiece pieceComponent = _piecesHighlighted[i];
				
				if (pieceComponent.pos.x == nextBoardPos.x && pieceComponent.pos.y == nextBoardPos.y)
				{
					foundNextCell = true;
					Cell prevCell = GameEngine.GameController.GetNextAction().CellFrom;
					Cell nextCell = GetCellAt(nextBoardPos);
					
					if(nextCell.Piece && (nextCell.Piece.Relation == PieceRelation.ENEMY || nextCell.HasFight))
					{
						if (nextCell.HasFight)
						{
							if (nextCell.Piece.Relation == PieceRelation.ENEMY && nextCell.AttackerPiece.Relation == PieceRelation.SELF)
							{
								pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
								GameEngine.GameController.UpdateNextActionCell(nextCell, BoardAction.ATTACK_HELP);
							}
							else if (nextCell.Piece.Relation == PieceRelation.SELF && nextCell.AttackerPiece.Relation == PieceRelation.ENEMY)
							{
								pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
								GameEngine.GameController.UpdateNextActionCell(nextCell, BoardAction.DEFEND_HELP);
							}
							
						}
						else if(nextCell.Piece.Relation == PieceRelation.ENEMY)
						{
							pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
							GameEngine.GameController.UpdateNextActionCell(nextCell, BoardAction.ATTACK);
						}
					} 
					else
					{
						pieceComponent.sprite = BasePiece.H_CELL_MOVE_TO;
						InteractionType interactionType = PieceInteraction.GetType(prevCell, nextCell);
						
						if (interactionType == InteractionType.NONE)
						{
							GameEngine.GameController.UpdateNextActionCell(nextCell, BoardAction.MOVE);
						}
						else if (interactionType == InteractionType.END_LEVEL || interactionType == InteractionType.END_LEVEL_FROM_HORSE)
						{
							GameEngine.GameController.UpdateNextActionCell(nextCell, BoardAction.END_LEVEL);
						}
						else
						{
							GameEngine.GameController.UpdateNextActionCell(nextCell, BoardAction.INTERACTION);
						}
						
					}
					
				} 
				else 
				{
					pieceComponent.sprite = BasePiece.H_CELL_HIGHLIGHTED;
				}
			}

			if (!foundNextCell)
			{
				GameEngine.GameController.UpdateNextActionCell(null, BoardAction.MOVE);
			}

		}
				
		public BasePiece CreatePieceFromChar(char ch)
		{
			GameObject prefab = ResourceCache.getPrefab(BasePiece.BASE_PREFAB);
			GameObject obj = Instantiate(prefab, gameObject.transform, true);
			BasePiece basePiece = _addPieceScriptByChar(obj, ch);
			basePiece.sprite = _getSpriteNameFromChar(ch);
			basePiece.Relation = _getRelationFromChar(ch);
			
			basePiece.Setup(Vector2.zero);

			return basePiece;
		}
		
		public void ClearMovingUI()
		{
			_removeHighlightMoves();
			_currentArrow.visible = false;
		}
				
		public void ClearBoard()
		{
			_removeHighlightMoves ();

			for (int i=0; i<H; i++) {
				for (int j=0; j<W; j++) {
					if(Cells[i,j]){
						_removePiecesAtCell(Cells[i,j]);
						Cells[i,j].Destructor();
						Destroy(Cells[i,j].gameObject);
						Cells[i,j] = null;
					}
				}
			}

			Cells = new Cell[H, W];
		}
		
		private HighlightPiece _createHighLight(Vector2 pos)
		{
			GameObject prefab = ResourceCache.getPrefab(BasePiece.BASE_PREFAB);
			GameObject obj = Instantiate(prefab, gameObject.transform, true);
			HighlightPiece hightlightPiece = obj.AddComponent<HighlightPiece>();
			hightlightPiece.Setup(pos);

			hightlightPiece.sprite = BasePiece.H_CELL_HIGHLIGHTED;
		
			return hightlightPiece;
		}

		private BasePiece _addPieceScriptByChar(GameObject obj, char c)
		{
			switch (c)
			{
				case TypePiece.KING:
					return obj.AddComponent<King>();
				
				case TypePiece.KING_HORSE:
					return obj.AddComponent<KingHorse>();
				
				case TypePiece.PAWN: 
				case TypePiece.PAWN_ENEMY:
					return obj.AddComponent<Pawn>();
				
				case TypePiece.HORSE: 
				case TypePiece.HORSE_ENEMY:
					return obj.AddComponent<Horse>();
				
				case TypePiece.BISHOP: 
				case TypePiece.BISHOP_ENEMY:
					return obj.AddComponent<Bishop>();
				
				case TypePiece.ROOK: 
				case TypePiece.ROOK_ENEMY:
					return obj.AddComponent<Rook>();
				
				case TypePiece.QUEEN: 
				case TypePiece.QUEEN_ENEMY:
					return obj.AddComponent<Queen>();
				
							
				case TypePiece.BUILDING_HOME:
					return obj.AddComponent<Home>();
			}
			
			return obj.AddComponent<King>();
		}

		private string _getSpriteNameFromChar(char c)
		{
			switch (c)
			{
				//self
				case TypePiece.KING:
					return BasePiece.KING_WHITE;
				case TypePiece.KING_HORSE:
					return BasePiece.KING_HORSE_WHITE;
				case TypePiece.PAWN:
					return BasePiece.PAWN_WHITE;
				case TypePiece.HORSE:
					return BasePiece.HORSE_WHITE;
				case TypePiece.BISHOP:
					return BasePiece.BISHOP_WHITE;
				case TypePiece.ROOK:
					return BasePiece.ROOK_WHITE;
				case TypePiece.QUEEN:
					return BasePiece.QUEEN_WHITE;
				// enemy
				case TypePiece.PAWN_ENEMY:
					return BasePiece.PAWN_BLACK;
				case TypePiece.HORSE_ENEMY:
					return BasePiece.HORSE_BLACK;
				case TypePiece.BISHOP_ENEMY:
					return BasePiece.BISHOP_BLACK;
				case TypePiece.ROOK_ENEMY:
					return BasePiece.ROOK_BLACK;
				case TypePiece.QUEEN_ENEMY:
					return BasePiece.QUEEN_BLACK;
				//buildings
				case TypePiece.BUILDING_HOME:
					return BasePiece.BUILDING_HOME_CASTLE;
			}

			return BasePiece.KING_WHITE;
		}
		
		private int _getRelationFromChar(char ch)
		{
			if (ch == TypePiece.KING || ch == TypePiece.KING_HORSE)
				return PieceRelation.SELF;

			switch (ch)
			{
				// my pieces
				case TypePiece.KING:
				case TypePiece.KING_HORSE:
				case TypePiece.PAWN:	
				case TypePiece.HORSE:
				case TypePiece.BISHOP:
				case TypePiece.ROOK:
				case TypePiece.QUEEN:
					return PieceRelation.SELF;
				// buildings
				case TypePiece.BUILDING_HOME:
					return PieceRelation.BUILDING;
			}

			return PieceRelation.ENEMY;
		}

		private void _addPiece(BasePiece piece)
		{
			Cells[(int) piece.pos.y, (int) piece.pos.x].Piece = piece;
		}

		private void _onNextActionChanged(BoardAction action)
		{
			Debug.Log(action.Name);
			switch (action.Name)
			{
				case BoardAction.MOVE:
					_showMoveToAction(action.CellFrom);
					_currentArrow.iconVisible = false;
					break;
				case BoardAction.ATTACK:
					_currentArrow.iconVisible = true;
					_currentArrow.icon.Sprite = InteractionIcon.ICON_ATTACK;
					break;
				case BoardAction.INTERACTION:
					_currentArrow.iconVisible = true;
					_currentArrow.icon.Sprite = InteractionIcon.ICON_INTERACTION;
					break;

			}
		}

		private void _showMoveToAction(Cell cell)
		{
			if (!_currentArrow)
			{
				GameObject arrowPrefab = ResourceCache.getPrefab("prefabs/ScalableArrow");
				GameObject arrow = Instantiate(arrowPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				arrow.transform.SetParent(transform);
				_currentArrow = arrow.GetComponent<ScalableArrow>();
				_currentArrow.Setup();
			}

			_currentArrow.visible = true;
			_currentArrow.transform.localPosition =
				new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, 0);

			if (!_currentPosPieceHighligh) _highlightMoves(cell.Piece);
		}

		private void _destroyPiece(BasePiece piece)
		{
			Destroy (piece.gameObject);
		}

		private void _highlightMoves(BasePiece piece)
		{
			for (int i=0; i < piece.Moves.Count; i++) {
				Vector2 p = piece.pos+piece.Moves[i];
				
				if (p.x < 0 || p.y < 0) continue;
				if (!_canMoveToCellAt(p, piece)) continue;
				
				Cell cell = GetCellAt(p);
				
				if(!cell) continue;
				if (cell.Piece && !cell.Piece.IsInteractableWith(piece) && cell.Piece.Type != TypePiece.BUILDING_HOME) continue; // can move only to empty cell or home
				
				_createHighLightAt(p, piece.pos);
			}

			if (piece.InteractiveMoves != null)
			{
				for (int i=0; i<piece.InteractiveMoves.Count; i++) {
					Vector2 p = piece.pos + piece.InteractiveMoves[i].Move;
					if (p.x < 0 || p.y < 0 || !GetCellAt(p) || !_canMoveToCellAt(p, piece, true) ) continue;
					_createHighLightAt(p, piece.pos);
				}
			}
			
			if (piece.MovesAttack != null)
			{
				for (int i=0; i<piece.MovesAttack.Count; i++) 
				{
					Vector2 p = piece.pos + piece.MovesAttack[i];
					
					if (p.x < 0 || p.y < 0) continue;
					if (!_canMoveToCellAt(p, piece)) continue;
				
					Cell cell = GetCellAt(p);
				
					if(!cell) continue;

					BasePiece pieceTo = cell.Piece;
					if (!pieceTo) continue; // can attack to not empty cell
					if (pieceTo.Relation != PieceRelation.ENEMY && !cell.HasFight) continue; // can attack only enemy or fightcell

					_createHighLightAt(p, piece.pos);
				}
			}
		
			_createHighLightAt(piece.pos, piece.pos, true);
		}

		private void _createHighLightAt(Vector2 pos, Vector2 posFrom, bool isCurrentPos = false)
		{
			HighlightPiece component = _createHighLight(pos);
			
			component.pos = pos;
			component.zIndex = ZIndex.PIECES_HIGHLIGHT;
			component.AnimateScale(0, 1.0f);
			
			if (!isCurrentPos) {
				component.sprite = BasePiece.H_CELL_HIGHLIGHTED;
				_piecesHighlighted.Add (component);
			} 
			else
			{
				component.sprite = BasePiece.H_CELL_CURRENT;
				_currentPosPieceHighligh = component;
			}
		}

		private void _removeHighlightMoves()
		{
			for (int i=0; i<_piecesHighlighted.Count; i++) 
			{
				Destroy(_piecesHighlighted[i].gameObject);
			}
			
			if (_currentPosPieceHighligh) 
			{
				Destroy (_currentPosPieceHighligh.gameObject);
			}

			_piecesHighlighted = new List<HighlightPiece> ();
			_currentPosPieceHighligh = null;
		}

		private bool _canMoveToCellAt(Vector2 pos, BasePiece piece, bool isInteraction = false){

			Cell cell = GetCellAt (pos);
			
			if(!cell) return false;
			
			if (!piece.CanJump)
			{
				if (!IsFreeCellsToMove(piece.pos, pos)) return false;
			}

			if (!cell.Piece)
			{
				return true;
			}
			
			if (cell.Piece.Relation == PieceRelation.ENEMY && isInteraction)
				return false;
			
			if (cell.Piece.IsInteractableWith(piece))
			{
				return true;
			}

			if (cell.Piece.Relation == PieceRelation.ENEMY)
			{
				return true;
			}
			
			if (cell.HasFight && (cell.Piece.Relation == PieceRelation.ENEMY || cell.AttackerPiece.Relation == PieceRelation.ENEMY))
			{
				return true;
			}

			return false;
		}

		private void _removePieces(List<BasePiece> list){
			for (int i=0; i<list.Count; i++) {
				list[i].Destructor();
				Destroy(list[i].gameObject);
			}
		}

		private void _removePiecesAtCell(Cell cell)
		{
			if (cell.Piece)
			{
				cell.Piece.Destructor();
				Destroy(cell.Piece.gameObject);
			}
			if (cell.AttackerPiece)
			{
				cell.AttackerPiece.Destructor();
				Destroy(cell.AttackerPiece.gameObject);
			}
		}
		
		private void _createPieceFromMap(char ch, Vector2 pos)
		{
			if (ch == TypePiece.EMPTY || ch == TypePiece.NONE)
				return;
			
			BasePiece basePiece = CreatePieceFromChar(ch);
			basePiece.pos = pos;
			
			_addPiece(basePiece);

		}

	}
}