using Rabbitminers.Stardew.PacketOfLostCrops.utilities;
using StardewModdingAPI;

namespace Rabbitminers.Stardew.PacketOfLostCrops;

internal class ModEntry : Mod
{
    /*********
     ** Public methods
     *********/
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        Monitor.Log("Setting up", LogLevel.Info);
        
        HarmonyPatcher.Apply(this, new LoggingPatches(monitor: Monitor));
    }
}