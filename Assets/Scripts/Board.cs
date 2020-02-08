using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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

	List<GameObject> piecesHighlighted = new List<GameObject>();
	GameObject currentPosPieceHighligh;

	[HideInInspector] public List<BasePiece> myPieces = null;
	[HideInInspector] public List<BasePiece> enemyPieces = null;

	public BasePiece currentPiece;

	private Dictionary<char, GameObject> piecesPrefabs;
	
	public void Setup()
	{
		game = GetComponent<Game> () as Game;

		piecesPrefabs = new Dictionary<char, GameObject> ();
		piecesPrefabs.Add (TypePiece.KING, PrefabsList.king);
		piecesPrefabs.Add (TypePiece.PAWN, PrefabsList.pawn);
		piecesPrefabs.Add (TypePiece.BUILDING_HOME, PrefabsList.home);
		piecesPrefabs.Add (TypePiece.HORSE, PrefabsList.horse);
		piecesPrefabs.Add (TypePiece.KING_HORSE, PrefabsList.king_horse);
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
				GameObject obj = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
				Cell cell = obj.GetComponent<Cell>();
			
				int i2 = maxY - i - 1;
				cell.Setup(new Vector2(j,i2));

				cells[i2,j] = cell;
				obj.transform.SetParent(this.transform);

				createPieceFromMap(map[i][j], new Vector2(j,i2));

				if(map[i][j]=='K')
				{
					game.centerCamera(cell.piece.transform.position);
				}
			}
		}
	}

	private void createPieceFromMap(char s, Vector2 pos){

		if (!piecesPrefabs.ContainsKey(s))
			return;

		GameObject prefab = piecesPrefabs [s];
		GameObject obj = game.engine.Instance (prefab);
		BasePiece basePiece = obj.GetComponent<BasePiece> () as BasePiece;

		basePiece.relation = getRelationFromChar (s);
		if (basePiece.relation == Relation.SELF) {
			PieceDrag dragComponent = obj.AddComponent<PieceDrag> () as PieceDrag;
			dragComponent.piece = basePiece;
			basePiece.relation = Relation.SELF;
		}

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

	public void setDraggingPiece(BasePiece piece)
	{
		currentPiece = piece;
		if(currentPiece !=null)
		{
			highlightMoves(piece);
		} else {
			removeHighlightMoves();
		}
	}

	public TurnData movePieceTo(Vector2 pos, BasePiece piece)
	{
		TurnData turn = new TurnData();

		Cell cell1 = cells [(int)piece.pos.y, (int)piece.pos.x];
		Cell cell2 = cells [(int)pos.y, (int)pos.x];

		if (cell2 == null) {
			return turn;
		}

		game.centerCamera(cell2.transform.position);

		bool interaction = false;

		if (cell2.piece) 
		{
			if (cell2.piece.relation == Relation.ENEMY) 
			{
				turn.killedPiece = cell2.piece;
				killEnemy (cell2.piece);

				Debug.Log ("KILL ENEMY");
			} 
			else if(cell2.piece.isInteractableWith(piece))
			{
				PieceInteraction.interact(cell1, cell2, piece, cell2.piece, pos);	
				interaction = true;
			} 
			else if(cell2.piece.relation == Relation.BUILDING)
			{
				if(cell2.piece.GetComponent<Home>() != null)
				{
					Game.gameStateController.levelComplete();
				}
			}
		} 
			
		if(!interaction) 
		{
			piece.pos = pos;
			cell2.piece = cell1.piece;
			cell1.piece = null;
		}

		return turn;
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
			if (p.x < 0 || p.y<0 || !getCellAt(p) || !canMoveToCellAt(p) ) continue;
			createHighLightAt(p);
		}

		createHighLightAt(piece.pos, true);
	}

	private void createHighLightAt(Vector2 pos, bool isCurrentPos = false)
	{
		GameObject highlight = game.engine.Instance (PrefabsList.cell_highlighted);
		HighlightPiece component = highlight.GetComponent<HighlightPiece> () as HighlightPiece;
		component.zIndex = ZIndex.PIECES_HIGHLIGHT;
		component.pos = pos;

		if (!isCurrentPos) {
			piecesHighlighted.Add (highlight);
		} else {
			component.currentSprite = component.cell_current;
			currentPosPieceHighligh = highlight;
		}
	}

	public void removeHighlightMoves()
	{
		for (int i=0; i<piecesHighlighted.Count; i++) {
			piecesHighlighted[i].transform.SetParent(null);
			Destroy(piecesHighlighted[i]);
		}

		if (currentPosPieceHighligh) {
			currentPosPieceHighligh.transform.SetParent (null);
			Destroy (currentPosPieceHighligh);
		}

		piecesHighlighted = new List<GameObject> ();
		currentPosPieceHighligh = null;
	}

	// drop piece to next cell or move to initial position
	public void dragAndDropIfCan(BasePiece piece)
	{
		Vector3 worldPos = piece.transform.position;
		bool foundMove = false;
		Vector2 moveToPos = new Vector2(0,0);
		for (int i=0; i<piece.moves.Count; i++) {
			Vector2 p = piece.pos+piece.moves[i];
			if (p.x < 0 || p.y<0 || !getCellAt(p)) continue;
			if(Mathf.Round(worldPos.x*Game.COORDS_TO_POS) == p.x && Mathf.Round(worldPos.y*Game.COORDS_TO_POS)==p.y && canMoveToCellAt(p))
			{
				foundMove = true;
				moveToPos = p;
				break;
			}
		}

		if (foundMove) {
			movePieceTo(moveToPos, piece);
		} else {
			piece.RefreshPos ();
		}
	}

	// lighing next cell that we can put in a piece
	public void updateHighlightMoveTo(BasePiece piece)
	{
		Vector3 worldPos = piece.transform.position;

		// find highlighet piece by pos
		for (int i=0; i<piecesHighlighted.Count; i++) {
			HighlightPiece pieceComponent = (piecesHighlighted [i].GetComponent<HighlightPiece>() as HighlightPiece);
			if (pieceComponent.pos.x == Mathf.Round(worldPos.x*Game.COORDS_TO_POS) && 
			    pieceComponent.pos.y == Mathf.Round(worldPos.y*Game.COORDS_TO_POS)) {

				if(getCellAt(pieceComponent.pos).piece && getCellAt(pieceComponent.pos).piece.relation == Relation.ENEMY){
					pieceComponent.currentSprite = pieceComponent.cell_attack;
				} else {
					pieceComponent.currentSprite = pieceComponent.cell_next;
				}

			} else {
				pieceComponent.currentSprite = pieceComponent.cell_highlighted;
			}
		}

	}

	public bool canMoveToCellAt(Vector2 pos){

		Cell cell = getCellAt (pos);

		if(!cell.piece)
			return true;

		if (cell.piece.relation == Relation.SELF && !cell.piece.isInteractableWith(currentPiece))
			return false;

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
