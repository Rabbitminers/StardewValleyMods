using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Rabbitminers.Stardew.WheresMyStuff.Core;

public sealed class TickTimer : IDisposable
{
    private readonly IModHelper _helper;

    private int _ticksRemaining;
    private bool _isRunning;

    public bool IsRunning => _isRunning;
    public int TicksRemaining => _ticksRemaining;

    public event Action? OnComplete;

    public TickTimer(IModHelper helper)
    {
        if (helper == null)
        {
            throw new ArgumentNullException(nameof(helper));
        }
        
        _helper = helper;
    }

    public void Start(int durationTicks)
    {
        if (durationTicks <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationTicks));   
        }

        _ticksRemaining = durationTicks;

        if (_isRunning)
        {
            return;
        }

        _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        _isRunning = true;
    }

    public void Restart(int durationTicks)
    {
        Stop();
        
        Start(durationTicks);
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            return;
        }

        _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
        _isRunning = false;
    }

    public void Reset(int durationTicks)
    {
        _ticksRemaining = durationTicks;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!_isRunning)
        {
            return;
        }


        _ticksRemaining--;

        if (_ticksRemaining > 0)
        {
            return;
        }

        Stop();
        OnComplete?.Invoke();
    }

    public void Dispose()
    {
        Stop();
    }
}