﻿using System.Windows;
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
            var pMax = new Point(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            foreach (var figure in Figures)
            {
                figure.Draw(drawingContext);
                figure.Move(pMax);
            }
        }

        public List<Figures.Figure> Figures { get; set; } = [];
    }
}
