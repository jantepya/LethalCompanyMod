
using BepInEx.Logging;
using GameNetcodeStuff;
using System;

namespace LethalChungus.Patches
{
    internal class TerminalKill
    {
        public static void Init(ManualLogSource logger)
        {
            try
            {
                TerminalApi.TerminalApi.AddCommand("fuckit", new TerminalApi.Classes.CommandInfo
                {
                    DisplayTextSupplier = EndGame
                });

                TerminalApi.TerminalApi.AddCommand("kill", new TerminalApi.Classes.CommandInfo
                {
                    DisplayTextSupplier = KillPlayer
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private static string EndGame()
        {
            var stats = StartOfRound.Instance.GetEndgameStatsInOrder();
            StartOfRound.Instance.FirePlayersAfterDeadlineClientRpc(stats, true);
            return "lol ok\n\n";
        }

        private static string KillPlayer()
        {
            if (StartOfRound.Instance.mapScreen == null || StartOfRound.Instance.mapScreen.targetedPlayer == null)
            {
                return "Couldn't find targeted player...";
            }

            PlayerControllerB player = StartOfRound.Instance.mapScreen.targetedPlayer;

            if (player.isPlayerDead)
            {
                return "already dead :p";
            }

            player.KillPlayer(UnityEngine.Vector3.zero);
            return "lol rip\n\n";
        }
    }
}
