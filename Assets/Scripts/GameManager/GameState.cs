using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class GameState
{
    public static bool inShop = false; // default to false
    public static bool inTabPauseMenu = false; // default to false
    public static bool inPauseMenu = false; // default to false
    public static bool isTutroial = true; // default to true
    public static bool isDead = false; // default to false

    public static void Reset()
    {
        inShop = false;
        inTabPauseMenu = false;
        inPauseMenu = false;
        isTutroial = true;
        isDead = false;
    }
}
