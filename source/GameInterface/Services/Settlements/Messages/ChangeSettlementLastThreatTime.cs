﻿using Common.Messaging;

namespace GameInterface.Services.Settlements.Messages;

/// <summary>
/// 
/// </summary>
public record ChangeSettlementLastThreatTime : ICommand
{
    public string SettlementId { get; }
    public long? LastThreatTimeTicks { get; }

    public ChangeSettlementLastThreatTime(string settlementId, long? lastThreatTimeTicks)
    {
        SettlementId = settlementId;
        LastThreatTimeTicks = lastThreatTimeTicks;
    }
}
