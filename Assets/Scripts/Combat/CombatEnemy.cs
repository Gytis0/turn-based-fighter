using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEnemy : CombatHumanoid
{
    public delegate void EnemyTurnEnd(Action action, CombatHumanoid character);
    public event EnemyTurnEnd onEnemyTurnEnd;
}
