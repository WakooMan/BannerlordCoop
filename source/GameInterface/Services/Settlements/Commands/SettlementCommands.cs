﻿using Autofac;
using GameInterface.Services.ObjectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using static TaleWorlds.CampaignSystem.Settlements.Settlement;
using static TaleWorlds.Library.CommandLineFunctionality;

namespace GameInterface.Services.Template.Commands;

internal class SettlementCommands
{
    [CommandLineArgumentFunction("enter_random_castle", "coop.debug.settlements")]
    public static string EnterRandomCastle(List<string> strings)
    {
        var castles = Campaign.Current.CampaignObjectManager.Settlements.Where(settlement => settlement.IsCastle).ToArray();

        Random random = new Random();

        var randomCastle = castles[random.Next(castles.Length)];

        EncounterManager.StartSettlementEncounter(MobileParty.MainParty, randomCastle);

        return $"Entering {randomCastle.Name} castle";
    }

    [CommandLineArgumentFunction("get_town_name", "coop.debug.settlements")]
    public static string GetTownName(List<string> strings)
    {
        if (strings.Count != 1) return "Invalid usage, expected \"get_town_name <settlment id>\"";

        string settlementId = strings.Single();

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get town name";

        var objectManager = container.Resolve<IObjectManager>();

        if (objectManager.Contains(settlementId) == false) return $"{settlementId} does not exist";

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"{settlementId} was in object manager but was not of type Settlement";

        return $"Settlement Name: {settlement.Name}";
    }

    // coop.debug.settlements.set_enemies_spotted town_ES3 45.4
    /// <summary>
    /// Changes the NumberOfEnemiesSpottedAround
    /// </summary>
    /// <param name="args">the settlement and float value</param>
    /// <returns>info that is was succesfull</returns>
    [CommandLineArgumentFunction("set_enemies_spotted", "coop.debug.settlements")]
    public static string SetEnemiesSpotted(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_enemies_spotted <settlment id> <float_value>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";


        if (float.TryParse(args[1], out var num) == false)
        {
            return $"Error setting the value: {args[1]} to a float.";
        }

        settlement.NumberOfEnemiesSpottedAround = num;

        return $"Successfully set the Settlement ({settlementId}) NumberOfEnemiesSpottedAround to '{args[1]}'";
    }


    // coop.debug.settlements.set_allies_spotted town_ES3 45.4
    /// <summary>
    /// Changes the NumberOfAlliesSpottedAround
    /// </summary>
    /// <param name="args">the settlement and float value</param>
    /// <returns>info that is was succesful</returns>
    [CommandLineArgumentFunction("set_allies_spotted", "coop.debug.settlements")]
    public static string SetAlliesSpotted(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_enemies_spotted <settlment id> <float_value>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";

        if (float.TryParse(args[1], out var num) == false)
            return $"Error setting the value: {args[1]} to a float.";

        settlement.NumberOfAlliesSpottedAround = num;

        return $"Successfully set the Settlement ({settlementId}) NumberOfAlliesSpottedAround to '{args[1]}'";
    }


    // coop.debug.settlements.set_bribe_paid town_ES3 50.0
    /// <summary>
    /// Changes the BribePaid
    /// </summary>
    /// <param name="args">the settlement and int value</param>
    /// <returns>info that is was succesful</returns>
    [CommandLineArgumentFunction("set_bribe_paid", "coop.debug.settlements")]
    public static string SetBribePaid(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_bribe_paid <settlment id> <int_value>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";

        if (int.TryParse(args[1], out var num) == false)
            return $"Error setting the value: {args[1]} to a int.";

        settlement.BribePaid = num;

        return $"Successfully set the Settlement ({settlementId}) BribePaid to '{args[1]}'";
    }


