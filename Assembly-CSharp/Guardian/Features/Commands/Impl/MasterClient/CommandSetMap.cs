﻿using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.MasterClient
{
    class CommandSetMap : Command
    {
        public CommandSetMap() : base("setmap", new string[] { "map" }, "<name>", true) { }

        public override void Execute(InRoomChat irc, string[] args)
        {
            if (!Mod.Instance.IsMultiMap)
            {
                irc.AddLine("This is not a Multi-Map room!".WithColor("FF0000"));
                return;
            }
            if (args.Length > 0)
            {
                LevelInfo levelInfo = LevelInfo.GetInfo(string.Join(" ", args));
                if (levelInfo != null)
                {
                    PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                    {
                        { "Map", levelInfo.Name }
                    });

                    ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable()
                    {
                        { PhotonPlayerProperty.IsTitan, 1 },
                    };
                    foreach (PhotonPlayer player in PhotonNetwork.playerList)
                    {
                        player.SetCustomProperties(properties);
                    }

                    FengGameManagerMKII.Instance.RestartGame();

                    GameHelper.Broadcast($"The map in play is now {levelInfo.Name}!");
                }
            }
            else
            {
                irc.AddLine("Available Maps:".WithColor("AAFF00"));

                foreach (LevelInfo level in LevelInfo.Levels)
                {
                    irc.AddLine("> ".WithColor("00FF00").AsBold() + level.Name);
                }
            }
        }
    }
}
