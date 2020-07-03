using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Rook : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			Moves = new List<Vector2>()
			{
				new Vector2(0, 1),
				new Vector2(0, 2),
				new Vector2(0, 3),
				new Vector2(0, 4),
				new Vector2(0, -1),
				new Vector2(0, -2),
				new Vector2(0, -3),
				new Vector2(0, -4),
				new Vector2(1, 0),
				new Vector2(2, 0),
				new Vector2(3, 0),
				new Vector2(4, 0),
				new Vector2(-1, 0),
				new Vector2(-2, 0),
				new Vector2(-3, 0),
				new Vector2(-4, 0),
			};

			MovesAttack = Moves;

			Type = TypePiece.ROOK;

			base.Setup(pos);

			CreateStats();
			Stats.attack = 3;
			Stats.health = 2;
		}

	}
}