    // coop.debug.settlements.set_hit_points town_ES3 50.4
    /// <summary>
    /// Changes the SettlementHitPoints
    /// </summary>
    /// <param name="args">the settlement and float value</param>
    /// <returns>info that is was succesful</returns>
    [CommandLineArgumentFunction("set_hit_points", "coop.debug.settlements")]
    public static string SetHitPoints(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_bribe_paid <settlment id> <int_value>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";

        if (float.TryParse(args[1], out var num) == false)
            return $"Error setting the value: {args[1]} to a float.";

        settlement.SettlementHitPoints = num;

        return $"Successfully set the Settlement ({settlementId}) SettlementHitPoints to '{args[1]}'";
    }

    // coop.debug.settlements.last_attacker town_ES1 CoopParty
    // coop.debug.settlements.last_attacker town_ES3 lord_2_8_party_1
    /// <summary>
    /// Changes the LastAttackerParty
    /// </summary>
    /// <param name="args">the settlementid and last_attacker</param>
    /// <returns>info that is was succesful</returns>
    [CommandLineArgumentFunction("last_attacker", "coop.debug.settlements")]
    public static string SetLastAttackerParty(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"last_attacker <settlementId> <last_attacker_id>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];
        string mobilePartyId = args[1];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";


        if (objectManager.TryGetObject<MobileParty>(mobilePartyId, out var mobileParty) == false)
            return $"Settlement: {mobilePartyId} was not found.";


        settlement.LastAttackerParty = mobileParty;


        return $"Successfully set the Settlement ({settlementId}) MobileParty to '{mobileParty.StringId}'";
    }

    // coop.debug.settlements.list_siege_state
    /// <summary>
    // Lists all the possible siege states
    /// </summary>
    /// <returns>all the siegeStates</returns>
    [CommandLineArgumentFunction("list_siege_state", "coop.debug.settlements")]
    public static string ListSiegeStates(List<string> args)
    {

        StringBuilder sb = new();

        foreach(int i in Enum.GetValues(typeof(Settlement.SiegeState))) {
            sb.AppendLine($"{i}: {Enum.GetName(typeof(Settlement.SiegeState), i)}");
        }
        return sb.ToString();
    }

    // coop.debug.settlements.set_siege_state town_ES1 InTheLordsHall
    /// <summary>
    /// Changes the SiegeState
    /// </summary>
    /// <param name="args">the settlementid and SiegeState</param>
    /// <returns>info that is was succesful</returns>
    [CommandLineArgumentFunction("set_siege_state", "coop.debug.settlements")]
    public static string SetSiegeState(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_siege_state <settlementId> <siege_state>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];
        string siegeState = args[1];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";

        if (Enum.TryParse<SiegeState>(siegeState, true, out var state) == false)
            return $"{siegeState} was not a valid enum in {nameof(SiegeState)}";


        settlement.CurrentSiegeState = state;


