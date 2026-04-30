using System.Runtime.CompilerServices;
using Rabbitminers.Stardew.WheresMyStuff.Core;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Rabbitminers.Stardew.WheresMyStuff;

internal class ModEntry : Mod
{
    private ChestIndicatorRenderer? _renderer;
    private ChestHighlighterState? _state;
    
    public override void Entry(IModHelper helper)
    {
        Monitor.Log("Setting Up", LogLevel.Info);
        
        _renderer = new ChestIndicatorRenderer(helper: Helper);
        _state = new ChestHighlighterState(helper: Helper, monitor: Monitor);
        
        helper.Events.Input.ButtonPressed += _onButtonPressed;
        helper.Events.Display.RenderedWorld += _OnRenderedWorld;
    }

    private void _onButtonPressed(object? sender, ButtonPressedEventArgs args)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        switch (args.Button)
        {
            case SButton.Z:
                _SearchFromIngameItem();
                break;
            case SButton.F3:
                _SearchFromText();
                break;
        }
        
    }

    private void _SearchFromIngameItem()
    {
        if (_state == null)
        {
            Monitor.Log("tried to render highlighted chests but state was null", LogLevel.Warn);
            return;
        }
        
        var chests = ItemLocator.FindIngameItem(Helper);
        
        _state.HighlightChests(chests);
    }

    private void _SearchFromText()
    {
        if (Game1.activeClickableMenu != null)
        {
            return;
        }
        
        Game1.activeClickableMenu = new SearchMenu(_OnSearchSubmitted);
    }

    private void _OnSearchSubmitted(string text)
    {
        Game1.activeClickableMenu = null;

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        
        if (_state == null)
        {
            Monitor.Log("tried to render highlighted chests but state was null", LogLevel.Warn);
            return;
        }
        
        var chests = ItemLocator.FindItemByName(text, fuzzy: true);
        _state.HighlightChests(chests);
    }

    private void _OnRenderedWorld(object? _, RenderedWorldEventArgs renderEvent)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (_state == null)
        {
            Monitor.Log("tried to render highlighted chests but state was null", LogLevel.Warn);
            return;
        }

        if (_renderer == null)
        {
            Monitor.Log("tried to render chests but renderer was null", LogLevel.Warn);
            return;
        }

        var chests = _state.HighlightedChests;
        
        _renderer.DrawIndicators(chests, renderEvent);
    }
}