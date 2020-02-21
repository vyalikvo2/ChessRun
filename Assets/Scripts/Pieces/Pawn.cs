using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pawn : BasePiece
{
	public override void Setup(Vector2 pos)
	{	
		moves = new List<Vector2> () {
			new Vector2(1, 	0), // top
		};

		movesAttack = new List<Vector2> () {
			new Vector2(-1, 1), // top left
			new Vector2(1, 1) // top right
		};
		
		type = TypePiece.PAWN;

		base.Setup (pos);
		
		createStats();
		stats.attack = 1;
		stats.health = 2;
	}
	
}
