using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace Rabbitminers.Stardew.WheresMyStuff;

internal class SearchMenu : IClickableMenu
{
    private const int RegionSubmitButton = 102;
    private const int RegionNamingBox = 104;

    private ClickableTextureComponent _submitButton;
    private TextBox _textBox;
    private Action<string> _onSubmit;

    public SearchMenu(Action<string> onSubmit)
    {
        _onSubmit = onSubmit;
        _textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor);

        _textBox.X = Game1.uiViewport.Width / 2 - 192;
        _textBox.Y = Game1.uiViewport.Height / 2;
        _textBox.textLimit = 999;
        _textBox.limitWidth = false;
        
        _textBox.Width = 256;
        _textBox.Height = 192;

        _textBox.Selected = true;
        
        _textBox.OnEnterPressed += _TrySubmit;

        _submitButton = new ClickableTextureComponent(
            bounds: new Rectangle(
                x: _textBox.X + _textBox.Width + 36, 
                y: Game1.uiViewport.Height / 2 - 8,
                width: 64, 
                height: 64
            ),
            texture: Game1.mouseCursors,
            sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
            scale: 1f
        );

        _submitButton.myID = RegionSubmitButton;
        _submitButton.leftNeighborID = RegionNamingBox;

        if (Game1.options.snappyMenus)
        {
            return;
        }
        
        populateClickableComponentList();
        snapToDefaultClickableComponent();
    }
    

    private void _DrawBackground(SpriteBatch batch)
    {
        batch.Draw(
            texture: Game1.fadeToBlackRect, 
            destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,  
            color: Color.Black * 0.75f
        );
    }

    public override void draw(SpriteBatch batch)
    {
        base.draw(batch);

        if (!Game1.options.showClearBackgrounds)
        {
            _DrawBackground(batch);
        }
        
        _textBox.Draw(batch);
        _submitButton.draw(batch);
        
        drawMouse(batch);
    }

    public override void receiveKeyPress(Keys key)
    {
        if (_textBox.Selected)
        {
            return;
        }

        if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
        {
            return;
        }
        
        base.receiveKeyPress(key);
    }

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);

        if (_submitButton == null)
        {
            return;
        }

        if (_submitButton.containsPoint(x, y))
        {
            var newScale = Math.Min(1.1f, _submitButton.scale + 0.05f);
            _submitButton.scale = newScale;
        }
        else
        {
            var newScale = Math.Max(1f, _submitButton.scale - 0.05f);
            _submitButton.scale = newScale;
        }
    }
    
    public override void receiveGamePadButton(Buttons button)
    {
        base.receiveGamePadButton(button);

        if (!_textBox.Selected)
        {
            return;
        }

        switch (button)
        {
            case Buttons.DPadUp:
            case Buttons.DPadDown:
            case Buttons.DPadLeft:
            case Buttons.DPadRight:
            case Buttons.LeftThumbstickLeft:
            case Buttons.LeftThumbstickUp:
            case Buttons.LeftThumbstickDown:
            case Buttons.LeftThumbstickRight:
                _textBox.Selected = false;
                break;
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        
        _textBox.Update();

        if (!_submitButton.containsPoint(x, y))
        {
            return;
        }
        
        _onSubmit(_textBox.Text);
        Game1.playSound("smallSelect");
    }

    private void _TrySubmit(TextBox sender)
    {
        var text = sender.Text;
        
        if (text.Length == 0) 
        {
            Game1.exitActiveMenu();
            return;
        }
        
        _onSubmit(text);
    }
}