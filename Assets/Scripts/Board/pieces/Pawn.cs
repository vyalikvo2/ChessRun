using UnityEngine;
using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class Pawn : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			Moves = new List<Vector2>()
			{
				new Vector2(0, 1), // top
			};

			MovesAttack = new List<Vector2>()
			{
				new Vector2(-1, 1), // top left
				new Vector2(1, 1) // top right
			};

			Type = TypePiece.PAWN_ENEMY;

			base.Setup(pos);

			CreateStats();
			Stats.attack = 1;
			Stats.health = 2;
		}
	}
}
