﻿using System;
using System.Collections.Generic;

namespace AchManager.AchievementTrigger
{
  /// <summary>
  /// Required current job for the trigger to trigger.
  /// </summary>
  public enum Job
  {
    /// <summary>
    /// Any class.
    /// </summary>
    Any = -1,

    /// <summary>
    /// Adventurer 
    /// </summary>
    ADV = 0,

    /// <summary>
    /// Gladiator 
    /// </summary>
    GLA = 1,

    /// <summary>
    /// Pugilist 
    /// </summary>
    PGL = 2,

    /// <summary>
    /// Marauder 
    /// </summary>
    MRD = 3,

    /// <summary>
    /// Lancer 
    /// </summary>
    LNC = 4,

    /// <summary>
    /// Archer 
    /// </summary>
    ARC = 5,

    /// <summary>
    /// Conjurer 
    /// </summary>
    CNJ = 6,

    /// <summary>
    /// Thaumaturge
    /// </summary>
    THM = 7,

    /// <summary>
    /// Carpenter
    /// </summary>
    CRP = 8,

    /// <summary>
    /// Blacksmith
    /// </summary>
    BSM = 9,

    /// <summary>
    /// Armorer
    /// </summary>
    ARM = 10,

    /// <summary>
    /// Goldsmith
    /// </summary>
    GSM = 11,

    /// <summary>
    /// Leatherworker
    /// </summary>
    LTW = 12,

    /// <summary>
    /// Weaver
    /// </summary>
    WVR = 13,

    /// <summary>
    /// Alchemist
    /// </summary>
    ALC = 14,

    /// <summary>
    /// Culinarian
    /// </summary>
    CUL = 15,

    /// <summary>
    /// Miner
    /// </summary>
    MIN = 16,

    /// <summary>
    /// Botanist
    /// </summary>
    BTN = 17,

    /// <summary>
    /// Fisher
    /// </summary>
    FSH = 18,

    /// <summary>
    /// Paladin 
    /// </summary>
    PLD = 19,

    /// <summary>
    /// Monk 
    /// </summary>
    MNK = 20,

    /// <summary>
    /// Warrior 
    /// </summary>
    WAR = 21,

    /// <summary>
    /// Dragoon 
    /// </summary>
    DRG = 22,

    /// <summary>
    /// Bard 
    /// </summary>
    BRD = 23,

    /// <summary>
    /// WhiteMage 
    /// </summary>
    WHM = 24,

    /// <summary>
    /// BlackMage
    /// </summary>
    BLM = 25,

    /// <summary>
    /// Arcanist 
    /// </summary>
    ACN = 26,

    /// <summary>
    /// Summoner 
    /// </summary>
    SMN = 27,

    /// <summary>
    /// Scholar 
    /// </summary>
    SCH = 28,

    /// <summary>
    /// Rogue 
    /// </summary>
    ROG = 29,

    /// <summary>
    /// Ninja 
    /// </summary>
    NIN = 30,

    /// <summary>
    /// Machinist 
    /// </summary>
    MCH = 31,

    /// <summary>
    /// DarkKnight 
    /// </summary>
    DRK = 32,

    /// <summary>
    /// Astrologian 
    /// </summary>
    AST = 33,

    /// <summary>
    /// Samurai 
    /// </summary>
    SAM = 34,

    /// <summary>
    /// RedMage 
    /// </summary>
    RDM = 35,

    /// <summary>
    /// BlueMage 
    /// </summary>
    BLU = 36,

    /// <summary>
    /// Gunbreaker 
    /// </summary>
    GNB = 37,

    /// <summary>
    /// Dancer 
    /// </summary>
    DNC = 38,

    /// <summary>
    /// Reaper 
    /// </summary>
    RPR = 39,

    /// <summary>
    /// Sage 
    /// </summary>
    SGE = 40,

    /// <summary>
    /// Viper 
    /// </summary>
    VPR = 41,

    /// <summary>
    /// Pictomancer 
    /// </summary>
    PCT = 42,
  }

  /// <summary>
  /// Base configuration for every trigger.
  /// </summary>
  [Serializable]
  public class TriggerConfig
  {
    #region Properties

    /// <summary>
    /// If true, shows a dalamud notification
    /// when an achievement gains progress.
    /// </summary>
    public bool ShowNotification { get; set; } = true;

    /// <summary>
    /// If true, shows a chat message
    /// when an achievement gains progress.
    /// </summary>
    public bool ShowChatMessage { get; set; } = true;

    /// <summary>
    /// Delay in ms to wait before sending the trigger.
    /// </summary>
    public int DelayMS { get; set; } = 0;

    /// <summary>
    /// If true, triggers only after this trigger has been triggered
    /// <see cref="TriggerEveryCount"/> times.
    /// </summary>
    public bool TriggerEveryXTimes { get; set; } = false;

    /// <summary>
    /// Amount of triggers necessary to actually trigger.
    /// </summary>
    public int TriggerEveryCount { get; set; } = 2;

    /// <summary>
    /// Required job for the trigger to trigger.
    /// </summary>
    public Job RequiredJob { get; set; } = Job.Any;

    /// <summary>
    /// List of territories that are either white-
    /// or blacklisted depending on <see cref="TreatRequiredTerritoriesAsWhitelist"/>.
    /// </summary>
    public List<uint> RequiredTerritories { get; set; } = [];

    /// <summary>
    /// Defines if the <see cref="RequiredTerritories"/> are treated
    /// as a whitelist. If false, it is treated as a blacklist.
    /// </summary>
    public bool TreatRequiredTerritoriesAsWhitelist {  get; set; } = false;

    #endregion Properties
  }
}