using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewObject = StardewValley.Object;

namespace Rabbitminers.Stardew.WheresMyStuff.Core;

internal class ItemLocator
{
    private static bool _IsChestWithItem(StardewObject tile, string name, bool fuzzy = false)
    {
        if (tile is not Chest chest)
        {
            return false;
        }

        if (fuzzy)
        {
            return chest.Items.Any(item => Matching.IsFuzzyMatch(item.Name, name));
        }
        
        
        return chest.Items.Any(item => item.Name == name);   
    }

    private static Item? _ItemUnderCursor(GameMenu menu)
    {
        var currentPage = menu.GetCurrentPage();

        return currentPage switch
        {
            InventoryPage inventory => inventory.hoveredItem,
            ItemGrabMenu inventory => inventory.hoveredItem,
            MenuWithInventory inventory => inventory.hoveredItem,
            _ => null
        };
    }

    private static Item? _IngameItem(IModHelper helper)
    {
        if (Game1.activeClickableMenu is GameMenu menu)
        {
            return _ItemUnderCursor(menu);
        }

        return Game1.player.CurrentItem;
    }
    
    public static List<Chest> FindItemByName(string name, bool fuzzy = false)
    {
        var objects = Game1.currentLocation.Objects;
        
        var chests = objects.Values
            .Where(tile => _IsChestWithItem(tile, name, fuzzy))
            .OfType<Chest>()
            .ToList();
        
        return chests;
    }

    public static List<Chest> FindIngameItem(IModHelper helper)
    {
        var item = _IngameItem(helper);

        if (item == null)
        {
            return new List<Chest>();
        } 
        
        return FindItemByName(item.Name);
    }
}