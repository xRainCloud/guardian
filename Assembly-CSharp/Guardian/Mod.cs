﻿using Guardian.AntiAbuse;
using Guardian.Features.Commands;
using Guardian.Features.Properties;
using Guardian.Networking;
using Guardian.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Guardian
{
    class Mod : MonoBehaviour
    {
        public static Mod Instance;
        public static string Build = "02162021";
        public static string RootDir = Application.dataPath + "\\..";
        public static string HostWhitelistPath = RootDir + "\\Hosts.txt";
        public static CommandManager Commands = new CommandManager();
        public static PropertyManager Properties = new PropertyManager();
        public static List<string> HostWhitelist = new List<string>();
        public static Regex BlacklistedTags = new Regex("<\\/?(size|material|quad)([^>]*)?>", RegexOptions.IgnoreCase);
        public static Logger Logger = new Logger();

        private static bool Initialized = false;
        private static bool FirstJoin = true;

        public List<int> Muted = new List<int>();
        public bool IsMultiMap;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (!Initialized)
            {
                // Check for an update before doing anything
                StartCoroutine(CheckForUpdate());

                // Host whitelist (for skins)
                if (!File.Exists(HostWhitelistPath))
                {
                    HostWhitelist.Add("i.imgur.com");
                    HostWhitelist.Add("imgur.com");
                    HostWhitelist.Add("cdn.discordapp.com");
                    HostWhitelist.Add("cdn.discord.com");
                    HostWhitelist.Add("media.discordapp.net");
                    HostWhitelist.Add("i.gyazo.com");
                    File.WriteAllLines(HostWhitelistPath, HostWhitelist.ToArray());
                }
                LoadSkinHostWhitelist();

                // Auto-load RC name and guild (if possible)
                FengGameManagerMKII.NameField = PlayerPrefs.GetString("name", string.Empty);
                if (FengGameManagerMKII.NameField.Uncolored().Length == 0)
                {
                    FengGameManagerMKII.NameField = LoginFengKAI.Player.Name;
                }
                LoginFengKAI.Player.Guild = PlayerPrefs.GetString("guildname", string.Empty);

                // Load various features
                Commands.Load();
                Properties.Load();

                // Print out debug information
                Logger.Info($"Installed Version: {Build}");
                Logger.Info($"Unity Version: {Application.unityVersion}");
                Logger.Info($"OS: {SystemInfo.operatingSystem}");
                Logger.Info($"Platform: {Application.platform}");

                // Property whitelist
                NetworkPatches.PropertyWhitelist.Add("sender");
                NetworkPatches.PropertyWhitelist.Add("GuardianMod");
                foreach (FieldInfo field in typeof(PhotonPlayerProperty).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    NetworkPatches.PropertyWhitelist.Add((string)field.GetValue(null));
                }

                Initialized = true;

                DiscordHelper.StartTime = GameHelper.CurrentTimeMillis();

            }

            base.gameObject.AddComponent<UI.ModUI>();
            base.gameObject.AddComponent<MicEF>();

            DiscordHelper.SetPresence(new Discord.Activity
            {
                Details = $"Staring at the main menu...",
            });
        }

        private IEnumerator CheckForUpdate()
        {
            using (WWW www = new WWW("http://lewd.cf/GUARDIAN_BUILD.TXT?t=" + GameHelper.CurrentTimeMillis()))
            {
                yield return www;

                Logger.Info("Latest Version: " + www.text);

                if (!www.text.Split('\n')[0].Equals(Build))
                {
                    Logger.Error("You are running an outdated build, please update!");
                    Logger.Error("https://tiny.cc/GuardianMod".WithColor("0099FF"));

                    try
                    {
                        GameObject.Find("VERSION").GetComponent<UILabel>().text = "[FF0000]Mod is outdated![-] Please download the latest build from [0099FF]https://tiny.cc/GuardianMod[-]!";
                    }
                    catch { }
                }
            }
        }

        public static void LoadSkinHostWhitelist()
        {
            HostWhitelist = new List<string>(File.ReadAllLines(HostWhitelistPath));
        }

        // Attempts to fix some dumb bugs that occur when you alt-tab
        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && IN_GAME_MAIN_CAMERA.Gametype != GameType.Stop)
            {
                // Minimap turning white
                if (Minimap.Instance != null)
                {
                    Minimap.WaitAndTryRecaptureInstance(0.1f);
                }

                // TPS crosshair ending up where it shouldn't
                if (IN_GAME_MAIN_CAMERA.CameraMode == CAMERA_TYPE.TPS)
                {
                    Screen.lockCursor = false;
                    Screen.lockCursor = true;
                }
            }
        }

        void OnLevelWasLoaded(int level)
        {
            if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
            {
                string difficulty = "Training";
                switch (IN_GAME_MAIN_CAMERA.Difficulty)
                {
                    case 0:
                        difficulty = "Normal";
                        break;
                    case 1:
                        difficulty = "Hard";
                        break;
                    case 2:
                        difficulty = "Abnormal";
                        break;
                }

                DiscordHelper.SetPresence(new Discord.Activity
                {
                    Details = $"Playing in singleplayer.",
                    State = $"{FengGameManagerMKII.Level.Name} / {difficulty}"
                });
            }

            if (FirstJoin)
            {
                FirstJoin = false;
                string joinMessage = Properties.JoinMessage.Value.Colored();
                if (joinMessage.Uncolored().Length <= 0)
                {
                    joinMessage = Properties.JoinMessage.Value;
                }
                if (joinMessage.Length > 0)
                {
                    Commands.Find("say").Execute(InRoomChat.Instance, joinMessage.Split(' '));
                }
            }
        }

        void OnPhotonPlayerConnected(PhotonPlayer player)
        {
            Logger.Info($"[{player.Id}] ".WithColor("FFCC00") + GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]).Colored() + " connected.".WithColor("00FF00"));
        }

        void OnPhotonPlayerDisconnected(PhotonPlayer player)
        {
            Logger.Info($"[{player.Id}] ".WithColor("FFCC00") + GExtensions.AsString(player.customProperties[PhotonPlayerProperty.Name]).Colored() + " disconnected.".WithColor("FF0000"));
        }

        void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
        {
            NetworkPatches.OnPlayerPropertyModification(playerAndUpdatedProps);

            PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
            ExitGames.Client.Photon.Hashtable properties = playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;

            // Neko Mod detection
            if (properties.ContainsValue("N_user") || properties.ContainsValue("N_owner"))
            {
                player.isNeko = true;
                player.isNekoUser = properties.ContainsValue("N_user");
                player.isNekoOwner = properties.ContainsValue("N_owner");
            }

            // FoxMod detection
            if (properties.ContainsKey("FoxMod"))
            {
                player.isFoxMod = true;
            }
        }

        void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            NetworkPatches.OnRoomPropertyModification(propertiesThatChanged);

            if (!FirstJoin)
            {
                PhotonPlayer sender = null;
                if (propertiesThatChanged.ContainsKey("sender") && propertiesThatChanged["sender"] is PhotonPlayer)
                {
                    sender = (PhotonPlayer)propertiesThatChanged["sender"];
                }

                if (sender == null || sender.isMasterClient)
                {
                    if (propertiesThatChanged.ContainsKey("Map") && propertiesThatChanged["Map"] is string && IsMultiMap)
                    {
                        LevelInfo levelInfo = LevelInfo.GetInfo((string)propertiesThatChanged["Map"]);
                        if (levelInfo != null)
                        {
                            FengGameManagerMKII.Level = levelInfo;
                        }
                    }

                    if (propertiesThatChanged.ContainsKey("Lighting") && propertiesThatChanged["Lighting"] is string)
                    {
                        if (GExtensions.TryParseEnum((string)propertiesThatChanged["Lighting"], out DayLight time))
                        {
                            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setDayLight(time);
                        }
                    }
                }
            }
        }

        void OnJoinedLobby()
        {
            PhotonNetwork.playerName = LoginFengKAI.Player.Name;

            DiscordHelper.SetPresence(new Discord.Activity
            {
                Details = "Searching for a room...",
                State = $"Region: {NetworkHelper.GetRegionCode()}"
            });
        }

        void OnJoinedRoom()
        {
            IsMultiMap = PhotonNetwork.room.name.Split('`')[1].StartsWith("Multi-Map");
            Muted = new List<int>();
            FirstJoin = true;

            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { "GuardianMod", 1 },
            });

            string[] roomInfo = PhotonNetwork.room.name.Split('`');
            if (roomInfo.Length > 6)
            {

                DiscordHelper.SetPresence(new Discord.Activity
                {
                    Details = $"Playing in {(roomInfo[5].Length == 0 ? string.Empty : "[PWD]")} {roomInfo[0].Uncolored()}",
                    State = $"({NetworkHelper.GetRegionCode()}) {roomInfo[1]} / {roomInfo[2].ToUpper()}"
                });
            }
        }

        void OnLeftRoom()
        {
            DiscordHelper.SetPresence(new Discord.Activity
            {
                Details = "Idle..."
            });
        }

        void OnConnectionFail(DisconnectCause cause)
        {
            Logger.Warn($"OnConnectionFail ({cause})");
        }

        void OnPhotonRoomJoinFailed(object[] codeAndMsg)
        {
            Logger.Error($"OnPhotonRoomJoinFailed ({codeAndMsg[0]} : {codeAndMsg[1]})");
        }

        void OnApplicationQuit()
        {
            Properties.Save();

            DiscordHelper.Shutdown();
        }

        void Update()
        {
            DiscordHelper.RunCallbacks();
        }
    }
}