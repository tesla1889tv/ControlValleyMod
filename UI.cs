using StardewValley;

namespace ControlValley
{
    public class UI
    {
        public static void ShowError(string msg)
        {
            Game1.addHUDMessage(new HUDMessage(msg, HUDMessage.error_type));
        }

        public static void ShowInfo(string msg)
        {
            Game1.addHUDMessage(new HUDMessage(msg, HUDMessage.newQuest_type));
        }
    }
}
