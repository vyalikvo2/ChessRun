using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

// New
public enum CellState
{
    None,
    Friendly,
    Enemy,
    Free,
    OutOfBounds
}

public class TurnData {
	public BasePiece killedPiece = null;
	public float damageDealed = 0;
}

public class Board : MonoBehaviour
{
	public static int W = 50;
	public static int H = 50;

	private Game game;

	public  Cell[,] cells = new Cell[H, W];

	[SerializeField]
	public GameObject cellPrefab;

	List<HighlightPiece> piecesHighlighted = new List<HighlightPiece>();
	HighlightPiece currentPosPieceHighligh;

	[HideInInspector] public List<BasePiece> myPieces = null;
	[HideInInspector] public List<BasePiece> enemyPieces = null;
	
	private Dictionary<char, GameObject> piecesPrefabs;

	private ScalableArrow currentArrow;

	public void Setup()
	{
		game = GetComponent<Game> () as Game;

		piecesPrefabs = new Dictionary<char, GameObject> ();
		piecesPrefabs.Add (TypePiece.KING, PrefabsList.king);
		piecesPrefabs.Add (TypePiece.PAWN, PrefabsList.pawn);
		piecesPrefabs.Add (TypePiece.BUILDING_HOME, PrefabsList.home);
		piecesPrefabs.Add (TypePiece.HORSE, PrefabsList.horse);
		piecesPrefabs.Add (TypePiece.KING_HORSE, PrefabsList.king_horse);

		Game.gameController.onNextActionChanged += onNextActionChanged;
	}
	

    public void CreateSolid(int w, int h)
    {
		for (int i=0; i<h; i++) {
			for( var j=0;j<w;j++){
				GameObject obj = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
				Cell cell = obj.GetComponent<Cell>();
				cell.Setup(new Vector2(j, i));
				cells[i,j] = cell;
				obj.transform.SetParent(this.transform);
			}
		}
	}


	public void CreateByMap(string[] map)
	{
		int maxY = map.Length;
		for (int i=0; i<maxY; i++) {
			for( var j=0;j<map[i].Length;j++){
				if(map[i][j]=='#') continue;
				GameObject obj = Instantiate(cellPrefab, gameObject.transform) as GameObject;
				Cell cell = obj.GetComponent<Cell>();
			
				int i2 = maxY - i - 1;
				cell.Setup(new Vector2(j,i2));

				cells[i2,j] = cell;

				createPieceFromMap(map[i][j], new Vector2(j,i2));

				if(map[i][j]=='K')
				{
					game.centerCamera(cell.transform.position, false);
				}
			}
		}
	}

	private void createPieceFromMap(char s, Vector2 pos){

		if (!piecesPrefabs.ContainsKey(s))
			return;

		GameObject prefab = piecesPrefabs [s];
		GameObject obj = Instantiate (prefab, gameObject.transform, true);
		BasePiece basePiece = obj.GetComponent<BasePiece> () as BasePiece;

		basePiece.relation = getRelationFromChar (s);

		basePiece.Setup(pos);
		addPiece(basePiece);

	}

	public BasePiece createPieceByType(char s){

		if (!piecesPrefabs.ContainsKey(s))
			return null;

		GameObject prefab = piecesPrefabs [s];
		GameObject obj = game.engine.Instance (prefab);
		BasePiece basePiece = obj.GetComponent<BasePiece> () as BasePiece;
		basePiece.Setup(new Vector2(0,0));

		return basePiece;
	}

	int getRelationFromChar(char s){
		if (s == 'K' || s == 'H')
			return Relation.SELF;
		if(s == 'E')
			return Relation.BUILDING;

		return Relation.ENEMY;
	}

	public void addPiece(BasePiece piece)
	{
		cells [(int)piece.pos.y, (int)piece.pos.x].piece = piece;
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
			GameObject arrowPrefab = Resources.Load("prefabs/ScalableArrow") as GameObject;
			GameObject arrow = Instantiate(arrowPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			arrow.transform.SetParent(transform);
			currentArrow = arrow.GetComponent<ScalableArrow>();
			currentArrow.Setup();
		}

		currentArrow.visible = true;
		currentArrow.transform.localPosition = new Vector3(cell.piece.transform.localPosition.x, cell.piece.transform.localPosition.y, 0);
		
		if(!currentPosPieceHighligh) highlightMoves(cell.piece);
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
		piece.pos = cell2.pos;
		
		cell2.piece = cell1.piece;
		cell1.piece = null;
	}

	public void attackPiece(Cell cell1, Cell cell2)
	{
		killEnemy (cell2.piece);
		cell1.piece.pos = cell2.pos;
		cell2.piece = cell1.piece;
		cell1.piece = null;
	}
	
	public void killEnemy(BasePiece piece)
	{
		piece.gameObject.transform.SetParent (null);
		Destroy (piece.gameObject);
	}

	public void highlightMoves(BasePiece piece)
	{
		for (int i=0; i<piece.moves.Count; i++) {
			Vector2 p = piece.pos+piece.moves[i];
			if (p.x < 0 || p.y < 0 || !getCellAt(p) || !canMoveToCellAt(p, piece) ) continue;
			createHighLightAt(p, piece.pos);
		}

		if (piece.interactiveMoves != null)
		{
			for (int i=0; i<piece.interactiveMoves.Count; i++) {
				Vector2 p = piece.pos+piece.interactiveMoves[i].move;
				if (p.x < 0 || p.y < 0 || !getCellAt(p) || !canMoveToCellAt(p, piece) ) continue;
				createHighLightAt(p, piece.pos);
			}
		}
	
		createHighLightAt(piece.pos, piece.pos, true);
	}

	private void createHighLightAt(Vector2 pos, Vector2 posFrom, bool isCurrentPos = false)
	{
		GameObject highlight = game.engine.Instance (PrefabsList.cell_highlighted);
		HighlightPiece component = highlight.GetComponent<HighlightPiece> () as HighlightPiece;
		component.zIndex = ZIndex.PIECES_HIGHLIGHT;

		component.animateScale(0, 1.0f);
		component.setBoardPosition(posFrom);
		component.pos = pos;

		if (!isCurrentPos) {
			piecesHighlighted.Add (component);
		} else {
			component.currentSprite = component.cell_current;
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
				if(nextCell.piece && nextCell.piece.relation == Relation.ENEMY){
					pieceComponent.currentSprite = pieceComponent.cell_attack;
					Game.gameController.updateNextActionCell(nextCell, GameAction.ATTACK);
				} 
				else
				{
					pieceComponent.currentSprite = pieceComponent.cell_next;
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
				
			} else {
				pieceComponent.currentSprite = pieceComponent.cell_highlighted;
			}
		}

		if (!foundNextCell)
		{
			Game.gameController.updateNextActionCell(null, GameAction.MOVE);
		}

	}

	public bool canMoveToCellAt(Vector2 pos, BasePiece piece){

		Cell cell = getCellAt (pos);

		if (!cell.piece)
			return true;
		
		if (cell.piece.isInteractableWith(piece))
		{
			return true;
		}

		if (cell.piece.relation == Relation.ENEMY)
		{
			return true;
		}

		return false;
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

	public void clearBoard()
	{
		removeHighlightMoves ();
		removePieces (myPieces);
		removePieces (enemyPieces);
		myPieces = new List<BasePiece> ();
		enemyPieces = new List<BasePiece> ();

		for (int i=0; i<H; i++) {
			for (int j=0; j<W; j++) {
				if(cells[i,j]){
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
