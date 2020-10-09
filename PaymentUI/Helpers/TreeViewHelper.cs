using System.Windows;

namespace PaymentUI.Helpers
{
    public static class TreeViewHelper
    {
        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                return null;
            }

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);

                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
