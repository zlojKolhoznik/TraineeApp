using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TraineeApp.Extensions
{
    internal static class UiElementExtensions
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement element)
        {
            element.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
