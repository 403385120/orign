using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;

namespace ZYXray.Utils
{
    static class WindowHelper
    {
        public static Window GetMainWindow()
        {
            return Application.Current.MainWindow;
        }

        /// <summary>
        /// Limitations: both the application and the window must be active
        /// </summary>
        /// <returns></returns>
        public static Window GetActiveWindow()
        {
            if (Application.Current.Windows.Count > 0)
            {
                Window wnd = Application.Current.Windows.OfType<MetroWindow>().SingleOrDefault(x => x.IsActive);

                if(null == wnd)
                {
                    wnd = Application.Current.Windows.OfType<MetroWindow>().LastOrDefault();
                }

                return wnd;
            }
            else
            {
                return null;
            }
        }

        public static Window GetWindowByName(string name)
        {
            if(null == Application.Current.Windows)
            {
                return null;
            }

            foreach(Window wnd in Application.Current.Windows)
            {
                if(wnd.Name == name)
                {
                    return wnd;
                }
            }

            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
