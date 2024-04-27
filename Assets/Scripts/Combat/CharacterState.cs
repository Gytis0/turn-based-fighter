using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
    public CharacterState(CombatHumanoid humanoid)
    {
        position = humanoid.transform.position;
        rotation = humanoid.transform.rotation;
        combatStance = humanoid.GetCombatStance();
        combatState = humanoid.GetCombatState();
        stamina = humanoid.GetStamina();
        composure = humanoid.GetComposure();
        maxComposure = humanoid.GetMaxComposure();
        allWeight = humanoid.GetAllWeight();
        weaponWeight = humanoid.GetWeaponWeight();
        shieldWeight = humanoid.GetShieldWeight();
        weaponActions = humanoid.GetWeaponActions();
        shieldActions = humanoid.GetShieldActions();
    }

    public void UpdateState(Vector3 position, Quaternion rotation, CombatStance combatStance, CombatState combatState, float stamina, float composure)
    {
        this.position = position;
        this.rotation = rotation;
        this.combatStance = combatStance;
        this.combatState = combatState;
        this.stamina = stamina;
        this.composure = composure;
    }
    public Vector3 position;
    public Quaternion rotation;
    public CombatStance combatStance;
    public CombatState combatState;
    public float stamina, composure, maxComposure;
    public float allWeight, weaponWeight, shieldWeight;
    public List<ActionName> weaponActions, shieldActions;
}
