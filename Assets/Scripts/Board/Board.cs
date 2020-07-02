using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DG.Tweening;

public class Board : MonoBehaviour
{
	public static int W = 50;
	public static int H = 50;

	private Game game;

	public Cell[,] cells = new Cell[H, W];

	[SerializeField] public GameObject cellPrefab;

	List<HighlightPiece> piecesHighlighted = new List<HighlightPiece>();
	HighlightPiece currentPosPieceHighligh;

	private ScalableArrow currentArrow;
	[HideInInspector] public FightController fight;
	[HideInInspector] public BotLogic botLogic;

	public void Setup()
	{
		game = GetComponent<Game>() as Game;
		
		fight = GetComponent<FightController>();
		fight.Setup(this);

		botLogic = GetComponent<BotLogic>();

		Game.gameController.onNextActionChanged += onNextActionChanged;
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
					game.centerCamera(cell.transform.position, false);
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
		BasePiece basePiece = addPieceScriptByChar(obj, ch);
		basePiece.sprite = getSpriteNameFromChar(ch);
		basePiece.relation = getRelationFromChar(ch);
		
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

	private BasePiece addPieceScriptByChar(GameObject obj, char c)
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

	string getSpriteNameFromChar(char c)
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
	
	int getRelationFromChar(char ch)
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

	private void onNextActionChanged(GameAction action)
	{
		Debug.Log(action.name);
		switch (action.name)
		{
			case GameAction.MOVE:
				showMoveToAction(action.cellFrom);
				currentArrow.iconVisible = false;
				break;
			case GameAction.ATTACK:
				currentArrow.iconVisible = true;
				currentArrow.icon.sprite = InteractionIcon.ICON_ATTACK;
				break;
			case GameAction.INTERACTION:
				currentArrow.iconVisible = true;
				currentArrow.icon.sprite = InteractionIcon.ICON_INTERACTION;
				break;

		}
	}

	public void showMoveToAction(Cell cell)
	{
		if (!currentArrow)
		{
			GameObject arrowPrefab = ResourceCache.getPrefab("prefabs/ScalableArrow");
			GameObject arrow = Instantiate(arrowPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			arrow.transform.SetParent(transform);
			currentArrow = arrow.GetComponent<ScalableArrow>();
			currentArrow.Setup();
		}

		currentArrow.visible = true;
		currentArrow.transform.localPosition =
			new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, 0);

		if (!currentPosPieceHighligh) highlightMoves(cell.piece);
	}

	public void updateDraggingAction(Vector3 pos)
	{
		Cell cell = Game.gameController.nextAction.cellFrom;
		Vector3 pos1 = new Vector3(cell.pos.x * Game.CELL_SIZE, cell.pos.y * Game.CELL_SIZE, 0);
		currentArrow.width = (int) Vector3.Distance(pos1, pos);

		Vector3 direction = pos - pos1;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		currentArrow.setRotation(angle);

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
	
	public bool processKilling(Cell fightCell, int fightStatus)
	{
		Debug.Log("KILLING AT " + fightCell.pos);
		bool gameOver = false;
		
		if (fightStatus == FightController.KILLED_DEFENDER)
		{
			if (fightCell.piece.relation == Relation.SELF && (fightCell.piece.type == TypePiece.KING || fightCell.piece.type == TypePiece.KING_HORSE)) // check gameover
				gameOver = true;

			destroyPiece (fightCell.piece);
			fightCell.piece = fightCell.attackerPiece;
		} 
		else if (fightStatus == FightController.KILLED_ATTACKER)
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
			
			createHighLightAt(p, piece.pos);
		}

		if (piece.interactiveMoves != null)
		{
			for (int i=0; i<piece.interactiveMoves.Count; i++) {
				Vector2 p = piece.pos + piece.interactiveMoves[i].move;
				if (p.x < 0 || p.y < 0 || !getCellAt(p) || !canMoveToCellAt(p, piece, true) ) continue;
				createHighLightAt(p, piece.pos);
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

				createHighLightAt(p, piece.pos);
			}
		}
	
		createHighLightAt(piece.pos, piece.pos, true);
	}

	private void createHighLightAt(Vector2 pos, Vector2 posFrom, bool isCurrentPos = false)
	{
		Debug.Log("CRETE AT " + pos);

		HighlightPiece component = createHighLight(pos);
		
		component.pos = pos;
		component.zIndex = ZIndex.PIECES_HIGHLIGHT;
		component.animateScale(0, 1.0f);
		
		if (!isCurrentPos) {
			component.sprite = BasePiece.H_CELL_HIGHLIGHTED;
			piecesHighlighted.Add (component);
		} 
		else
		{
			component.sprite = BasePiece.H_CELL_CURRENT;
			currentPosPieceHighligh = component;
		}
	}

	public void clearMovingUI()
	{
		removeHighlightMoves();
		currentArrow.visible = false;
	}
	public void removeHighlightMoves()
	{
		for (int i=0; i<piecesHighlighted.Count; i++) 
		{
			piecesHighlighted[i].gameObject.transform.SetParent(null);
			Destroy(piecesHighlighted[i].gameObject);
		}
		
		if (currentPosPieceHighligh) 
		{
			currentPosPieceHighligh.gameObject.transform.SetParent (null);
			Destroy (currentPosPieceHighligh.gameObject);
		}

		piecesHighlighted = new List<HighlightPiece> ();
		currentPosPieceHighligh = null;
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
		for (int i = 0; i < piecesHighlighted.Count; i++)
		{
			Vector2 nextBoardPos = new Vector2(Mathf.Round(pos.x/Game.CELL_SIZE), Mathf.Round(pos.y/Game.CELL_SIZE));
			HighlightPiece pieceComponent = piecesHighlighted[i];
			
			if (pieceComponent.pos.x == nextBoardPos.x && pieceComponent.pos.y == nextBoardPos.y)
			{
				foundNextCell = true;
				Cell prevCell = Game.gameController.nextAction.cellFrom;
				Cell nextCell = getCellAt(nextBoardPos);
				
				if(nextCell.piece && (nextCell.piece.relation == Relation.ENEMY || nextCell.hasFight))
				{
					if (nextCell.hasFight)
					{
						if (nextCell.piece.relation == Relation.ENEMY && nextCell.attackerPiece.relation == Relation.SELF)
						{
							pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
							Game.gameController.updateNextActionCell(nextCell, GameAction.ATTACK_HELP);
						}
						else if (nextCell.piece.relation == Relation.SELF && nextCell.attackerPiece.relation == Relation.ENEMY)
						{
							pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
							Game.gameController.updateNextActionCell(nextCell, GameAction.DEFEND_HELP);
						}
						
					}
					else if(nextCell.piece.relation == Relation.ENEMY)
					{
						pieceComponent.sprite = BasePiece.H_CELL_ENEMY;
						Game.gameController.updateNextActionCell(nextCell, GameAction.ATTACK);
					}
				} 
				else
				{
					pieceComponent.sprite = BasePiece.H_CELL_MOVE_TO;
					string interactionType = PieceInteraction.getType(prevCell, nextCell);
					
					if (interactionType == PieceInteraction.NONE)
					{
						Game.gameController.updateNextActionCell(nextCell, GameAction.MOVE);
					}
					else if (interactionType == PieceInteraction.END_LEVEL || interactionType == PieceInteraction.END_LEVEL_FROM_HORSE)
					{
						Game.gameController.updateNextActionCell(nextCell, GameAction.END_LEVEL);
					}
					else
					{
						Game.gameController.updateNextActionCell(nextCell, GameAction.INTERACTION);
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
			Game.gameController.updateNextActionCell(null, GameAction.MOVE);
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

	private void removePieces(List<BasePiece> list){
		for (int i=0; i<list.Count; i++) {
			list[i].Destructor();
			list[i].gameObject.transform.SetParent(null);
			Destroy(list[i].gameObject);
		}
	}

	private void removePiecesAtCell(Cell cell)
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
					removePiecesAtCell(cells[i,j]);
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
