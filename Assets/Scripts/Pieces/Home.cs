using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Home : BasePiece
{
	public override void Setup(Vector2 pos)
	{	
		moves = new List<Vector2> () {

		};
		
		movesAttack = new List<Vector2> () {
		};

		base.Setup (pos);
	}
	
}
