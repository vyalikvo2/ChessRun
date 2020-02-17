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

		/*GameObject go = Instantiate (PrefabsList.UI_Enemy_Healthbar, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		HealthBar hp = go.GetComponent<HealthBar> () as HealthBar;
		hp.Setup (66, 7, 30, 100);
		go.transform.SetParent (transform);*/

		type = TypePiece.PAWN;

		base.Setup (pos);
		
		createStats();
		stats.attack = 1;
		stats.health = 2;
	}
	
}
