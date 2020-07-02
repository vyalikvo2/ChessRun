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

		private Game _game;

		public Cell[,] cells = new Cell[H, W];

		[SerializeField] public GameObject cellPrefab;

		private List<HighlightPiece> _piecesHighlighted = new List<HighlightPiece>();
		private HighlightPiece _currentPosPieceHighligh;
		private ScalableArrow _currentArrow;
		
		[HideInInspector] public FightController fight;
		[HideInInspector] public BotLogic botLogic;
		
		
		public void Setup()
		{
			_game = GetComponent<Game>();
			
			fight = GetComponent<FightController>();
			fight.Setup(this);

			botLogic = GetComponent<BotLogic>();

			Game.gameController.onNextActionChanged += _onNextActionChanged;
		}
		
		public void CreateByMap(string[] map)
		{
			int maxY = map.Length;
			for (int i = 0; i < maxY; i++)
			{
				for (var j = 0; j < map[i].Length; j++)
				{
					if (map[i][j] == TypePiece.NONE) continue;
					GameObject obj = Instantiate(cellPrefab, gameObject.transform) as GameObject;
					Cell cell = obj.GetComponent<Cell>();

					int i2 = maxY - i - 1;
					cell.Setup(new Vector2(j, i2));

					cells[i2, j] = cell;

					createPieceFromMap(map[i][j], new Vector2(j, i2));

					if (map[i][j] == TypePiece.KING)
					{
						_game.centerCamera(cell.transform.position, false);
					}
				}
			}
		}

		private void createPieceFromMap(char ch, Vector2 pos)
		{
			if (ch == TypePiece.EMPTY || ch == TypePiece.NONE)
				return;
			
			BasePiece basePiece = createPieceFromChar(ch);
			basePiece.pos = pos;
			
			addPiece(basePiece);

		}

		public BasePiece createPieceFromChar(char ch)
		{
			GameObject prefab = ResourceCache.getPrefab(BasePiece.BASE_PREFAB);
			GameObject obj = Instantiate(prefab, gameObject.transform, true);
			BasePiece basePiece = _addPieceScriptByChar(obj, ch);
			basePiece.sprite = _getSpriteNameFromChar(ch);
			basePiece.relation = _getRelationFromChar(ch);
			
			basePiece.Setup(Vector2.zero);

			return basePiece;
		}
		
		
		public HighlightPiece createHighLight(Vector2 pos)
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
				return Relation.SELF;

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
					return Relation.SELF;
				// buildings
				case TypePiece.BUILDING_HOME:
					return Relation.BUILDING;
			}

			return Relation.ENEMY;
		}

		public void addPiece(BasePiece piece)
		{
			cells[(int) piece.pos.y, (int) piece.pos.x].piece = piece;
		}

		private void _onNextActionChanged(BoardAction action)
		{
			Debug.Log(action.name);
			switch (action.name)
			{
				case BoardAction.MOVE:
					showMoveToAction(action.cellFrom);
					_currentArrow.iconVisible = false;
					break;
				case BoardAction.ATTACK:
					_currentArrow.iconVisible = true;
					_currentArrow.icon.sprite = InteractionIcon.ICON_ATTACK;
					break;
				case BoardAction.INTERACTION:
					_currentArrow.iconVisible = true;
					_currentArrow.icon.sprite = InteractionIcon.ICON_INTERACTION;
					break;

			}
		}

		public void showMoveToAction(Cell cell)
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

			if (!_currentPosPieceHighligh) highlightMoves(cell.piece);
		}

		public void updateDraggingAction(Vector3 pos)
		{
			Cell cell = Game.gameController.getNextAction().cellFrom;
			Vector3 pos1 = new Vector3(cell.pos.x * Game.CELL_SIZE, cell.pos.y * Game.CELL_SIZE, 0);
			_currentArrow.width = (int) Vector3.Distance(pos1, pos);

			Vector3 direction = pos - pos1;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			_currentArrow.setRotation(angle);

			recalculateDraggingAction(pos);
		}

		public void movePiece(Cell cell1, Cell cell2)
		{
			BasePiece piece = cell1.piece;

			Game.gameController.movePieceTo(piece, cell2.pos);

			cell2.piece = cell1.piece;
			cell1.piece = null;
		}
		
		public void attackPiece(Cell cell1, Cell cell2)
		{
			fight.beginAttack(cell1, cell2);
		}
		
		public void continueFight(Cell cell2)
		{
			Game.gameController.myMoveAttackCellContinue();
			if (cell2.attackerPiece.relation == Relation.SELF)
			{
				fight.animateFightCellAttackerAttack(cell2);
			}
			else if(cell2.piece.relation == Relation.SELF)
			{
				fight.animateFightCellDefenderAttack(cell2);
			}
		}

		public void attackHelpPiece(Cell cell1, Cell cell2)
		{
			fight.animateAllyHelpAttack(cell1, cell2);
		}
		
		public void defendHelpPiece(Cell cell1, Cell cell2)
		{
			fight.animateAllyHelpDefend(cell1, cell2);
		}
		
		public bool processKilling(Cell fightCell, FightCellResult fightCellResult)
		{
			bool gameOver = false;
			
			if (fightCellResult == FightCellResult.KILLED_DEFENDER)
			{
				if (fightCell.piece.relation == Relation.SELF && (fightCell.piece.type == TypePiece.KING || fightCell.piece.type == TypePiece.KING_HORSE)) // check gameover
					gameOver = true;

				destroyPiece (fightCell.piece);
				fightCell.piece = fightCell.attackerPiece;
			} 
			else if (fightCellResult == FightCellResult.KILLED_ATTACKER)
			{
				if (fightCell.attackerPiece.relation == Relation.SELF && (fightCell.attackerPiece.type == TypePiece.KING || fightCell.attackerPiece.type == TypePiece.KING_HORSE)) // check gameover
					gameOver = true;
				
				destroyPiece (fightCell.attackerPiece);
			}
			
			fightCell.attackerPiece = null;
			fightCell.hasFight = false;
			
			return gameOver;
		}

		public void createFightAtCell(Cell attacker, Cell defender)
		{
			defender.attackerPiece = attacker.piece;
			defender.attackerPiece.pos = defender.pos;
			defender.hasFight = true;
			attacker.piece = null;
		}
		
		public void destroyPiece(BasePiece piece)
		{
			piece.gameObject.transform.SetParent (null);
			Destroy (piece.gameObject);
		}

		public void highlightMoves(BasePiece piece)
		{
			for (int i=0; i < piece.moves.Count; i++) {
				Vector2 p = piece.pos+piece.moves[i];
				
				if (p.x < 0 || p.y < 0) continue;
				if (!canMoveToCellAt(p, piece)) continue;
				
				Cell cell = getCellAt(p);
				
				if(!cell) continue;
				if (cell.piece && !cell.piece.isInteractableWith(piece) && cell.piece.type != TypePiece.BUILDING_HOME) continue; // can move only to empty cell or home
				
				_createHighLightAt(p, piece.pos);
			}

			if (piece.interactiveMoves != null)
			{
				for (int i=0; i<piece.interactiveMoves.Count; i++) {
					Vector2 p = piece.pos + piece.interactiveMoves[i].move;
					if (p.x < 0 || p.y < 0 || !getCellAt(p) || !canMoveToCellAt(p, piece, true) ) continue;
					_createHighLightAt(p, piece.pos);
				}
			}
			
			if (piece.movesAttack != null)
			{
				for (int i=0; i<piece.movesAttack.Count; i++) 
				{
					Vector2 p = piece.pos + piece.movesAttack[i];
					
					if (p.x < 0 || p.y < 0) continue;
					if (!canMoveToCellAt(p, piece)) continue;
				
					Cell cell = getCellAt(p);
				
					if(!cell) continue;

					BasePiece pieceTo = cell.piece;
					if (!pieceTo) continue; // can attack to not empty cell
					if (pieceTo.relation != Relation.ENEMY && !cell.hasFight) continue; // can attack only enemy or fightcell

					_createHighLightAt(p, piece.pos);
				}
			}
		
			_createHighLightAt(piece.pos, piece.pos, true);
		}

		private void _createHighLightAt(Vector2 pos, Vector2 posFrom, bool isCurrentPos = false)
		{
			HighlightPiece component = createHighLight(pos);
			
			component.pos = pos;
			component.zIndex = ZIndex.PIECES_HIGHLIGHT;
			component.animateScale(0, 1.0f);
			
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

		public void clearMovingUI()
		{
			removeHighlightMoves();
			_currentArrow.visible = false;
		}
		public void removeHighlightMoves()
		{
			for (int i=0; i<_piecesHighlighted.Count; i++) 
			{
				_piecesHighlighted[i].gameObject.transform.SetParent(null);
				Destroy(_piecesHighlighted[i].gameObject);
			}
			
			if (_currentPosPieceHighligh) 
			{
				_currentPosPieceHighligh.gameObject.transform.SetParent (null);
				Destroy (_currentPosPieceHighligh.gameObject);
			}

			_piecesHighlighted = new List<HighlightPiece> ();
			_currentPosPieceHighligh = null;
		}

		public Vector3 getLocal3Position(Vector2 pos)
		{
			return new Vector3(pos.x * Game.POS_TO_COORDS , pos.y * Game.POS_TO_COORDS, 0);
		}
		
		// lighing next cell that we can put in a piece
		public void recalculateDraggingAction(Vector3 pos)
		{
			// find highlighet piece by pos
			bool foundNextCell = false;
			for (int i = 0; i < _piecesHighlighted.Count; i++)
			{
				Vector2 nextBoardPos = new Vector2(Mathf.Round(pos.x/Game.CELL_SIZE), Mathf.Round(pos.y/Game.CELL_SIZE));
				HighlightPiece pieceComponent = _piecesHighlighted[i];
				
				if (pieceComponent.pos.x == nextBoardPos.x && pieceComponent.pos.y == nextBoardPos.y)
				{
					foundNextCell = true;
					Cell prevCell = Game.gameController.getNextAction().cellFrom;
					Cell nextCell = getCellAt(nextBoardPos);
					
					if(nextCell.piece && (nextCell.piece.relation == Relation.ENEMY || nextCell.hasFight))
					{
						if (nextCell.hasFight)
						{
							if (nextCell.piece.relation == Relation.ENEMY && nextCell.attackerPiece.relation == Relation.SELF)
							{
								pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
								Game.gameController.updateNextActionCell(nextCell, BoardAction.ATTACK_HELP);
							}
							else if (nextCell.piece.relation == Relation.SELF && nextCell.attackerPiece.relation == Relation.ENEMY)
							{
								pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
								Game.gameController.updateNextActionCell(nextCell, BoardAction.DEFEND_HELP);
							}
							
						}
						else if(nextCell.piece.relation == Relation.ENEMY)
						{
							pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
							Game.gameController.updateNextActionCell(nextCell, BoardAction.ATTACK);
						}
					} 
					else
					{
						pieceComponent.sprite = BasePiece.H_CELL_MOVE_TO;
						InteractionType interactionType = PieceInteraction.getType(prevCell, nextCell);
						
						if (interactionType == InteractionType.NONE)
						{
							Game.gameController.updateNextActionCell(nextCell, BoardAction.MOVE);
						}
						else if (interactionType == InteractionType.END_LEVEL || interactionType == InteractionType.END_LEVEL_FROM_HORSE)
						{
							Game.gameController.updateNextActionCell(nextCell, BoardAction.END_LEVEL);
						}
						else
						{
							Game.gameController.updateNextActionCell(nextCell, BoardAction.INTERACTION);
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
				Game.gameController.updateNextActionCell(null, BoardAction.MOVE);
			}

		}

		public bool canMoveToCellAt(Vector2 pos, BasePiece piece, bool isInteraction = false){

			Cell cell = getCellAt (pos);
			
			if(!cell) return false;
			
			if (!piece.canJump)
			{
				if (!isFreeCellsToMove(piece.pos, pos)) return false;
			}

			if (!cell.piece)
			{
				return true;
			}
			
			if (cell.piece.relation == Relation.ENEMY && isInteraction)
				return false;
			
			if (cell.piece.isInteractableWith(piece))
			{
				return true;
			}

			if (cell.piece.relation == Relation.ENEMY)
			{
				return true;
			}
			
			if (cell.hasFight && (cell.piece.relation == Relation.ENEMY || cell.attackerPiece.relation == Relation.ENEMY))
			{
				return true;
			}

			return false;
		}

		public bool isFreeCellsToMove(Vector2 startPos, Vector2 endPos)
		{
			int xDir = Math.Sign((endPos - startPos).x);
			int yDir = Math.Sign((endPos - startPos).y);

			Vector2 curPos = startPos;
			for (int i = 0; i < 5; i++)
			{
				curPos += new Vector2(xDir, yDir);
				if (curPos == endPos) break;
				
				Cell cell = getCellAt(curPos);
				if (!cell) return false;
				if (cell.piece) return false;
			}

			return true;
		}

		public Cell getCellAt(Vector2 pos){
			return cells[(int)pos.y,(int)pos.x] as Cell;
		}

		private void _removePieces(List<BasePiece> list){
			for (int i=0; i<list.Count; i++) {
				list[i].Destructor();
				list[i].gameObject.transform.SetParent(null);
				Destroy(list[i].gameObject);
			}
		}

		private void _removePiecesAtCell(Cell cell)
		{
			if (cell.piece)
			{
				cell.piece.Destructor();
				Destroy(cell.piece.gameObject);
			}
			if (cell.attackerPiece)
			{
				cell.attackerPiece.Destructor();
				Destroy(cell.attackerPiece.gameObject);
			}
		}
		
		public void clearBoard()
		{
			removeHighlightMoves ();

			for (int i=0; i<H; i++) {
				for (int j=0; j<W; j++) {
					if(cells[i,j]){
						_removePiecesAtCell(cells[i,j]);
						cells[i,j].Destructor();
						cells[i,j].gameObject.transform.SetParent(null);
						Destroy(cells[i,j].gameObject);
						cells[i,j] = null;
					}
				}
			}

			cells = new Cell[H, W];
		}
	}
}