using UnityEngine;
using System.Collections;

public class PieceInteraction : MonoBehaviour {

	public static Game game;

	public static void interact(Cell c1, Cell c2, BasePiece p1, BasePiece p2, Vector2 pos)
	{
		if(p2.type == TypePiece.HORSE && p1.type==TypePiece.KING){
			BasePiece newPiece = game.board.createPieceByType(TypePiece.KING_HORSE);

			destroyPiece(c1.piece);
			destroyPiece(c2.piece);
			c1.piece = null;
			c2.piece = null;

			newPiece.pos = c2.pos;
			c2.piece = newPiece;

			PieceDrag dragComponent = newPiece.gameObject.AddComponent<PieceDrag> () as PieceDrag;
			dragComponent.piece = newPiece;
			newPiece.relation = Relation.SELF;

			Debug.Log("INTERACT ");
		}
	}

	public static void destroyPiece(BasePiece p){
		p.gameObject.transform.SetParent(null);
		p.Destructor();
		Destroy(p.gameObject);
	}
}
