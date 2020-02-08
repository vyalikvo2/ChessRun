using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KingHorse : BasePiece
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

		type = TypePiece.KING_HORSE;
		
		base.Setup (pos);
	}
	
}
