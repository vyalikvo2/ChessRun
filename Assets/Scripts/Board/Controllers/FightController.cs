using System;
using DG.Tweening;
using UnityEngine;
using ChessRun.Board.Pieces;

namespace ChessRun.Board.Controllers
{

public enum FightCellState
{
    NONE,
    ATTTACKER_MOVE_TO_ATTACK_CELL,
    ATTACKER_ATTACK,
    DEFENDER_ATTACK
}

public enum FightCellStatus
{
    NORMAL,
    DEFENDER
}

public enum FightCellResult
{
    KILLED_ATTACKER,
    KILLED_DEFENDER
}

public class FightController : MonoBehaviour
{
    public static float TIME_MOVE_TO_ATTACK = 0.8f;
    public static float TIME_ATTACKING = 0.2f;
    public static float TIME_ATTACKING_BACK = 0.2f;
    public static float TIME_ATTACKED_SHIFT = 0.15f;
    public static float TIME_ATTACKED_BACK_SHIFT = 0.15f;
    
    public static float TIME_DEFENCE_ALLY_ATTACK = 0.4f;
    public static float TIME_DEFENCE_ALLY_ATTACK_BACK = 0.3f;
    
    public static float TIME_DEATH_SHIFT = 0.2f;
    public static float TIME_WIN_AFTER_HELP = 0.2f; // time moving to center after killed enemy by aly

    public static float TIME_NORMALIZE_STATS = 0.2f; // time to move stats on normal position
 
    
    [HideInInspector] public ChessBoard Board;

    [HideInInspector] public FightCellState FightState = FightCellState.NONE;

    private Cell _cell1; // attackers cell
    private Cell _cell2; // cell where we have fight

    private BasePiece _defender;
    private BasePiece _attacker;

    private Vector3 _attackerPosition = new Vector3(-0.3f, 0, 0);
    private Vector3 _defenderPosition = new Vector3(0.3f, 0, 0);
    private Vector3 _attackedShift = new Vector3(0.1f, 0, 0);
    
    private Vector3 _deathShift = new Vector3(0.2f,0,0);

    public void Setup(ChessBoard board)
    {
        Board = board;
    }
    
    public void BeginAttack(Cell cell1, Cell cell2)
    {
        if (_checkCleaningFight("beginAttack")) return;

        FightState = FightCellState.ATTTACKER_MOVE_TO_ATTACK_CELL;

        _cell1 = cell1;
        _cell2 = cell2;
        
        _attacker = cell1.Piece;
        _defender = cell2.Piece;
        
        // animate
        _defender.SetFightStatus(FightCellStatus.DEFENDER, TIME_MOVE_TO_ATTACK);
        
        _defender.SpriteObj.transform.DOLocalMove(_defenderPosition, TIME_MOVE_TO_ATTACK);
        
        _attacker.transform.DOLocalMove(cell2.transform.localPosition, TIME_MOVE_TO_ATTACK);
        _attacker.SpriteObj.transform.DOLocalMove(_attackerPosition, TIME_MOVE_TO_ATTACK).OnComplete(_createFight);
    }
    
    public void AnimateAtackerAttack()
    {
        FightState = FightCellState.ATTACKER_ATTACK;
        
        _attacker.SpriteObj.transform.DOLocalMove(Vector3.zero, TIME_ATTACKING).OnComplete(_applyAttackerAttack);

        if (!_isKilling(_attacker,_defender))
        {
            _attacker.SpriteObj.transform.DOLocalMove(_attackerPosition, TIME_ATTACKING_BACK).SetDelay(TIME_ATTACKING);
            _defender.SpriteObj.transform.DOLocalMove(_defenderPosition + _attackedShift, TIME_ATTACKED_SHIFT).SetDelay(TIME_ATTACKING);
            _defender.SpriteObj.transform.DOLocalMove(_defenderPosition, TIME_ATTACKED_BACK_SHIFT).SetDelay(TIME_ATTACKING+TIME_ATTACKED_SHIFT)
                .OnComplete(_afterAttack);
        }
        else
        {
            _defender.SpriteObj.transform.DOLocalMove(_defenderPosition + _deathShift, TIME_DEATH_SHIFT).SetDelay(TIME_ATTACKING);
            _defender.KilledAnimation(1, TIME_DEATH_SHIFT, _onDefenderKilled);
        }
    }

