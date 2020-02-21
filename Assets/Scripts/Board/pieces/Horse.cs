using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Horse : BasePiece
{
	public override void Setup(Vector2 pos)
	{
		moves = new List<Vector2> () {
			new Vector2(-1,-2), // bottom left
			new Vector2(-1, 2), // top left
			new Vector2( 2,-1), // bottom right
			new Vector2( 2, 1), // top right
			new Vector2( -2, 1), // top
			new Vector2( -2,-1), // bottom
			new Vector2( 1, -2), // left
			new Vector2( 1, 2)  // right
		};
		
		movesAttack = moves;

		type = TypePiece.HORSE;
		
		base.Setup (pos);
		
		createStats();
		stats.health = 2;
		stats.attack = 2;
	}
	
	public override bool isInteractableWith(BasePiece piece)
	{
		return piece.type == TypePiece.KING;
	}

}
