﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace Coop.Config
{
    class RequiredSyncTypes
    {
        public static readonly Type[] Types = new Type[]
        {
            typeof(MobileParty),
            typeof(Hero),
        };
    }
}