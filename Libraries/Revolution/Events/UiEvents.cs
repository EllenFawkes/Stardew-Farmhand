﻿using System;
using Revolution.Attributes;
using Revolution.Logging;

namespace Revolution.Events
{
    public static class UiEvents
    {
        public static event EventHandler OnAfterIClickableMenuInitialized = delegate { };
        public static event EventHandler OnAfterBundleConstructed = delegate { };

        [Hook(HookType.Exit, "StardewValley.Menus.IClickableMenu", "initialize")]
        internal static void InvokeAfterIClickableMenuInitialized([ThisBind] object @this, 
            [InputBind(typeof(int), "x")] int x,
            [InputBind(typeof(int), "y")] int y,
            [InputBind(typeof(int), "width")] int width,
            [InputBind(typeof(int), "height")] int height,
            [InputBind(typeof(bool), "showUpperRightCloseButton")] bool showCloseBtn
            )
        {
            Log.Success("It works!");
            EventCommon.SafeInvoke(OnAfterIClickableMenuInitialized, null);
        }
        
        [Hook(HookType.Exit, "StardewValley.Menus.Bundle", ".ctor")]
        internal static void InvokeAfterBundleConstructed([ThisBind] object @this)
        {
            EventCommon.SafeInvoke(OnAfterBundleConstructed, @this);
        }
    }
}