using HarmonyLib;
using Rabbitminers.Stardew.PacketOfLostCrops.utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;

namespace Rabbitminers.Stardew.PacketOfLostCrops;

/// <summary>
/// Attach far more in depth logging into network disconnects, this is only intended for debugging
/// </summary>
internal class LoggingPatches : BasePatcher
{
    private static IMonitor? _monitor;
    private static byte? _lastProcessedPacketId;
    
    public LoggingPatches(IMonitor monitor) {
        LoggingPatches._monitor = monitor;
    }
    
    public override void Apply(Harmony harmony, IMonitor monitor)
    {
        monitor.Log("Applying Network Diagnostic Patches...");

        harmony.Patch(
            original: AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.processIncomingMessage)),
            prefix: new HarmonyMethod(typeof(LoggingPatches), nameof(ProcessIncomingMessage_Prefix))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.LogDisconnect)),
            prefix: new HarmonyMethod(typeof(LoggingPatches), nameof(LogDisconnect_Prefix))
        );
    }
    
    // This runs EVERY time a packet is received, right before the vanilla code processes it.
    private static void ProcessIncomingMessage_Prefix(IncomingMessage msg)
    {
        try
        {
            _lastProcessedPacketId = msg.MessageType;
        }
        catch (Exception)
        {
            // Catch silently so we don't accidentally cause a crash while logging
        }
    }
    
// This intercepts the vanilla disconnect routine
    private static void LogDisconnect_Prefix(Multiplayer.DisconnectType disconnectType)
    {
        if (_monitor == null)
        {
            return; // because what's the fucking point
        }
        
        _monitor.Log("==================================================", LogLevel.Error);
        _monitor.Log($"[NETWORK FAILURE DETECTED]", LogLevel.Error);
        _monitor.Log($"Reason: {disconnectType.ToString()}", LogLevel.Error);
        _monitor.Log($"Last Packet Type Processed: {_lastProcessedPacketId}", LogLevel.Error);
            
        // Capture the call chain to see exactly what triggered the drop
        var fullStackTrace = Environment.StackTrace;
        _monitor.Log($"\n--- CALLSITE STACK TRACE --- \n{fullStackTrace}", LogLevel.Warn);
        _monitor.Log("==================================================", LogLevel.Error);
    }
}