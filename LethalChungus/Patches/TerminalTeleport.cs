using BepInEx.Logging;
using GameNetcodeStuff;
using System;
using System.Reflection;
using UnityEngine.AI;

namespace LethalChungus.Patches
{
    internal static class TerminalTeleport
    {
        private static readonly System.Random Random = new System.Random();

        public static void Init(ManualLogSource logger) 
        {
            try
            {
                TerminalApi.TerminalApi.AddCommand("teleport", new TerminalApi.Classes.CommandInfo
                {
                    DisplayTextSupplier = WrapLineEnding(HandleTeleportCommand)
                }, clearPreviousText: false);

                TerminalApi.TerminalApi.AddCommand("invteleport", new TerminalApi.Classes.CommandInfo
                {
                    DisplayTextSupplier = WrapLineEnding(HandleInverseTeleportKeyword)
                }, clearPreviousText: false);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private static Func<string> WrapLineEnding(System.Func<string> action)
        {
            return () => {
                try
                {
                    return action() + "\n\n";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            };
        }

        private static string HandleInverseTeleportKeyword()
        {
            if (!ShipTeleporter.hasBeenSpawnedThisSessionInverse)
            {
                return "uh oh need to buy an inverse teleporter";
            }
            ShipTeleporter teleporter = GetTeleporter(true);
            if (teleporter == null)
            {
                return "couldn't find inverse teleporter :(";
            }

            PlayerControllerB player = StartOfRound.Instance.mapScreen.targetedPlayer;

            if (player.deadBody != null)
            {
                return "nah they dead";
            }

            int playerId = (int)player.playerClientId;
            UnityEngine.Vector3 position = GetRandomPosition();

            MethodInfo dynMethod = teleporter.GetType().GetMethod("TeleportPlayerOutWithInverseTeleporter", BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod.Invoke(teleporter, [playerId, position]);

            teleporter.TeleportPlayerOutServerRpc(playerId, position);

            return "good luck :)";
        }

        private static UnityEngine.Vector3 GetRandomPosition()
        {
            UnityEngine.Vector3 position3 = RoundManager.Instance.insideAINodes[Random.Next(0, RoundManager.Instance.insideAINodes.Length)].transform.position;
            return RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(position3, 10f, default(NavMeshHit), Random);
        }

        private static string HandleTeleportCommand()
        {
            if (!ShipTeleporter.hasBeenSpawnedThisSession)
            {
                return "uh oh need to buy a teleporter";
            }
            ShipTeleporter teleporter = GetTeleporter(false);
            if (teleporter == null)
            {
                return "couldn't find teleporter :(";
            }

            if (teleporter.cooldownTime > 0f)
            {
                return $"Gotta wait: {(int)teleporter.cooldownTime + 1} secs";
            }
            teleporter.PressTeleportButtonOnLocalClient();

            return "ok fine";
        }

        private static ShipTeleporter GetTeleporter(bool isInverseTeleport)
        {
            foreach (ShipTeleporter teleporter in UnityEngine.Object.FindObjectsOfType<ShipTeleporter>())
            {
                if (teleporter.isInverseTeleporter == isInverseTeleport)
                {
                    return teleporter;
                }
            }
            return null;
        }
    }
}
