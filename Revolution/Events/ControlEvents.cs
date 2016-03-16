﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Revolution.Events
{
    public class ControlEvents
    {
        public static event EventHandler OnKeyboardChanged = delegate { };
        public static event EventHandler OnKeyPressed = delegate { };
        public static event EventHandler OnKeyReleased = delegate { };
        public static event EventHandler OnMouseChanged = delegate { };
        public static event EventHandler OnControllerButtonPressed = delegate { };
        public static event EventHandler OnControllerButtonReleased = delegate { };
        public static event EventHandler OnControllerTriggerPressed = delegate { };
        public static event EventHandler OnControllerTriggerReleased = delegate { };

        public static void InvokeKeyboardChanged(KeyboardState priorState, KeyboardState newState)
        {
            OnKeyboardChanged.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeMouseChanged(MouseState priorState, MouseState newState)
        {
            OnMouseChanged.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeKeyPressed(Keys key)
        {
            OnKeyPressed.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeKeyReleased(Keys key)
        {
            OnKeyReleased.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeButtonPressed(PlayerIndex playerIndex, Buttons buttons)
        {
            OnControllerButtonPressed.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeButtonReleased(PlayerIndex playerIndex, Buttons buttons)
        {
            OnControllerButtonReleased.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeTriggerPressed(PlayerIndex playerIndex, Buttons buttons, float value)
        {
            OnControllerTriggerPressed.Invoke(null, EventArgs.Empty);
        }

        public static void InvokeTriggerReleased(PlayerIndex playerIndex, Buttons buttons, float value)
        {
            OnControllerTriggerReleased.Invoke(null, EventArgs.Empty);
        }
    }
}