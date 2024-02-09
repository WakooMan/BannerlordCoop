﻿using Common.Messaging;
using GameInterface.Services.Kingdoms.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameInterface.Services.Kingdoms.Messages
{
    /// <summary>
    /// Event that is handled on server side, when Kingdom.AddDecision method is called.
    /// </summary>
    public record LocalDecisionAdded: IEvent
    {
        public string KingdomId { get; }

        public KingdomDecisionData Data { get; }

        public bool IgnoreInfluenceCost { get; }

        public LocalDecisionAdded(string kingdomId, KingdomDecisionData data, bool ignoreInfluenceCost)
        {
            KingdomId = kingdomId;
            Data = data;
            IgnoreInfluenceCost = ignoreInfluenceCost;
        }
    }
}
