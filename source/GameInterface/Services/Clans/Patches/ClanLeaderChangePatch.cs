﻿using Common;
using Common.Extensions;
using Common.Messaging;
using Common.Util;
using GameInterface.Services.Clans.Messages;
using GameInterface.Services.GameDebug.Patches;
using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace GameInterface.Services.Clans.Patches
{
    [HarmonyPatch(typeof(ChangeClanLeaderAction), "ApplyInternal")]
    public class ClanLeaderChangePatch
    {
        private static readonly AllowedInstance<Clan> AllowedInstance = new AllowedInstance<Clan>();

        private static readonly Action<Clan, Hero> ApplyInternal =
            typeof(ChangeClanLeaderAction)
            .GetMethod("ApplyInternal", BindingFlags.NonPublic | BindingFlags.Static)
            .BuildDelegate<Action<Clan, Hero>>();

        static bool Prefix(Clan clan, Hero newLeader = null)
        {
            CallStackValidator.Validate(clan, AllowedInstance);

            if (AllowedInstance.IsAllowed(clan)) return true;

            MessageBroker.Instance.Publish(clan, new ChangeClanLeader(clan.StringId, newLeader.StringId));

            return false;
        }
        public static void RunOriginalChangeClanLeader(Clan clan, Hero newLeader = null)
        {
            using (AllowedInstance)
            {
                AllowedInstance.Instance = clan;

                GameLoopRunner.RunOnMainThread(() =>
                {
                    ApplyInternal.Invoke(clan, newLeader);
                }, true);
            }
        }
    }
}
