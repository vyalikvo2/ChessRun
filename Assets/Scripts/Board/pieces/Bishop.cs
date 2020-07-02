using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Bishop : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			moves = new List<Vector2>()
			{
				new Vector2(1, 1),
				new Vector2(2, 2),
				new Vector2(3, 3),
				new Vector2(4, 4),
				new Vector2(1, -1),
				new Vector2(2, -2),
				new Vector2(3, -3),
				new Vector2(4, -4),
				new Vector2(-1, 1),
				new Vector2(-2, 2),
				new Vector2(-3, 3),
				new Vector2(-4, 4),
				new Vector2(-1, -1),
				new Vector2(-2, -2),
				new Vector2(-3, -3),
				new Vector2(-4, -4)
			};
			movesAttack = moves;

			type = TypePiece.BISHOP;

			base.Setup(pos);

			createStats();
			stats.attack = 2;
			stats.health = 3;
		}

	}
}
