using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TraineeApp.Adornrers
{
    internal class FiguresDrawer : Adorner
    {
        public FiguresDrawer(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            foreach (var figure in Figures)
            {
                figure.Draw(drawingContext);
            }
        }

        public List<Figures.Figure> Figures { get; set; } = [];
    }
}
