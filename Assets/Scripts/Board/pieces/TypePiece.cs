using UnityEngine;

namespace ChessRun.Board.Pieces
{
	public class TypePiece : MonoBehaviour
	{
		public const char NONE = '`'; // not set
		public const char EMPTY = 'O'; // empty cell

		public const char KING = 'K';
		public const char PAWN = 'P';
		public const char HORSE = 'H';
		public const char BISHOP = 'B';
		public const char ROOK = 'R';
		public const char QUEEN = 'Q';
		public const char KING_HORSE = 'M';

		public const char PAWN_ENEMY = '1';
		public const char HORSE_ENEMY = '2';
		public const char BISHOP_ENEMY = '3';
		public const char ROOK_ENEMY = '4';
		public const char QUEEN_ENEMY = '5';
		public const char KING_ENEMY = '$';

		public const char BUILDING_HOME = 'E';
	}
}
