﻿using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WorldsAdriftReborn.HelperClasses;
using UnityEngine;
using System.Linq;

namespace WorldsAdriftReborn.Config
{
    internal static class ModSettings
    {
        public static ConfigFile modConfig { get; set; }
        public static ConfigEntry<string> steamUserId { get; set; }
        public static ConfigEntry<string> steamAppId { get; set; }
        public static ConfigEntry<string> steamBranchName { get; set; }
        public static ConfigEntry<string> restServerUrl { get; set; }
        public static ConfigEntry<string> restServerDeploymentUrl { get; set; }
        public static ConfigEntry<string> NTPServerUrl { get; set; }
        public static ConfigEntry<string> localAssetPath { get; set; }
        public static ConfigEntry<string> gameServerHost { get; set; }

        public static void InitConfig()
        {
            string warConfigDirectory = $"{Directory.GetCurrentDirectory()}\\BepInEx\\plugins\\WAR\\Config\\WARConfig.json";
            bool warDirectoryExists = Directory.Exists(warConfigDirectory);
            
            if (!warDirectoryExists)
            {
                string errorMsg = "BepInEx file structure not setup correctly, expected BepInEx\\plugins\\WAR\\Config\\WARConfig.json";
                Debug.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            
            Debug.Log($"WAR config directory -> {warConfigDirectory}");
            JObject warJson;
            using (StreamReader file = File.OpenText(warConfigDirectory))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    warJson = (JObject)JToken.ReadFrom(reader);
                }
            }

            var warConfig = new WARConfiguration(warJson);
            Debug.Log($"Configuration => {warConfig}");
            modConfig = new ConfigFile(Paths.ConfigPath + "\\WorldsAdriftReborn.cfg", true);

            steamUserId = modConfig.Bind(WARConstants.Steam,
                                                    WARConstants.SteamUserId,
                                                    warConfig.SteamConfig.SteamUserId,
                                                    "Sets the Steam User ID that the game uses internally. Its not important for the functionality to set this to a specific value.");
            
            steamAppId = modConfig.Bind(WARConstants.Steam,
                                                    WARConstants.SteamAppId,
                                                    warConfig.SteamConfig.SteamAppId,
                                                    "Sets the Steam App ID that the game uses internally. Its not important for the functionality to set this to a specific value.");
            
            steamBranchName = modConfig.Bind(WARConstants.Steam,
                                                    WARConstants.SteamBranchName,
                                                    warConfig.SteamConfig.SteamBranchName,
                                                    "Sets the Steam Branch name that the game uses internally. Its not important for the functionality to set this to a specific value.");

            restServerUrl = modConfig.Bind(WARConstants.REST,
                                                    WARConstants.RESTServerUrl,
                                                    warConfig.RESTConfig.ServerUrl,
                                                    "Sets the URL for the REST server that the game queries once the main menu is reached.");
            
            restServerDeploymentUrl = modConfig.Bind(WARConstants.REST,
                                                    WARConstants.RESTServerDeploymentUrl,
                                                    warConfig.RESTConfig.ServerDeploymentUrl,
                                                    "Sets the URL for the REST server that the game queries once the main menu is reached. It is the endpoint where server status informations are retrieved from.");

            NTPServerUrl = modConfig.Bind(WARConstants.NTP,
                                                    WARConstants.NTPServerUrl,
                                                    warConfig.GeneralConfig.NtpServerUrl,
                                                    "Set the NTP server that should be used to synchronize time.");

            localAssetPath = modConfig.Bind(WARConstants.AssetLoader,
                                                    WARConstants.AssetLoaderFilePath,
                                                    warConfig.GeneralConfig.AssetLoaderFilePath,
                                                    "The intermediate part of the Asset folder path. Gets 'unity\\' appended. In some cases the game fails to determine the intermediate path so you can set it here or leave it blank.");

            gameServerHost = modConfig.Bind(WARConstants.GameServer,
                                                    WARConstants.GameServerHost,
                                                    warConfig.GeneralConfig.GameServerHost,
                                                    "The hostname or address of the game server.");
        }
    }
}
