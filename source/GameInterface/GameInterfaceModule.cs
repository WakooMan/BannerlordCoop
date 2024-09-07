﻿using Autofac;
using Common.PacketHandlers;
using GameInterface.AutoSync;
using GameInterface.Serialization;
using GameInterface.Services;
using GameInterface.Services.Entity;
using GameInterface.Services.ObjectManager;
using GameInterface.Services.Players;
using GameInterface.Services.Time;
using HarmonyLib;

namespace GameInterface;

public class GameInterfaceModule : Module
{
    // TODO move to config
    public const string HarmonyId = "TaleWorlds.MountAndBlade.Bannerlord.Coop";

    protected override void Load(ContainerBuilder builder)
    {
        
        builder.RegisterInstance(new Harmony(HarmonyId)).As<Harmony>().SingleInstance();

        builder.RegisterType<GameInterface>().As<IGameInterface>().InstancePerLifetimeScope().AutoActivate();
        builder.RegisterType<BinaryPackageFactory>().As<IBinaryPackageFactory>().InstancePerLifetimeScope();
        builder.RegisterType<ControllerIdProvider>().As<IControllerIdProvider>().InstancePerLifetimeScope();
        builder.RegisterType<ControlledEntityRegistry>().As<IControlledEntityRegistry>().InstancePerLifetimeScope();
        builder.RegisterType<TimeControlModeConverter>().As<ITimeControlModeConverter>().InstancePerLifetimeScope();
        builder.RegisterType<PlayerRegistry>().As<IPlayerRegistry>().InstancePerLifetimeScope();

        builder.RegisterType<AutoSyncBuilder>().As<IAutoSyncBuilder>().InstancePerLifetimeScope();
        builder.RegisterType<PacketSwitchProvider>().As<IPacketSwitchProvider>().InstancePerLifetimeScope();
        builder.RegisterType<AutoSyncPatchCollector>().As<IAutoSyncPatchCollector>().InstancePerLifetimeScope();

        builder.RegisterType<PacketManager>().As<IPacketManager>().InstancePerLifetimeScope();

        builder.RegisterModule<ServiceModule>();
        builder.RegisterModule<ObjectManagerModule>();
        builder.RegisterModule<AutoSyncModule>();


        base.Load(builder);
    }
}
