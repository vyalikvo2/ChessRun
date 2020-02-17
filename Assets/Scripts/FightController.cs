using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FightState
{
    public const string NONE = "none";
    public const string ATTTACKER_MOVE_TO_ATTACK_CELL = "attacker_move_to_attack_cell";
    public const string ATTACKER_ATTACK = "attacker_attack";
    public const string DEFENDER_ATTACK = "defender_attack";
}

public class FightController : MonoBehaviour
{
    public static int FIGHT_STATUS_NORMAL = 0;
    public static int FIGHT_STATUS_DEFENDER = 1;

    public static int KILLED_ATTACKER = 0;
    public static int KILLED_DEFENDER = 1;
    
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
 
    
    [HideInInspector] public Board board;

    [HideInInspector] public string state = FightState.NONE;

    private Cell cell1; // attackers cell
    private Cell cell2; // cell where we have fight

    private BasePiece defender;
    private BasePiece attacker;

    private Vector3 attackerPosition = new Vector3(-0.3f, 0, 0);
    private Vector3 defenderPosition = new Vector3(0.3f, 0, 0);
    private Vector3 attackedShift = new Vector3(0.1f, 0, 0);
    
    private Vector3 deathShift = new Vector3(0.2f,0,0);

    public void Setup(Board board)
    {
        this.board = board;
    }

    private bool checkCleaningFight(string name)
    {
        bool notClean = cell1 != null || cell2 != null || defender != null || attacker != null;
        if (notClean)
        {
            Debug.Log("NOT CLEAN: "+name);
        }
        return notClean;
    }

    public void beginAttack(Cell cell1, Cell cell2)
    {
        if (checkCleaningFight("beginAttack")) return;

        state = FightState.ATTTACKER_MOVE_TO_ATTACK_CELL;

        this.cell1 = cell1;
        this.cell2 = cell2;
        
        attacker = cell1.piece;
        defender = cell2.piece;
        
        // animate
        defender.setFightStatus(FIGHT_STATUS_DEFENDER, TIME_MOVE_TO_ATTACK);
        
        defender.spriteObj.transform.DOLocalMove(defenderPosition, TIME_MOVE_TO_ATTACK);
        
        attacker.transform.DOLocalMove(cell2.transform.localPosition, TIME_MOVE_TO_ATTACK);
        attacker.spriteObj.transform.DOLocalMove(attackerPosition, TIME_MOVE_TO_ATTACK).OnComplete(animateAtackerAttack);
    }

    // defender attack animation
    public void animateAtackerAttack()
    {
        board.createFightAtCell(cell1, cell2);
        state = FightState.ATTACKER_ATTACK;
        
        attacker.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_ATTACKING).OnComplete(applyAttackerAttack);

