using BepInEx;
using LethalChungus.Patches;

namespace LethalChungus
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi")]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class LethalChungus : BaseUnityPlugin
    {
        public static LethalChungus Instance { get; private set; } = null!;

        private void Awake()
        {
            Instance = this;

            Logger.LogDebug("Patching...");

            TerminalTeleport.Init(Logger);
            TerminalKill.Init(Logger);
            CustomOutsideObjects.Init(Logger, Info);

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }
    }
}
