using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Pawn : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			moves = new List<Vector2>()
			{
				new Vector2(0, 1), // top
			};

			movesAttack = new List<Vector2>()
			{
				new Vector2(-1, 1), // top left
				new Vector2(1, 1) // top right
			};

			type = TypePiece.PAWN_ENEMY;

			base.Setup(pos);

			createStats();
			stats.attack = 1;
			stats.health = 2;
		}
	}
}
