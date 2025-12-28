// Assets/Scripts/Core/UnitDataSO.cs
using UnityEngine;
using System.Collections.Generic;

public enum CharacterClass
{
    Chronomancer,
    Inquisitor,
    SoulEater,
    Artificer,
    ShadowFin,
    BattleBard,
    MonsterHunter,
    NecromancerAnimator,
    BerserkerVampire,
    WorldWalker
}

public enum Race
{
    Silvani,
    Crystallids,
    Aerogens,
    Mycolds,
    DepthDwarves,
    AbyssElves,
    Slimeborn,
    Elxi,
    Biomorphs,
    Human
}

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Battle/Unit Data")]
public class UnitDataSO : ScriptableObject
{
    [Header("Identity")]
    public string unitName;
    public CharacterClass characterClass;
    public Race race;
    public int baseStarLevel = 1;
    public int maxStarLevel = 3;

    [Header("Visual")]
    public Sprite portrait;
    public Sprite battleSprite;
    public GameObject unitPrefab;
    public RuntimeAnimatorController animatorController;

    [Header("Base Stats")]
    public int maxHealth = 100;
    public int attackPower = 20;
    public int defense = 10;
    public int speed = 5;
    public float attackRange = 1.5f;
    public int moveRange = 3;

    [Header("Special Abilities")]
    public AbilityData basicAbility;
    public AbilityData ultimateAbility;

    [Header("Synergies")]
    public List<Race> raceSynergies;
    public List<CharacterClass> classSynergies;

    [Header("Shop Info")]
    public int shopCost = 100;
    public int tier = 1;
}

[System.Serializable]
public class AbilityData
{
    public string abilityName;
    public string description;
    public int damage;
    public int healAmount;
    public float effectDuration;
    public int cooldown;
    public int manaCost;
    public TargetType targetType;
    public List<StatusEffect> statusEffects;
}

public enum TargetType
{
    Single,
    Area,
    Self,
    Ally
}

[System.Serializable]
public class StatusEffect
{
    public EffectType type;
    public float duration;
    public int value;
}

public enum EffectType
{
    Stun,
    Poison,
    Slow,
    SpeedBoost,
    DamageBoost,
    DefenseBoost,
    HealOverTime
}

public enum DamageType
{
    Physical,
    Magic,
    True
}