        if (!isKilling(attacker,defender))
        {
            attacker.spriteObj.transform.DOLocalMove(attackerPosition, TIME_ATTACKING_BACK).SetDelay(TIME_ATTACKING);
            defender.spriteObj.transform.DOLocalMove(defenderPosition + attackedShift, TIME_ATTACKED_SHIFT).SetDelay(TIME_ATTACKING);
            defender.spriteObj.transform.DOLocalMove(defenderPosition, TIME_ATTACKED_BACK_SHIFT).SetDelay(TIME_ATTACKING+TIME_ATTACKED_SHIFT)
                .OnComplete(animateDefenderAttack);
        }
        else
        {
            defender.spriteObj.transform.DOLocalMove(defenderPosition + deathShift, TIME_DEATH_SHIFT).SetDelay(TIME_ATTACKING);
            defender.killedAnimation(1, TIME_DEATH_SHIFT, onDefenderKilled);
        }
    }

    // defender attack animation
    private void animateDefenderAttack()
    {
        state = FightState.DEFENDER_ATTACK;
        
        defender.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_ATTACKING).OnComplete(applyDefenderAttack);
        
        if (!isKilling(defender,attacker))
        {
            defender.spriteObj.transform.DOLocalMove(defenderPosition, TIME_ATTACKING_BACK).SetDelay(TIME_ATTACKING);
            attacker.spriteObj.transform.DOLocalMove(attackerPosition - attackedShift, TIME_ATTACKED_SHIFT).SetDelay(TIME_ATTACKING);
            attacker.spriteObj.transform.DOLocalMove(attackerPosition, TIME_ATTACKED_BACK_SHIFT)
                .SetDelay(TIME_ATTACKING + TIME_ATTACKED_SHIFT).OnComplete(afterAtack);
        }
        else
        {
            attacker.spriteObj.transform.DOLocalMove(attackerPosition - deathShift, TIME_DEATH_SHIFT).SetDelay(TIME_ATTACKING);
            attacker.killedAnimation(-1, TIME_DEATH_SHIFT, onAttackerKilled);
        }
    }
    
    // ally attacks cell to defent teammate
    public void animateAllyHelpDefend(Cell cell1, Cell cell2)
    {
        if (checkCleaningFight("animateAllyHelpDefend")) return;
        
        this.cell1 = cell1;
        this.cell2 = cell2;
        
        BasePiece allyPiece = cell2.piece;

        attacker = cell1.piece;
        defender = cell2.attackerPiece;
        
        Vector3 pos = cell2.transform.localPosition - cell1.transform.localPosition;
        attacker.spriteObj.transform.DOLocalMove(pos, TIME_DEFENCE_ALLY_ATTACK).SetEase(Ease.InCubic).OnComplete(applyAllyDefense);
        attacker.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_DEFENCE_ALLY_ATTACK_BACK).SetEase(Ease.OutExpo).SetDelay(TIME_DEFENCE_ALLY_ATTACK);

        if (!isKilling(attacker, defender))
        {
            defender.spriteObj.transform.DOLocalMove(attackerPosition - attackedShift, TIME_ATTACKED_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK);
            defender.spriteObj.transform.DOLocalMove(attackerPosition, TIME_ATTACKED_BACK_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT).OnComplete(afterHelpAttack);
        }
        else
        {
            defender.spriteObj.transform.DOLocalMove(attackerPosition - deathShift, TIME_DEATH_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT);
            defender.killedAnimation(-1, TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT, onAttackerKilled);
            
            allyPiece.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK);
        }
    }
    
    public void animateAllyHelpAttack(Cell cell1, Cell cell2)
    {
        if (checkCleaningFight("animateAllyHelpAttack")) return;
        
        this.cell1 = cell1;
        this.cell2 = cell2;
        
        BasePiece allyPiece = cell2.attackerPiece;

        attacker = cell1.piece;
        defender = cell2.piece;
        
        Vector3 pos = cell2.transform.localPosition - cell1.transform.localPosition;
        attacker.spriteObj.transform.DOLocalMove(pos, TIME_DEFENCE_ALLY_ATTACK).SetEase(Ease.InCubic).OnComplete(applyAllyAttack);
        attacker.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_MOVE_TO_ATTACK).SetEase(Ease.OutExpo).SetDelay(TIME_DEFENCE_ALLY_ATTACK);

        if (!isKilling(attacker, defender))
        {
            defender.spriteObj.transform.DOLocalMove(attackerPosition - attackedShift, TIME_ATTACKED_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK);
            defender.spriteObj.transform.DOLocalMove(attackerPosition, TIME_ATTACKED_BACK_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT).OnComplete(afterHelpAttack);
        }
        else
        {
            defender.spriteObj.transform.DOLocalMove(attackerPosition - deathShift, TIME_DEATH_SHIFT)
                .SetDelay(TIME_DEFENCE_ALLY_ATTACK + TIME_ATTACKED_SHIFT);
            defender.killedAnimation(-1, TIME_DEFENCE_ALLY_ATTACK, onDefenderKilled);
            
            allyPiece.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_WIN_AFTER_HELP).SetDelay(TIME_DEFENCE_ALLY_ATTACK);
        }
    }
    
    // attacker continues his attack
    public void animateFightCellAttackerAttack(Cell cell)
    {
        if (checkCleaningFight("animateFightCellAttackerAttack")) return;
        
        this.cell2 = cell;
        
        attacker = cell.attackerPiece;
        defender = cell.piece;
        
        state = FightState.ATTACKER_ATTACK;
        
        attacker.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_ATTACKING).OnComplete(applyAttackerAttack);

        if (!isKilling(attacker,defender))
        {
            attacker.spriteObj.transform.DOLocalMove(attackerPosition, TIME_ATTACKING_BACK).SetDelay(TIME_ATTACKING);
            defender.spriteObj.transform.DOLocalMove(defenderPosition + attackedShift, TIME_ATTACKED_SHIFT).SetDelay(TIME_ATTACKING);
            defender.spriteObj.transform.DOLocalMove(defenderPosition, TIME_ATTACKED_BACK_SHIFT).SetDelay(TIME_ATTACKING+TIME_ATTACKED_SHIFT).OnComplete(animateDefenderAttack);
        }
        else
        {
            defender.spriteObj.transform.DOLocalMove(defenderPosition + deathShift, TIME_DEATH_SHIFT).SetDelay(TIME_ATTACKING);
            defender.killedAnimation(1, TIME_DEATH_SHIFT, onDefenderKilled);
        }
    }
    
        
    // defender continues attack
    public void animateFightCellDefenderAttack(Cell cell)
    {       
        if (checkCleaningFight("animateFightCellDefenderAttack")) return;
        
        this.cell2 = cell;
        
        attacker = cell.piece;
        defender = cell.attackerPiece;
        
        state = FightState.DEFENDER_ATTACK;
        
        attacker.spriteObj.transform.DOLocalMove(Vector3.zero, TIME_ATTACKING).OnComplete(applyAttackerAttack);

        if (!isKilling(attacker,defender))
        {
            attacker.spriteObj.transform.DOLocalMove(defenderPosition, TIME_ATTACKING_BACK).SetDelay(TIME_ATTACKING);
            defender.spriteObj.transform.DOLocalMove(attackerPosition - attackedShift, TIME_ATTACKED_SHIFT).SetDelay(TIME_ATTACKING);
            defender.spriteObj.transform.DOLocalMove(attackerPosition, TIME_ATTACKED_BACK_SHIFT).SetDelay(TIME_ATTACKING+TIME_ATTACKED_SHIFT)
                .OnComplete(animateAtackerAttack);
        }
        else
        {
            defender.spriteObj.transform.DOLocalMove(attackerPosition - deathShift, TIME_DEATH_SHIFT).SetDelay(TIME_ATTACKING);
            defender.killedAnimation(1, TIME_DEATH_SHIFT, onAttackerKilled);
        }
    }


    private void onDefenderKilled()
    {
        bool isGameOver = board.processKilling(cell2, KILLED_DEFENDER);
        
        clearFight();
        Game.gameController.fightEnded(isGameOver);
    }
    
    private void onAttackerKilled()
    {
        bool isGameOver = board.processKilling(cell2, KILLED_ATTACKER);
        // move stats to normal
        cell2.piece.setFightStatus(FIGHT_STATUS_NORMAL, TIME_NORMALIZE_STATS);
        
        clearFight();
        Game.gameController.fightEnded(isGameOver);
    }

    private void applyAttackerAttack()
    {
        applyAttack(attacker, defender);
    }
    
    
    private void applyDefenderAttack()
    {
        applyAttack(defender, attacker);
    }
    
    private void applyAllyDefense()
    {
        applyAttack(attacker, defender);
    }
    private void applyAllyAttack()
    {
        applyAttack(attacker, defender);
    }
    
    private void applyAttack(BasePiece attacker, BasePiece defender)
    {
        int newHealth = defender.stats.health - attacker.stats.attack;
        newHealth = Math.Max(0, newHealth);
        defender.stats.health = newHealth;
    }
    
    private void afterAtack()
    {
        clearFight();
        Game.gameController.endMove();
    }
    
    private void afterHelpAttack()
    {
        clearFight();
        Game.gameController.endMove();
    }

    private bool isKilling(BasePiece attacker, BasePiece defender)
    {
        return defender.stats.health <= attacker.stats.attack;
    }

    private void clearFight()
    {
        cell1 = null;
        cell2 = null;
        defender = null;
        attacker = null;
        state = FightState.NONE;
    }

}
