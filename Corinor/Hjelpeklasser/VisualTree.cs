using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Corinor.Hjelpeklasser
{
    public class VisualTree
    {

        public static Window getWindowFromParent(FrameworkElement element)
        {
            DependencyObject o = element.Parent;
            while (o != null)
            {
                if (o is Window) 
                    return (Window)o;

                if (o is FrameworkElement) o = 
                    (o as FrameworkElement).Parent;
            }

            return null;
        }


        public static T getParentKontroll<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null && !(obj is T))
                obj = VisualTreeHelper.GetParent(obj) as DependencyObject;

            return obj as T;
        }

    }
}
