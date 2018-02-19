﻿using System;
using System.Threading.Tasks;

using IISSMVVM.Views;

using Microsoft.Toolkit.Uwp.Helpers;

namespace IISSMVVM.Services
{
    public static class FirstRunDisplayService
    {
        private static bool shown = false;

        internal static async Task ShowIfAppropriateAsync()
        {
            if (SystemInformation.IsFirstRun && !shown)
            {
                shown = true;
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
            }
        }
    }
}
