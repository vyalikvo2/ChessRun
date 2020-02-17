using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class King : BasePiece
{
	public override void Setup(Vector2 pos)
	{
		moves = new List<Vector2> () {
			new Vector2(-1,-1), // bottom left
			new Vector2(-1, 1), // top left
			new Vector2( 1,-1), // bottom right
			new Vector2( 1, 1), // top right
			new Vector2( 0, 1), // top
			new Vector2( 0,-1), // bottom
			new Vector2(-1, 0), // left
			new Vector2( 1, 0)  // right
		};

		movesAttack = moves;

		type = TypePiece.KING;

		base.Setup (pos);
		
		createStats();
		stats.health = 2;
		stats.attack = 1;
	}

}
