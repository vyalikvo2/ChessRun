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
        private Game _game;
        private List<BotMove> _moveList = new List<BotMove>();

        void Start()
        {
            _board = GetComponent<ChessBoard>();
            _game = GetComponent<Game>();
        }

        public void makeBotMove()
        {
            _moveList.Clear();

            for (int i = 0; i < ChessBoard.H; i++)
            {
                for (int j = 0; j < ChessBoard.W; j++)
                {
                    Cell cell = _board.cells[i, j];
                    if (!cell) continue;
                    if (!cell.piece) continue;

                    BasePiece piece = cell.piece;
                    if (piece.relation != Relation.ENEMY && !cell.hasFight) continue;

                    if (cell.hasFight)
                    {
                        Debug.Log("addFightMove");
                        BotMove cellFightMove = new BotMove(cell, null, BotMoveType.FIGHT_CELL_CONTINUE, 2);
                        _moveList.Add(cellFightMove);

                        continue; // bot at fight cell cant make other moves
                    }

                    for (int k = 0; k < piece.movesAttack.Count; k++)
                    {
                        Vector2 attackDir = piece.movesAttack[k];
                        attackDir.y = -attackDir.y; // ENEMY ATTACK DIRECTION
                        Vector2 attackPos = piece.pos + attackDir;
                        if (attackPos.x < 0 || attackPos.y < 0 || attackPos.x >= ChessBoard.W ||
                            attackPos.y > ChessBoard.H) continue;
                        Cell attackCell = _board.getCellAt(attackPos);

                        if (!cell.piece.canJump)
                        {
                            if (!_game.board.isFreeCellsToMove(cell.pos, attackPos)) continue;
                        }

                        if (!attackCell) continue;

                        if (attackCell.hasFight)
                        {
                            if (attackCell.piece.relation == Relation.ENEMY)
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
                        else if (cell.piece.relation == Relation.ENEMY && attackCell.piece &&
                                 attackCell.piece.relation == Relation.SELF)
                        {
                            int score = 3;
                            if (attackCell.piece.type == TypePiece.KING ||
                                attackCell.piece.type == TypePiece.KING_HORSE)
                            {
                                score = 10 + piece.stats.attack;
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
                processBotMove(nextMove);

            }
            else
            {
                Game.gameController.endMove();
            }
        }

        private void processBotMove(BotMove move)
        {
            Game.gameController.beginBotMove();
            Debug.Log("Process bot move " + move.type);
            if (move.type == BotMoveType.DEFEND_ATTACK)
            {
                _board.fight.animateAllyHelpDefend(move.cellFrom, move.cellTo);
            }
            else if (move.type == BotMoveType.ATTACK_HELP)
            {
                Debug.Log(move.cellFrom.pos + " -> " + move.cellTo.pos);
                _board.fight.animateAllyHelpAttack(move.cellFrom, move.cellTo);
            }
            else if (move.type == BotMoveType.ATTACK)
            {
                Debug.Log(move.cellFrom.pos + " -> " + move.cellTo.pos);
                _board.fight.beginAttack(move.cellFrom, move.cellTo);
            }
            else if (move.type == BotMoveType.FIGHT_CELL_CONTINUE)
            {
                Debug.Log(move.cellFrom.pos + " continue fight");
                if (move.cellFrom.piece.relation == Relation.ENEMY)
                {
                    _board.fight.animateFightCellDefenderAttack(move.cellFrom);
                }
                else
                {
                    _board.fight.animateFightCellAttackerAttack(move.cellFrom);
                }

            }

        }
    }
}

