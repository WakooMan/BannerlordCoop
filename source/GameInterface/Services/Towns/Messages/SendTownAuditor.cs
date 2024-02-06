using Common.Messaging;
using GameInterface.Services.Towns.Data;

namespace GameInterface.Services.Towns.Messages;

/// <summary>
/// Used by the town auditor debug command to send all sync value from client to server only
/// </summary>
public record SendTownAuditor : ICommand
{
    public List<TownAuditorData> Datas { get; }

    public SendTownAuditor(List<TownAuditorData> townAuditorDatas)
    {
        Data = townAuditorDatas;
    }
}
