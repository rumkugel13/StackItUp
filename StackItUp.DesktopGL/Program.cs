﻿using StackItUp.Shared;
using System;

namespace StackItUp.DesktopGL
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1(Game1.Platform.Desktop))
                game.Run();
        }
    }
}
