﻿using GameInterface.Services.ObjectManager;
using ProtoBuf;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

namespace GameInterface.Services.Kingdoms.Data
{
    /// <summary>
    /// Base class for serializing <see cref="KingdomDecision"> class.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof(DeclareWarDecisionData))]
    [ProtoInclude(101, typeof(ExpelClanFromKingdomDecisionData))]
    [ProtoInclude(102, typeof(KingdomPolicyDecisionData))]
    [ProtoInclude(103, typeof(KingSelectionKingdomDecisionData))]
    [ProtoInclude(104, typeof(MakePeaceKingdomDecisionData))]
    [ProtoInclude(105, typeof(SettlementClaimantDecisionData))]
    [ProtoInclude(106, typeof(SettlementClaimantPreliminaryDecisionData))]
    public abstract class KingdomDecisionData
    {

        [ProtoMember(1)]
        public string ProposerClanId { get; }
        [ProtoMember(2)]
        public string KingdomId { get; }
        [ProtoMember(3)]
        public long TriggerTime { get; }
        [ProtoMember(4)]
        public bool IsEnforced { get; }
        [ProtoMember(5)]
        public bool NotifyPlayer { get; }
        [ProtoMember(6)]
        public bool PlayerExamined { get; }

        protected KingdomDecisionData(string proposedClanId, string kingdomId, long triggerTime, bool isEnforced, bool notifyPlayer, bool playerExamined)
        {
            ProposerClanId = proposedClanId;
            KingdomId = kingdomId;
            TriggerTime = triggerTime;
            IsEnforced = isEnforced;
            NotifyPlayer = notifyPlayer;
            PlayerExamined = playerExamined;
        }

        /// <summary>
        /// Tries to get the proposerClan and kingdom based on the deserialized Ids.
        /// </summary>
        /// <param name="objectManager">object manager.</param>
        /// <param name="proposerClan">proposer clan.</param>
        /// <param name="kingdom">kingdom.</param>
        /// <returns>True if proposerClan and kingdom is successfully received from the object manager, else false.</returns>
        protected bool TryGetProposerClanAndKingdom(IObjectManager objectManager, out Clan proposerClan, out Kingdom kingdom)
        {
            if (!objectManager.TryGetObject(ProposerClanId, out proposerClan) |
                !objectManager.TryGetObject(KingdomId, out kingdom))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the base class's properties for the KingdomDecision object.
        /// </summary>
        /// <param name="kingdomDecision">kingdom decision object.</param>
        /// <param name="proposerClan">proposer clan.</param>
        /// <param name="kingdom">kingdom.</param>
        protected void SetKingdomDecisionProperties(KingdomDecision kingdomDecision, Clan proposerClan, Kingdom kingdom)
        {
            kingdomDecision.ProposerClan = proposerClan;
            kingdomDecision._kingdom = kingdom;
            kingdomDecision.IsEnforced = IsEnforced;
            kingdomDecision.NotifyPlayer = NotifyPlayer;
            kingdomDecision.PlayerExamined = PlayerExamined;
            kingdomDecision.TriggerTime = new CampaignTime(TriggerTime);
        }

        /// <summary>
        /// Tries to get/create the kingdom decision object from the current KingdomDecisionData object.
        /// Implemented in derived class.
        /// </summary>
        public abstract bool TryGetKingdomDecision(IObjectManager objectManager, out KingdomDecision kingdomDecision);
    }
}
