using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Figures
{
    [Serializable]
    public class FigureOutOfBoundsException : Exception, ISerializable
    {
        public FigureOutOfBoundsException()
        {
        }

        public FigureOutOfBoundsException(string message) : base(message)
        {
        }

        public FigureOutOfBoundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FigureOutOfBoundsException(string message, int x, int y) : base(message)
        {
            X = x;
            Y = y;
        }

        protected FigureOutOfBoundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            X = info.GetInt32("X");
            Y = info.GetInt32("Y");
            DateTime = info.GetDateTime("DateTime");
        }

        public int X { get; init; }
        public int Y { get; init; }
        public DateTime DateTime { get; init; } = DateTime.Now;
        public override string Message => base.Message + $" (X: {X}, Y: {Y})";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("DateTime", DateTime);
        }

        public override string ToString()
        {
            return $"[{DateTime}]" + base.ToString();
        }
    }
}