        return $"Successfully set the Settlement ({settlementId}) SiegeState to '{siegeState}'";
    }



    // coop.debug.settlements.set_militia town_ES1 45.0
    /// <summary>
    /// Changes the SiegeState
    /// </summary>
    /// <param name="args">the settlementid and float of how many troops (negative or pos)</param>
    /// <returns>info that is was succesful</returns>
    [CommandLineArgumentFunction("set_militia", "coop.debug.settlements")]
    public static string SetMiltiia(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_siege_state <settlementId> <militia_float>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];
        string militiaFloat = args[1];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";


        if (float.TryParse(militiaFloat, out var militia) == false)
            return $"Error setting the value: {militiaFloat} to a float.";

        settlement.Militia = militia;


        return $"Successfully set the Settlement ({settlementId}) Militia to '{militia}'";
    }


    // coop.debug.settlements.set_garrison_pay_limit town_ES3 23
    /// <summary>
    /// Changes the SiegeState
    /// </summary>
    /// <param name="args">the settlementid and float of how many troops (negative or pos)</param>
    /// <returns>info that is was succesful</returns>
    [CommandLineArgumentFunction("set_garrison_pay_limit", "coop.debug.settlements")]
    public static string SetGarrisonWageLimit(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_siege_state <settlementId> <militia_float>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];
        string garrisonInt = args[1];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";


        if (int.TryParse(garrisonInt, out var wageLimit) == false)
            return $"Error setting the value: {garrisonInt} to an int.";

        settlement.SetGarrisonWagePaymentLimit(wageLimit);


        return $"Successfully set the Settlement ({settlementId}) GarrisonWagePaymentLimit to '{wageLimit}'";
    }


    // coop.debug.settlements.collect_cache_notables town_ES3
    /// <summary>
    /// Tests collecting of notables to cache.
    /// </summary>
    /// <param name="args">the settlementid </param>
    /// <returns>info that is was successful</returns>
    [CommandLineArgumentFunction("collect_cache_notables", "coop.debug.settlements")]
    public static string CollectCacheNotables(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 1) return "Invalid usage, expected \"collect_cache_notables <settlementId>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args[0];

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";


        settlement.CollectNotablesToCache();


        return $"Successfully called settlement.CollectNotablesToCache() for {settlementId}.";
    }



    // Located in Modules\SandBox\ModuleData\settlements.xml
    // POROS EXAMPLE
    // coop.debug.settlements.info town_ES3 
    /// <summary>
    /// Gives a bunch of information on a settlement.
    /// </summary>
    /// <param name="args">settlement name</param>
    /// <returns>info about the settlement</returns>
    [CommandLineArgumentFunction("info", "coop.debug.settlements")]
    public static string Info(List<string> args)
    {

        if (args.Count != 1) return "Invalid usage, expected \"info <settlment id>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get Settlement";

        var objectManager = container.Resolve<IObjectManager>();

        string settlementId = args.Single();

        if (objectManager.TryGetObject<Settlement>(settlementId, out var settlement) == false)
            return $"Settlement: {settlementId} was not found.";

        StringBuilder sb = new();

        string lastAttackerParty = settlement.LastAttackerParty?.ArmyName.ToString() ?? "None";

        sb.AppendLine($"------------------- SETTLEMENT: {settlement.Name} -------------------");
        sb.AppendLine($"NumberOfEnemiesSpottedAround: '{settlement.NumberOfEnemiesSpottedAround}'");
        sb.AppendLine($"NumberOfAlliesSpottedAround: '{settlement.NumberOfAlliesSpottedAround}'");
        sb.AppendLine($"BribePaid: {settlement.BribePaid}");
        sb.AppendLine($"SettlementHitPoints: '{settlement.SettlementHitPoints}'");
        sb.AppendLine($"GarrisonWagePaymentLimit: '{settlement.GarrisonWagePaymentLimit}'");
        sb.AppendLine($"LastAttackerParty: '{lastAttackerParty}'");
        sb.AppendLine($"LastThreatTime:  '{settlement.LastThreatTime}'");
        sb.AppendLine($"CurrentSiegeState:   '{settlement.CurrentSiegeState}'");
        sb.AppendLine($"Militia :   '{settlement.Militia}'");
        sb.AppendLine($"LastVisitTimeOfOwner  :   '{settlement.LastVisitTimeOfOwner}'");
        sb.AppendLine($"ClaimedBy   :   '{settlement.ClaimedBy}'");
        sb.AppendLine($"ClaimValue    :   '{settlement.ClaimValue}'");
        sb.AppendLine($"CanBeClaimed     :   '{Convert.ToBoolean(settlement.CanBeClaimed)}'");
        sb.AppendLine($"------------------- SETTLEMENT: {settlement.Name} -------------------");

        return sb.ToString();
    }

    // coop.debug.settlementcomponent.set_owner town_comp_ES3 lord_6_5_party_1
    // Change Poros component owner
    /// <summary>
    /// Changes <see cref="SettlementComponent.Owner"/>
    /// </summary>
    /// <param name="args"><see cref="SettlementComponent"/> id, <see cref="MobileParty"/> or <see cref="Settlement"/> id</param>
    /// <returns>info that is was successful</returns>
    [CommandLineArgumentFunction("set_owner", "coop.debug.settlementComponent")]
    public static string SetOwner(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_owner <settlmentComponent id> <Mobile party id>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get SettlementComponent";
        var objectManager = container.Resolve<IObjectManager>();
        string settlementComponentId = args[0];
        string partyBaseId = args[1];
        PartyBase partyBase;
        if (objectManager.TryGetObject<SettlementComponent>(settlementComponentId, out var settlementComponent) == false)
            return $"SettlementComponent: {settlementComponentId} was not found.";
        if (objectManager.TryGetObject<Settlement>(partyBaseId, out var settlement))
        {
            partyBase = settlement.Party;
        }
        else if (objectManager.TryGetObject<MobileParty>(partyBaseId, out var mobileParty))
        {
            partyBase = mobileParty.Party;
        }
        else
        {
            return $"PartyBase: {partyBaseId} was not found.";
        }

        settlementComponent.Owner = partyBase;

        return $"Successfully set the SettlementComponent ({settlementComponentId}) Owner to '{args[1]}'";
    }

    // coop.debug.settlementcomponent.set_gold town_comp_ES3 401021
    // Change Poros component gold
    /// <summary>
    /// Changes <see cref="SettlementComponent.Gold"/>
    /// </summary>
    /// <param name="args"><see cref="SettlementComponent"/> id, amount of gold</param>
    /// <returns>info that is was successful</returns>
    [CommandLineArgumentFunction("set_gold", "coop.debug.settlementComponent")]
    public static string SetGold(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_owner <settlmentComponent id> <Gold>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get SettlementComponent";
        var objectManager = container.Resolve<IObjectManager>();
        string settlementComponentId = args[0];
        if (int.TryParse(args[1], out int gold) == false)
        {
            return "Unable to parse gold amount";
        }
        if (objectManager.TryGetObject<SettlementComponent>(settlementComponentId, out var settlementComponent) == false)
            return $"SettlementComponent: {settlementComponentId} was not found.";

        settlementComponent.Gold = gold;

        return $"Successfully set the SettlementComponent ({settlementComponentId}) Gold to '{args[1]}'";
    }

    // coop.debug.settlementcomponent.set_is_owner_unassigned town_comp_ES3 true
    // Change Poros component IsOwnerUnassigned
    /// <summary>
    /// Changes <see cref="SettlementComponent.IsOwnerUnassigned"/>
    /// </summary>
    /// <param name="args"><see cref="SettlementComponent"/> id, new <see cref="SettlementComponent.IsOwnerUnassigned"/> value></param>
    /// <returns>info that is was successful</returns>
    [CommandLineArgumentFunction("set_is_owner_unassigned", "coop.debug.settlementComponent")]
    public static string SetIsOwnerUnassigned(List<string> args)
    {
        if (ModInformation.IsClient) return "This function can only be used by the server";

        if (args.Count != 2) return "Invalid usage, expected \"set_owner <settlmentComponent id> <boolean>\"";

        if (ContainerProvider.TryGetContainer(out var container) == false) return "Unable to get SettlementComponent";
        var objectManager = container.Resolve<IObjectManager>();
        string settlementComponentId = args[0];
        if (bool.TryParse(args[1], out bool value) == false)
        {
            return "Unable to parse IsOwnerUnassigned";
        }
        if (objectManager.TryGetObject<SettlementComponent>(settlementComponentId, out var settlementComponent) == false)
            return $"SettlementComponent: {settlementComponentId} was not found.";


        settlementComponent.IsOwnerUnassigned = value;


        return $"Successfully set the SettlementComponent ({settlementComponentId}) IsOwnerUnassigned to '{args[1]}'";
    }
}
