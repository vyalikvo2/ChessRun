using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Bishop : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			Moves = new List<Vector2>()
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
			MovesAttack = Moves;

			Type = TypePiece.BISHOP;

			base.Setup(pos);

			CreateStats();
			Stats.attack = 2;
			Stats.health = 3;
		}

	}
}
