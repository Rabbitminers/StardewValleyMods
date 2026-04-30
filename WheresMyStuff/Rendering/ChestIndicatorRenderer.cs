using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace Rabbitminers.Stardew.WheresMyStuff;

internal class ChestIndicatorRenderer : IDisposable
{
    private const string IndicatorPath = "Assets/indicator.png";
    
    private readonly IModHelper _helper;
    private Texture2D? _indicatorTexture;

    public ChestIndicatorRenderer(IModHelper helper)
    {
        _helper = helper;

        _helper.Events.GameLoop.GameLaunched += _Setup;
    }

    private void _Setup(object? _,  GameLaunchedEventArgs eventArgs)
    {
        if (_indicatorTexture != null)
        {
            throw new Exception("Chest indicator setup called twice");
        }
        
        try
        {
            _indicatorTexture = _helper.ModContent.Load<Texture2D>(IndicatorPath);            
        } 
        catch
        {
            throw new Exception("Could not find indicator texture");
        }
    }

    private void _DrawIndicator(Chest chest, RenderedWorldEventArgs args)
    {
        if (_indicatorTexture == null)
        {
            return;
        }
        
        var globalX = chest.TileLocation.X * 64 + 32.0; 
        var globalY =  chest.TileLocation.Y * 64 - 40.0;

        var globalPosition = new Vector2((float) globalX, (float) globalY);
        var local = Game1.GlobalToLocal(Game1.viewport, globalPosition);

        var originX = _indicatorTexture.Width / 2;
        var originY = _indicatorTexture.Height;
        
        var origin = new Vector2(originX, originY);
        
        args.SpriteBatch.Draw(
            _indicatorTexture, 
            position: local, 
            sourceRectangle: null, 
            color: Color.White, 
            rotation: 0.0f, 
            origin, 
            scale: 4f, 
            effects: SpriteEffects.None, 
            layerDepth: 1f
        );
    }

    public void DrawIndicators(IReadOnlyList<Chest> chests, RenderedWorldEventArgs args)
    {
        foreach (var chest in chests)
        {
            _DrawIndicator(chest, args);
        }
    }

    public void Dispose()
    {
        _indicatorTexture?.Dispose();
        
        _helper.Events.GameLoop.GameLaunched -= _Setup;
    }
}