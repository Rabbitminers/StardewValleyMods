using StardewModdingAPI;
using StardewValley.Objects;

namespace Rabbitminers.Stardew.WheresMyStuff.Core;

internal class ChestHighlighterState : IDisposable
{
    private List<Chest> _highlightedChests = new();
    
    private readonly TickTimer _timer;
    private readonly IMonitor _monitor;

    public ChestHighlighterState(IModHelper helper, IMonitor monitor)
    {
        _timer = new TickTimer(helper);
        _monitor = monitor;
        
        _timer.OnComplete += ClearChests;
    }

    public IReadOnlyList<Chest> HighlightedChests => _highlightedChests;
    
    public void HighlightChests(List<Chest> chests)
    {
        if (_timer == null)
        {
            _monitor.Log("tried to highlight chests before setup", LogLevel.Warn);
            return;
        }

        if (chests.Count == 0)
        {
            return;
        }
        
        _highlightedChests = chests;
        _timer.Start(durationTicks: 60 * 5);
    }

    public void ClearChests()
    {
        if (_timer == null)
        {
            _monitor.Log("tried to clear chest highlights before setup", LogLevel.Warn);
            return;
        }
        
        _timer.Stop();
        _highlightedChests.Clear();
    }

    public void Dispose() => _timer?.Dispose();
    
}