using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHumanoid : MonoBehaviour
{
    public delegate void Death(CombatHumanoid character);
    public event Death onDeath;

    public delegate void Fallen(CombatHumanoid character);
    public event Fallen onFallen;

    // References
    protected Equipment equipment;
    protected HumanoidProperties humanoidProperties;
    protected HumanoidAnimationController humanoidAnimationController;
    protected HumanoidMovementController humanoidMovementController;
    protected Collider weaponCollider;
    protected OffensiveTrigger weaponTrigger;
    protected Collider legCollider;
    [SerializeField] protected OffensiveTrigger legTrigger;
    [SerializeField] protected Action skipTurnAction;

    // States
    protected bool inCombat = false;
    protected bool isOneHanded = false;
    protected bool fallen = false;
    protected CombatStance combatStance = CombatStance.Standing;
    protected CombatState combatState = CombatState.None;
    public CharacterState newestState;

    // Actions
    protected Queue<Action> actionQueue = new Queue<Action>();
    protected List<ActionCombination> combinations = new List<ActionCombination>();
    protected List<Action> availableActions = new List<Action>();

    // Configuration
    [SerializeField] protected float dodgeSpeed = 5f;
    [SerializeField] protected float runSpeed = 3f;
    [SerializeField] protected float walkSpeed = 1f;
    [SerializeField] protected float rollSpeed = 3f;
    [SerializeField] protected float fallingSpeed = 3f;

    // Values
    protected int gridSpacing;
    protected Tuple<Vector2, Vector2> gridBoundaries;
    protected float fallOverThreshold = 15f;

    protected void Start()
    {
        equipment = transform.GetComponent<Equipment>();
        humanoidProperties = transform.GetComponent<HumanoidProperties>();
        humanoidAnimationController = transform.GetComponent<HumanoidAnimationController>();
        humanoidMovementController = transform.GetComponent<HumanoidMovementController>();

        skipTurnAction.staminaDrain = skipTurnAction.baseStaminaDrain;
        skipTurnAction.composureDrain = skipTurnAction.baseComposureDrain;
    }

    public void SetCombinations(List<ActionCombination> combinations)
    {
        this.combinations = combinations;
    }

    public virtual void EndTurn()
    {

    }

    public virtual void SkipTurn()
    {

    }

    public virtual void EnableCombatMode(bool _enable, float gridSpacing, Tuple<Vector2, Vector2> gridBoundaries)
    {
        inCombat = _enable;
        humanoidProperties.EnableCharacterStatsOverhead(_enable);
        humanoidMovementController.inCombat = _enable;
        this.gridBoundaries = gridBoundaries;
        humanoidAnimationController.SetState(AnimationStates.IDLE);

        if (_enable)
        {
            humanoidAnimationController.SetAnimationModes(equipment.IsTwoHanded(), equipment.IsLeftHanded(), inCombat);
            this.gridSpacing = (int)gridSpacing;
            legCollider = legTrigger.GetComponent<Collider>();
            weaponCollider = equipment.GetEquippedWeaponObject().GetComponent<Collider>();
            weaponTrigger = equipment.GetEquippedWeaponObject().GetComponent<OffensiveTrigger>();
            if (GetType() == typeof(CombatPlayer)) weaponTrigger.isThisPlayer = true;
            else weaponTrigger.isThisPlayer = false;

            newestState = new CharacterState(this);
        }
    }

    public virtual void ExecuteAction(Action action)
    {
        EnableWeapon(false); EnableKicking(false);
        if(action.actionName == ActionName.Skip && combatStance == CombatStance.Fallen)
        {
            action.staminaDrain *= 1.25f;
            action.composureDrain *= 1.25f;
        }
        humanoidProperties.AlterStamina(-action.staminaDrain);
        humanoidProperties.AlterComposure(-action.composureDrain);

        if (action.actionName == ActionName.Block)
        {
            if (action.directions[0] == Direction.Left)
            {
                humanoidAnimationController.SetState(AnimationStates.BLOCKING_LEFT);
                combatState = CombatState.Blocking_Left;
            }
            else if (action.directions[0] == Direction.Right)
            {
                humanoidAnimationController.SetState(AnimationStates.BLOCKING_RIGHT);
                combatState = CombatState.Blocking_Right;
            }
            else if (action.directions[0] == Direction.Left)
            {
                humanoidAnimationController.SetState(AnimationStates.BLOCKING);
                combatState = CombatState.Blocking;
            }
        }
        else if (action.actionName == ActionName.Dodge)
        {
            if (action.directions[0] == Direction.Forward)
            {
                humanoidMovementController.Move(transform.position + Vector3.forward * gridSpacing, dodgeSpeed, AnimationStates.DODGING_FORWARD, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_FORWARD);
            }
            else if (action.directions[0] == Direction.Backward)
            {
                humanoidMovementController.Move(transform.position + Vector3.back * gridSpacing, dodgeSpeed, AnimationStates.DODGING_BACKWARD, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_BACKWARD);
            }
            else if (action.directions[0] == Direction.Left)
            {
                humanoidMovementController.Move(transform.position + Vector3.left * gridSpacing, dodgeSpeed, AnimationStates.DODGING_LEFT, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_LEFT);
            }
            else if (action.directions[0] == Direction.Right)
            {
                humanoidMovementController.Move(transform.position + Vector3.right * gridSpacing, dodgeSpeed, AnimationStates.DODGING_RIGHT, 0);
                humanoidAnimationController.SetState(AnimationStates.DODGING_RIGHT);
            }
        }
        else if (action.actionName == ActionName.Duck)
        {
            humanoidAnimationController.SetState(AnimationStates.DUCKING);
            combatState = CombatState.Ducking;

        }
        else if (action.actionName == ActionName.Get_Up)
        {
            combatStance = CombatStance.Standing;
            humanoidAnimationController.SetState(AnimationStates.GETTING_UP);
            fallen = false;
        }
        else if (action.actionName == ActionName.Overhead)
        {
            EnableWeapon(true, action);
            humanoidAnimationController.SetState(AnimationStates.OVERHEAD);
        }
        else if (action.actionName == ActionName.Kick)
        {
            humanoidAnimationController.SetState(AnimationStates.KICKING);
            EnableKicking(true);
        }
        else if (action.actionName == ActionName.Roll)
        {
            Direction facingDirection = UtilityScripts.GetFacingDirection(transform.rotation);
            AnimationStates animation;
            if ((action.directions[0] == Direction.Left && facingDirection == Direction.Forward) ||
                (action.directions[0] == Direction.Forward && facingDirection == Direction.Right) ||
                (action.directions[0] == Direction.Right && facingDirection == Direction.Backward) ||
                (action.directions[0] == Direction.Backward && facingDirection == Direction.Left)) animation = AnimationStates.ROLLING_LEFT;
            else animation = AnimationStates.ROLLING_RIGHT;

            Dictionary<Direction, Vector3> fourDirections = UtilityScripts.GetFourDirections(transform.position, false, gridSpacing);
            humanoidAnimationController.SetState(animation);
            humanoidMovementController.Move(fourDirections[action.directions[0]], rollSpeed, animation, 0f, true);
        }
        else if (action.actionName == ActionName.Run)
        {
            if (action.directions[0] == Direction.Forward) humanoidMovementController.Move(transform.position + Vector3.forward * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);
            else if (action.directions[0] == Direction.Backward) humanoidMovementController.Move(transform.position + Vector3.back * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);
            else if (action.directions[0] == Direction.Left) humanoidMovementController.Move(transform.position + Vector3.left * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);
            else if (action.directions[0] == Direction.Right) humanoidMovementController.Move(transform.position + Vector3.right * gridSpacing * 2, runSpeed, AnimationStates.RUNNING);

            humanoidAnimationController.SetState(AnimationStates.RUNNING);
        }
        else if (action.actionName == ActionName.Stab)
        {
            EnableWeapon(true, action);
            if (action.directions[0] == Direction.Forward) humanoidAnimationController.SetState(AnimationStates.STABBING);
            else if (action.directions[0] == Direction.Backward) humanoidAnimationController.SetState(AnimationStates.STABBING_DOWN);
        }
        else if (action.actionName == ActionName.Stand_Ground)
        {
            humanoidAnimationController.SetState(AnimationStates.STAND_GROUND);
        }
        else if (action.actionName == ActionName.Step)
        {
            if (action.directions[0] == Direction.Forward) humanoidMovementController.Move(transform.position + Vector3.forward * gridSpacing, walkSpeed, AnimationStates.WALKING);
            else if (action.directions[0] == Direction.Backward) humanoidMovementController.Move(transform.position + Vector3.back * gridSpacing, walkSpeed, AnimationStates.WALKING);
            else if (action.directions[0] == Direction.Left) humanoidMovementController.Move(transform.position + Vector3.left * gridSpacing, walkSpeed, AnimationStates.WALKING);
            else if (action.directions[0] == Direction.Right) humanoidMovementController.Move(transform.position + Vector3.right * gridSpacing, walkSpeed, AnimationStates.WALKING);

            humanoidAnimationController.SetState(AnimationStates.WALKING);
        }
        else if (action.actionName == ActionName.Swing)
        {
            EnableWeapon(true, action);

            if (action.directions[0] == Direction.Left) humanoidAnimationController.SetState(AnimationStates.SWINGING_LEFT);
            else if (action.directions[0] == Direction.Right) humanoidAnimationController.SetState(AnimationStates.SWINGING_RIGHT);
        }
        else if (action.actionName == ActionName.Throw)
        {
            EnableWeapon(true, action);

            humanoidAnimationController.SetState(AnimationStates.THROWING);
        }
    }

    public virtual void PromptAction(List<Action> availableActions)
    {
        
    }

    public virtual void PromptAction()
    {

    }

    public virtual void DenyAction()
    {
        actionQueue.Clear();
    }


    public float GetStamina()
    {
        return humanoidProperties.GetStamina();
    }
    public float GetComposure() { return humanoidProperties.GetComposure(); }
    public float GetMaxComposure() { return humanoidProperties.GetMaxComposure(); }

    public CombatStance GetCombatStance() { return combatStance; }
    public CombatState GetCombatState() { return combatState; }

    public float GetWeaponWeight()
    {
        return equipment.GetWeaponWeight();
    }

    public float GetShieldWeight()
    {
        return equipment.GetShieldWeight();
    }

    public float GetAllWeight()
    {
        return equipment.GetAllWeight();
    }

    public OffensiveTrigger GetLegTrigger() { return legTrigger; }

    public List<ActionName> GetShieldActions()
    {
        return equipment.GetShieldActions();
    }

    public List<ActionName> GetWeaponActions()
    {
        return equipment.GetWeaponActions();
    }

    public void TakeDamage(Weapon weapon)
    {
        float damage = weapon.GetDamage();
        float damageReduction = equipment.GetArmorDamageReduction(weapon.GetDamageType());

        damage = damage * (damageReduction / 100);

        humanoidProperties.AlterHealth(-damage);
        humanoidProperties.AlterComposure(-15);
        equipment.DamageArmors(weapon);

        if(humanoidProperties.GetHealth() <= 0)
        {
            onDeath(this);
            return;
        }

        if(combatStance != CombatStance.Fallen && humanoidProperties.GetComposure() <= fallOverThreshold)
        {
            humanoidAnimationController.SetState(AnimationStates.FALLING);
            Direction facingDirection = UtilityScripts.GetFacingDirection(transform.rotation);
            Dictionary<Direction, Vector3> fourDirections = UtilityScripts.GetFourDirections(transform.position, true, (int)gridSpacing);

            if(UtilityScripts.IsPositionValid(transform.position, fourDirections[facingDirection], gridBoundaries))
            {
                foreach (Direction direction in fourDirections.Keys)
                {
                    if (facingDirection == direction)
                    {
                        humanoidMovementController.Move(fourDirections[direction], fallingSpeed, AnimationStates.FALLING, 0f, true);
                        break;
                    }
                }
            }
            

            combatStance = CombatStance.Fallen;
        }

    }

    public void GetKicked(Weapon kick)
    {
        humanoidProperties.AlterComposure(-5);
        TakeDamage(kick);
    }

    void EnableWeapon(bool enable, Action action = null)
    {
        weaponCollider.isTrigger = enable;
        if (enable)
        {
            weaponTrigger.SetAction(action);
        }
    }

    void EnableKicking(bool enable)
    {
        legCollider.isTrigger = enable;
        if (enable)
        {
            legTrigger.SetAction(new Action(ActionName.Kick, ActionType.Offensive));
        }
    }

    public string GetCurrentAnimationName()
    {
        return humanoidAnimationController.GetCurrentAnimationName();

    }
    
    private void Update()
    {
        if (GetCurrentAnimationName().ToLower().Contains("getting_up"))
        {
            fallen = false;
        }
        if(!fallen && GetCurrentAnimationName().ToLower().Contains("fallen_idle"))
        {
            fallen = true;
            onFallen(this);
        }
    }
}
