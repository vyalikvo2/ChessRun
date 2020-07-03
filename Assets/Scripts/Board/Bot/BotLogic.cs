using System.Collections.Generic;
using UnityEngine;

using ChessRun.Board;
using ChessRun.Board.Pieces;

namespace ChessRun.Board.Bot
{

    public enum BotMoveType
    {
        MOVE,
        DEFEND_ATTACK,
        ATTACK,
        ATTACK_HELP,
        FIGHT_CELL_CONTINUE,
    }

    class BotMove
    {
        public Cell cellFrom;
        public Cell cellTo;
        public BotMoveType type = BotMoveType.MOVE;
        public int score = 0;

        public BotMove(Cell cellFrom, Cell cellTo, BotMoveType type, int score = 0)
        {
            this.cellFrom = cellFrom;
            this.cellTo = cellTo;
            this.type = type;
            this.score = score;
        }
    }

    public class BotLogic : MonoBehaviour
    {
        private ChessBoard _board;
        private GameEngine _gameEngine;
        private List<BotMove> _moveList = new List<BotMove>();

        void Start()
        {
            _board = GetComponent<ChessBoard>();
            _gameEngine = GetComponent<GameEngine>();
        }

        public void MakeBotMove()
        {
            _moveList.Clear();

            for (int i = 0; i < ChessBoard.H; i++)
            {
                for (int j = 0; j < ChessBoard.W; j++)
                {
                    Cell cell = _board.Cells[i, j];
                    if (!cell) continue;
                    if (!cell.Piece) continue;

                    BasePiece piece = cell.Piece;
                    if (piece.Relation != PieceRelation.ENEMY && !cell.HasFight) continue;

                    if (cell.HasFight)
                    {
                        BotMove cellFightMove = new BotMove(cell, null, BotMoveType.FIGHT_CELL_CONTINUE, 2);
                        _moveList.Add(cellFightMove);

                        continue; // bot at fight cell cant make other moves
                    }

                    for (int k = 0; k < piece.MovesAttack.Count; k++)
                    {
                        Vector2 attackDir = piece.MovesAttack[k];
                        attackDir.y = -attackDir.y; // ENEMY ATTACK DIRECTION
                        Vector2 attackPos = piece.pos + attackDir;
                        if (attackPos.x < 0 || attackPos.y < 0 || attackPos.x >= ChessBoard.W ||
                            attackPos.y > ChessBoard.H) continue;
                        Cell attackCell = _board.GetCellAt(attackPos);

                        if (!cell.Piece.CanJump)
                        {
                            if (!_gameEngine.Board.IsFreeCellsToMove(cell.Pos, attackPos)) continue;
                        }

                        if (!attackCell) continue;

                        if (attackCell.HasFight)
                        {
                            if (attackCell.Piece.Relation == PieceRelation.ENEMY)
                            {
                                BotMove defendAttackMove = new BotMove(cell, attackCell, BotMoveType.DEFEND_ATTACK, 5);
                                _moveList.Add(defendAttackMove);
                            }
                            else
                            {
                                BotMove attackHelpMove = new BotMove(cell, attackCell, BotMoveType.ATTACK_HELP, 4);
                                _moveList.Add(attackHelpMove);
                            }

                        }
                        else if (cell.Piece.Relation == PieceRelation.ENEMY && attackCell.Piece &&
                                 attackCell.Piece.Relation == PieceRelation.SELF)
                        {
                            int score = 3;
                            if (attackCell.Piece.Type == TypePiece.KING ||
                                attackCell.Piece.Type == TypePiece.KING_HORSE)
                            {
                                score = 10 + piece.Stats.attack;
                            }

                            BotMove attackMove = new BotMove(cell, attackCell, BotMoveType.ATTACK, score);
                            _moveList.Add(attackMove);
                        }
                    }
                }
            }

            if (_moveList.Count > 0)
            {
                _moveList.Sort(delegate(BotMove x, BotMove y)
                {
                    if (x.score > y.score) return -1;
                    else if (x.score <= y.score) return 1;
                    else return 0;
                });

                BotMove nextMove = _moveList[0];
                _processBotMove(nextMove);

            }
            else
            {
                GameEngine.GameController.EndMove();
            }
        }

        private void _processBotMove(BotMove move)
        {
            GameEngine.GameController.BeginBotMove();
            Debug.Log("Process bot move " + move.type);
            if (move.type == BotMoveType.DEFEND_ATTACK)
            {
                _board.Fight.AnimateAllyHelpDefend(move.cellFrom, move.cellTo);
            }
            else if (move.type == BotMoveType.ATTACK_HELP)
            {
                Debug.Log(move.cellFrom.Pos + " -> " + move.cellTo.Pos);
                _board.Fight.AnimateAllyHelpAttack(move.cellFrom, move.cellTo);
            }
            else if (move.type == BotMoveType.ATTACK)
            {
                Debug.Log(move.cellFrom.Pos + " -> " + move.cellTo.Pos);
                _board.Fight.BeginAttack(move.cellFrom, move.cellTo);
            }
            else if (move.type == BotMoveType.FIGHT_CELL_CONTINUE)
            {
                Debug.Log(move.cellFrom.Pos + " continue fight");
                if (move.cellFrom.Piece.Relation == PieceRelation.ENEMY)
                {
                    _board.Fight.AnimateFightCellDefenderAttack(move.cellFrom);
                }
                else
                {
                    _board.Fight.AnimateFightCellAttackerAttack(move.cellFrom);
                }

            }

        }
    }
}

