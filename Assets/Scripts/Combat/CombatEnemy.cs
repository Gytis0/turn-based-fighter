using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatEnemy : CombatHumanoid
{
    public delegate void EnemyTurnEnd(Action action, CombatHumanoid character);
    public event EnemyTurnEnd onEnemyTurnEnd;

    public override void ExecuteAction(Action action)
    {
        base.ExecuteAction(action);

    }
}