    // ally attacks cell to defent teammate
    public void AnimateAllyHelpDefend(Cell cell1, Cell cell2)
    {
        if (_checkCleaningFight("animateAllyHelpDefend")) return;
        
        _cell1 = cell1;
        _cell2 = cell2;
        
        BasePiece allyPiece = cell2.Piece;

        _attacker = cell1.Piece;
        _defender = cell2.AttackerPiece;
        
        Vector3 pos = cell2.transform.localPosition - cell1.transform.localPosition;
        _attacker.SpriteObj.transform.DOLocalMove(pos, TIME_DEFENCE_ALLY_ATTACK).SetEase(Ease.InCubic).OnComplete(_applyAllyDefense);
        _attacker.SpriteObj.transform.DOLocalMove(Vector3.zero, TIME_DEFENCE_ALLY_ATTACK_BACK).SetEase(Ease.OutExpo).SetDelay(TIME_DEFENCE_ALLY_ATTACK);

        if (!_isKilling(_attacker, _defender))
        {
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition - _attackedShift, TIME_ATTACKED_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK);
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition, TIME_ATTACKED_BACK_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT).OnComplete(_afterHelpAttack);
        }
        else
        {
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition - _deathShift, TIME_DEATH_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT);
            _defender.KilledAnimation(-1, TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT, _onAttackerKilled);
            
