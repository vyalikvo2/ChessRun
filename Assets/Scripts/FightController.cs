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

    [HideInInspector] public Board board;

    [HideInInspector] public string state = FightState.NONE;

    private Cell cell1; // attackers cell
    private Cell cell2; // cell where we have fight

    private Vector3 attackerPosition = new Vector3(-0.3f, 0, 0);
    private Vector3 defenderPosition = new Vector3(0.3f, 0, 0);
    private Vector3 attackedShift = new Vector3(0.1f, 0, 0);
    
    private Vector3 deathShift = new Vector3(0.2f,0,0);

    public void Setup(Board board)
    {
        this.board = board;
    }

    public void beginAttack(Cell cell1, Cell cell2)
    {
        state = FightState.ATTTACKER_MOVE_TO_ATTACK_CELL;
        
        this.cell1 = cell1;
        this.cell2 = cell2;
        
        // animate
        cell2.piece.stats.transform.DOLocalMove(new Vector3(1f, 0, 0), 1f);
        cell2.piece.spriteObj.transform.DOLocalMove(defenderPosition, 1f);
        
        cell1.piece.transform.DOLocalMove(cell2.transform.localPosition, 1f);
        cell1.piece.spriteObj.transform.DOLocalMove(attackerPosition, 1f).OnComplete(attackerFirstAttack);
    }

    public void attackerFirstAttack()
    {
        state = FightState.ATTACKER_ATTACK;
        
        cell1.piece.spriteObj.transform.DOLocalMove(-attackerPosition, 0.2f).OnComplete(applyAttackerAttack);
        cell1.piece.spriteObj.transform.DOLocalMove(Vector3.zero, 0.2f).SetDelay(0.2f);
        
        if (!isKilling(cell1,cell2))
        {
            cell2.piece.spriteObj.transform.DOLocalMove(attackedShift, 0.05f).SetDelay(0.2f);
            cell2.piece.spriteObj.transform.DOLocalMove(Vector3.zero, 0.5f).SetDelay(0.25f);
        }
        else
        {
            cell2.piece.spriteObj.transform.DOLocalMove(deathShift, 0.2f).SetDelay(0.2f);
            cell2.piece.killedAnimation(1, 0.2f, onDefenderKilled);
        }
    }

    private void onDefenderKilled()
    {
        board.processKilling(cell1, cell2);
        Game.gameController.fightEnded();
    }

    private void applyAttackerAttack()
    {
        int newHealth = cell2.piece.stats.health - cell1.piece.stats.attack;
        newHealth = Math.Max(0, newHealth);
        cell2.piece.stats.health = newHealth;
    }

    private bool isKilling(Cell cell1, Cell cell2)
    {
        return cell2.piece.stats.health <= cell1.piece.stats.attack;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
