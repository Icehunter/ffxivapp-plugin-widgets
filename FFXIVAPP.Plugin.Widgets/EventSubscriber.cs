﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSubscriber.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   EventSubscriber.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FFXIVAPP.Plugin.Widgets {
    using System;
    using System.Collections.Concurrent;

    using FFXIVAPP.Common.Core.Constant;
    using FFXIVAPP.Common.Models;
    using FFXIVAPP.Common.Utilities;
    using FFXIVAPP.IPluginInterface.Events;
    using FFXIVAPP.Plugin.Widgets.Properties;
    using FFXIVAPP.Plugin.Widgets.ViewModels;
    using FFXIVAPP.Plugin.Widgets.Windows;

    using NLog;

    using Sharlayan.Core;

    public static class EventSubscriber {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Subscribe() {
            Plugin.PHost.ConstantsUpdated += OnConstantsUpdated;

            // Plugin.PHost.NewChatLogEntry += OnNewChatLogEntry;
            // Plugin.PHost.NewMonsterEntries += OnNewMonsterEntries;
            // Plugin.PHost.NewNPCEntries += OnNewNPCEntries;
            Plugin.PHost.PCItemsUpdated += OnPCItemsUpdated;

            // Plugin.PHost.NewPlayerEntity += OnNewPlayerEntity;
            Plugin.PHost.TargetInfoUpdated += OnTargetInfoUpdated;

            // Plugin.PHost.NewPartyEntries += OnNewPartyEntries;
        }

        public static void UnSubscribe() {
            Plugin.PHost.ConstantsUpdated -= OnConstantsUpdated;

            // Plugin.PHost.NewChatLogEntry -= OnNewChatLogEntry;
            // Plugin.PHost.NewMonsterEntries -= OnNewMonsterEntries;
            // Plugin.PHost.NewNPCEntries -= OnNewNPCEntries;
            Plugin.PHost.PCItemsUpdated -= OnPCItemsUpdated;

            // Plugin.PHost.NewPlayerEntity -= OnNewPlayerEntity;
            Plugin.PHost.TargetInfoUpdated -= OnTargetInfoUpdated;

            // Plugin.PHost.NewPartyEntries -= OnNewPartyEntries;
        }

        private static void OnConstantsUpdated(object sender, ConstantsEntityEvent constantsEntityEvent) {
            // delegate event from constants, not required to subsribe, but recommended as it gives you app settings
            if (sender == null) {
                return;
            }

            ConstantsEntity constantsEntity = constantsEntityEvent.ConstantsEntity;
            Constants.AutoTranslate = constantsEntity.AutoTranslate;
            Constants.ChatCodes = constantsEntity.ChatCodes;
            Constants.Colors = constantsEntity.Colors;
            Constants.CultureInfo = constantsEntity.CultureInfo;
            Constants.CharacterName = constantsEntity.CharacterName;
            Constants.ServerName = constantsEntity.ServerName;
            Constants.GameLanguage = constantsEntity.GameLanguage;
            Constants.EnableHelpLabels = constantsEntity.EnableHelpLabels;
            Constants.Theme = constantsEntity.Theme;
            PluginViewModel.Instance.EnableHelpLabels = Constants.EnableHelpLabels;
        }

        // private static void OnNewChatLogEntry(object sender, ChatLogEntryEvent chatLogEntryEvent)
        // {
        // // delegate event from chat log, not required to subsribe
        // // this updates 100 times a second and only sends a line when it gets a new one
        // if (sender == null)
        // {
        // return;
        // }
        // var chatLogEntry = chatLogEntryEvent.ChatLogEntry;
        // try
        // {
        // if (chatLogEntry.Line.ToLower()
        // .StartsWith("com:"))
        // {
        // LogPublisher.HandleCommands(chatLogEntry);
        // }
        // LogPublisher.Process(chatLogEntry);
        // }
        // catch (Exception ex)
        // {
        // }
        // }

        // private static void OnNewMonsterEntries(object sender, ActorEntitiesEvent actorEntitiesEvent)
        // {
        // // delegate event from monster entities from ram, not required to subsribe
        // // this updates 10x a second and only sends data if the items are found in ram
        // // currently there no change/new/removed event handling (looking into it)
        // if (sender == null)
        // {
        // return;
        // }
        // var monsterEntities = actorEntitiesEvent.ActorEntities;
        // }

        // private static void OnNewNPCEntries(object sender, ActorEntitiesEvent actorEntitiesEvent)
        // {
        // // delegate event from npc entities from ram, not required to subsribe
        // // this list includes anything that is not a player or monster
        // // this updates 10x a second and only sends data if the items are found in ram
        // // currently there no change/new/removed event handling (looking into it)
        // if (sender == null)
        // {
        // return;
        // }
        // var npcEntities = actorEntitiesEvent.ActorEntities;
        // }
        private static void OnPCItemsUpdated(object sender, ActorItemsEvent actorEntitiesEvent) {
            // delegate event from player entities from ram, not required to subsribe
            // this updates 10x a second and only sends data if the items are found in ram
            // currently there no change/new/removed event handling (looking into it)
            if (sender == null) {
                return;
            }

            ConcurrentDictionary<uint, ActorItem> pcEntities = actorEntitiesEvent.ActorItems;
            XIVInfoViewModel.Instance.CurrentPCs = pcEntities;
        }

        // private static void OnNewPlayerEntity(object sender, PlayerEntityEvent playerEntityEvent)
        // {
        // // delegate event from player info from ram, not required to subsribe
        // // this is for YOU and includes all your stats and your agro list with hate values as %
        // // this updates 10x a second and only sends data when the newly read data is differen than what was previously sent
        // if (sender == null)
        // {
        // return;
        // }
        // var playerEntity = playerEntityEvent.PlayerEntity;
        // }
        private static void OnTargetInfoUpdated(object sender, TargetInfoEvent targetInfoEvent) {
            // delegate event from target info from ram, not required to subsribe
            // this includes the full entities for current, previous, mouseover, focus targets (if 0+ are found)
            // it also includes a list of upto 16 targets that currently have hate on the currently targeted monster
            // these hate values are realtime and change based on the action used
            // this updates 10x a second and only sends data when the newly read data is differen than what was previously sent
            if (sender == null) {
                return;
            }

            TargetInfo targetInfo = targetInfoEvent.TargetInfo;
            var emptyEntity = new TargetInfo();

            // assign empty entity
            EnmityWidgetViewModel.Instance.TargetEntity = emptyEntity;
            FocusTargetWidgetViewModel.Instance.TargetEntity = emptyEntity;
            CurrentTargetWidgetViewModel.Instance.TargetEntity = emptyEntity;

            // assign default current/focus target info
            EnmityWidgetViewModel.Instance.EnmityTargetIsValid = false;
            EnmityWidgetViewModel.Instance.EnmityTargetHPPercent = 0;
            EnmityWidgetViewModel.Instance.EnmityTargetDistance = 0;
            FocusTargetWidgetViewModel.Instance.FocusTargetIsValid = false;
            FocusTargetWidgetViewModel.Instance.FocusTargetHPPercent = 0;
            FocusTargetWidgetViewModel.Instance.FocusTargetDistance = 0;
            CurrentTargetWidgetViewModel.Instance.CurrentTargetIsValid = false;
            CurrentTargetWidgetViewModel.Instance.CurrentTargetHPPercent = 0;
            CurrentTargetWidgetViewModel.Instance.CurrentTargetDistance = 0;

            if (ActorItem.CurrentUser != null) {
                XIVInfoViewModel.Instance.CurrentUser = ActorItem.CurrentUser;
            }

            // if valid assign actual current target info
            if (targetInfo.CurrentTarget != null && targetInfo.CurrentTarget.IsValid && Settings.Default.ShowEnmityWidgetOnLoad) {
                EnmityWidgetViewModel.Instance.TargetEntity = targetInfo;
                EnmityWidgetViewModel.Instance.EnmityTargetIsValid = true;
                EnmityWidgetViewModel.Instance.EnmityTargetHPPercent = targetInfo.CurrentTarget.HPPercent;

                try {
                    EnmityWidgetViewModel.Instance.EnmityTargetDistance = XIVInfoViewModel.Instance.CurrentUser.GetDistanceTo(targetInfo.CurrentTarget);
                }
                catch (Exception ex) {
                    Logging.Log(Logger, new LogItem(ex, true));
                }
            }

            // if valid assign actual current target info
            if (targetInfo.CurrentTarget != null && targetInfo.CurrentTarget.IsValid && Settings.Default.ShowCurrentTargetWidgetOnLoad) {
                CurrentTargetWidgetViewModel.Instance.TargetEntity = targetInfo;
                CurrentTargetWidgetViewModel.Instance.CurrentTargetIsValid = true;
                CurrentTargetWidgetViewModel.Instance.CurrentTargetHPPercent = targetInfo.CurrentTarget.HPPercent;

                try {
                    CurrentTargetWidgetViewModel.Instance.CurrentTargetDistance = XIVInfoViewModel.Instance.CurrentUser.GetDistanceTo(targetInfo.CurrentTarget);
                }
                catch (Exception ex) {
                    Logging.Log(Logger, new LogItem(ex, true));
                }
            }

            // if valid assign actual focus target info
            if (targetInfo.FocusTarget != null && targetInfo.FocusTarget.IsValid && Settings.Default.ShowFocusTargetWidgetOnLoad) {
                FocusTargetWidgetViewModel.Instance.TargetEntity = targetInfo;
                FocusTargetWidgetViewModel.Instance.FocusTargetIsValid = true;
                FocusTargetWidgetViewModel.Instance.FocusTargetHPPercent = targetInfo.FocusTarget.HPPercent;

                try {
                    FocusTargetWidgetViewModel.Instance.FocusTargetDistance = XIVInfoViewModel.Instance.CurrentUser.GetDistanceTo(targetInfo.FocusTarget);
                }
                catch (Exception ex) {
                    Logging.Log(Logger, new LogItem(ex, true));
                }
            }
        }

        // private static void OnNewPartyEntries(object sender, PartyEntitiesEvent partyEntitiesEvent)
        // {
        // // delegate event that shows current party basic info
        // if (sender == null)
        // {
        // return;
        // }
        // var partyEntities = partyEntitiesEvent.PartyEntities;
        // }
    }
}