using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Home : BasePiece
{
	public override void Setup(Vector2 pos)
	{
		type = TypePiece.BUILDING_HOME;
		
		moves = new List<Vector2> () {

		};
		
		movesAttack = new List<Vector2> () {
		};

		base.Setup (pos);

		stats.attack = 0;
		stats.health = 0;
		stats.visible = false;
	}
	
	public override bool isInteractableWith(BasePiece piece)
	{
		return piece.type == TypePiece.KING_HORSE || piece.type == TypePiece.KING;
	}
	
}
