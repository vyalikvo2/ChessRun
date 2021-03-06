﻿using UnityEngine;

using System.Collections.Generic;

namespace ChessRun.Board.Pieces
{
	public class King : BasePiece
	{
		public override void Setup(Vector2 pos)
		{
			Moves = new List<Vector2>()
			{
				new Vector2(-1, -1), // bottom left
				new Vector2(-1, 1), // top left
				new Vector2(1, -1), // bottom right
				new Vector2(1, 1), // top right
				new Vector2(0, 1), // top
				new Vector2(0, -1), // bottom
				new Vector2(-1, 0), // left
				new Vector2(1, 0) // right
			};

			MovesAttack = Moves;

			Type = TypePiece.KING;

			base.Setup(pos);

			CreateStats();
			Stats.health = 2;
			Stats.attack = 1;
		}

	}
}
