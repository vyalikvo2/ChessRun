using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rook : BasePiece
{
	public override void Setup(Vector2 pos)
	{	
		moves = new List<Vector2> () {
			new Vector2(0, 	1), 
			new Vector2(0, 	2), 
			new Vector2(0, 	3), 
			new Vector2(0, 	4), 
			new Vector2(0, 	-1), 
			new Vector2(0, 	-2), 
			new Vector2(0, 	-3), 
			new Vector2(0, 	-4), 
			new Vector2(1, 	0), 
			new Vector2(2, 	0), 
			new Vector2(3, 	0), 
			new Vector2(4, 	0), 
			new Vector2(-1, 	0), 
			new Vector2(-2, 	0), 
			new Vector2(-3, 	0), 
			new Vector2(-4, 	0),
		};
		
		movesAttack = moves;

		type = TypePiece.ROOK;

		base.Setup (pos);
		
		createStats();
		stats.attack = 3;
		stats.health = 2;
	}
	
}
