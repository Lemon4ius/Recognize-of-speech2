using System;

using System.Windows;
using WpfApplication2;

namespace Themes_in_WPF
{
    class AppTheme
    {



        public static void ChangeTheme(Uri themeuri)
        {
            ResourceDictionary Theme = new ResourceDictionary() { Source = themeuri };

            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(Theme);

        }
    }
}