            allyPiece.SpriteObj.transform.DOLocalMove(Vector3.zero, TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK);
        }
    }
    
    public void AnimateAllyHelpAttack(Cell cell1, Cell cell2)
    {
        if (_checkCleaningFight("animateAllyHelpAttack")) return;
        
        _cell1 = cell1;
        _cell2 = cell2;
        
        BasePiece allyPiece = cell2.AttackerPiece;

        _attacker = cell1.Piece;
        _defender = cell2.Piece;
        
        Vector3 pos = cell2.transform.localPosition - cell1.transform.localPosition;
        _attacker.SpriteObj.transform.DOLocalMove(pos, TIME_DEFENCE_ALLY_ATTACK).SetEase(Ease.InCubic).OnComplete(_applyAllyAttack);
        _attacker.SpriteObj.transform.DOLocalMove(Vector3.zero, TIME_MOVE_TO_ATTACK).SetEase(Ease.OutExpo).SetDelay(TIME_DEFENCE_ALLY_ATTACK);

        if (!_isKilling(_attacker, _defender))
        {
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition - _attackedShift, TIME_ATTACKED_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK);
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition, TIME_ATTACKED_BACK_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT).OnComplete(_afterHelpAttack);
        }
        else
        {
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition - _deathShift, TIME_DEATH_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT);
            _defender.KilledAnimation(-1, TIME_DEFENCE_ALLY_ATTACK, _onDefenderKilled);
            
            allyPiece.SpriteObj.transform.DOLocalMove(Vector3.zero, TIME_WIN_AFTER_HELP).SetDelay(TIME_DEFENCE_ALLY_ATTACK);
        }
    }
    
    // attacker continues his attack
    public void AnimateFightCellAttackerAttack(Cell cell)
    {
        if (_checkCleaningFight("animateFightCellAttackerAttack")) return;
        
        this._cell2 = cell;
        
        _attacker = cell.AttackerPiece;
        _defender = cell.Piece;
        
        FightState = FightCellState.ATTACKER_ATTACK;
        
        _attacker.SpriteObj.transform.DOLocalMove(Vector3.zero, TIME_ATTACKING).OnComplete(_applyAttackerAttack);

        if (!_isKilling(_attacker,_defender))
        {
            _attacker.SpriteObj.transform.DOLocalMove(_attackerPosition, TIME_ATTACKING_BACK).SetDelay(TIME_ATTACKING);
            _defender.SpriteObj.transform.DOLocalMove(_defenderPosition + _attackedShift, TIME_ATTACKED_SHIFT).SetDelay(TIME_ATTACKING);
            _defender.SpriteObj.transform.DOLocalMove(_defenderPosition, TIME_ATTACKED_BACK_SHIFT).SetDelay(TIME_ATTACKING+TIME_ATTACKED_SHIFT)
            .OnComplete(_afterAttack);
        }
        else
        {
            _defender.SpriteObj.transform.DOLocalMove(_defenderPosition + _deathShift, TIME_DEATH_SHIFT).SetDelay(TIME_ATTACKING);
            _defender.KilledAnimation(1, TIME_DEATH_SHIFT, _onDefenderKilled);
        }
    }
    
        
    // defender continues attack
    public void AnimateFightCellDefenderAttack(Cell cell)
    {       
        if (_checkCleaningFight("animateFightCellDefenderAttack")) return;
        
        this._cell2 = cell;
        
        _attacker = cell.Piece;
        _defender = cell.AttackerPiece;
        
        FightState = FightCellState.DEFENDER_ATTACK;
        
        _attacker.SpriteObj.transform.DOLocalMove(Vector3.zero, TIME_ATTACKING).OnComplete(_applyAttackerAttack);

        if (!_isKilling(_attacker,_defender))
        {
            _attacker.SpriteObj.transform.DOLocalMove(_defenderPosition, TIME_ATTACKING_BACK).SetDelay(TIME_ATTACKING);
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition - _attackedShift, TIME_ATTACKED_SHIFT).SetDelay(TIME_ATTACKING);
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition, TIME_ATTACKED_BACK_SHIFT).SetDelay(TIME_ATTACKING+TIME_ATTACKED_SHIFT)
                .OnComplete(_afterAttack);
        }
        else
        {
            _defender.SpriteObj.transform.DOLocalMove(_attackerPosition - _deathShift, TIME_DEATH_SHIFT).SetDelay(TIME_ATTACKING);
            _defender.KilledAnimation(1, TIME_DEATH_SHIFT, _onAttackerKilled);
        }
    }


    private void _onDefenderKilled()
    {
        bool isGameOver = Board.ProcessKilling(_cell2, FightCellResult.KILLED_DEFENDER);
        
        _clearFight();
        GameEngine.GameController.FightEnded(isGameOver);
    }
    
    private void _onAttackerKilled()
    {
        bool isGameOver = Board.ProcessKilling(_cell2, FightCellResult.KILLED_ATTACKER);
        // move stats to normal
        _cell2.Piece.SetFightStatus(FightCellStatus.NORMAL, TIME_NORMALIZE_STATS);
        
        _clearFight();
        GameEngine.GameController.FightEnded(isGameOver);
    }

    private void _applyAttackerAttack()
    {
        _applyAttack(_attacker, _defender);
    }

    private void _applyDefenderAttack()
    {
        _applyAttack(_defender, _attacker);
    }
    
    private void _applyAllyDefense()
    {
        _applyAttack(_attacker, _defender);
    }
    private void _applyAllyAttack()
    {
        _applyAttack(_attacker, _defender);
    }
    
    private void _applyAttack(BasePiece attacker, BasePiece defender)
    {
        int newHealth = defender.Stats.health - attacker.Stats.attack;
        newHealth = Math.Max(0, newHealth);
        defender.Stats.health = newHealth;
    }
    
    private void _afterAttack()
    {
        _clearFight();
        GameEngine.GameController.EndMove();
    }
    
    private void _afterHelpAttack()
    {
        _clearFight();
        GameEngine.GameController.EndMove();
    }

    private bool _isKilling(BasePiece attacker, BasePiece defender)
    {
        return defender.Stats.health <= attacker.Stats.attack;
    }

    private bool _checkCleaningFight(string logMsg)
    {
        bool notClean = _cell1 != null || _cell2 != null || _defender != null || _attacker != null;
        if (notClean)
        {
            Debug.Log("NOT CLEAN: "+logMsg);
        }
        return notClean;
    }
    
    private void _createFight()
    {
        Board.CreateFightAtCell(_cell1, _cell2);
        AnimateAtackerAttack();
    }

    private void _clearFight()
    {
        _cell1 = null;
        _cell2 = null;
        _defender = null;
        _attacker = null;
        FightState = FightCellState.NONE;
    }
}
}

