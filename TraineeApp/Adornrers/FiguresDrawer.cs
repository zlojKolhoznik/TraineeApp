using Figures;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using Figure = Figures.Figure;

namespace TraineeApp.Adornrers
{
    internal class FiguresDrawer : Adorner
    {
        private const string LogPath = "log.txt";

        public FiguresDrawer(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var pMax = new Point(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            foreach (var figure in Figures)
            {
                figure.Draw(drawingContext);
                try
                {
                    figure.Move(pMax);
                }
                catch (FigureOutOfBoundsException e)
                {
                    using var file = new StreamWriter(LogPath, true);
                    file.WriteLine(e);
                    figure.ReturnToBounds(pMax);
                }
                figure.CollisionCheck(Figures);
            }
        }

        public List<Figure> Figures { get; set; } = [];
    }
}
