using Figures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;

using Figure = Figures.Figure;
using System.Xml.Serialization;

#pragma warning disable SYSLIB0011 // Type or member is obsolete (for BinaryFormatter)

namespace TraineeApp.IO
{
    internal static class FigureSerializer
    {
        public static void SaveToBinary(string path, IEnumerable<Figure> figures)
        {
            using var fs = new FileStream(path, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, figures);
        }

        public static void SaveToJson(string path, IEnumerable<Figure> figures)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var json = JsonSerializer.Serialize(figures, options);
            File.WriteAllText(path, json);
        }

        public static void SaveToXml(string path, IEnumerable<Figure> figures)
        {
            var serializer = new XmlSerializer(typeof(List<Figure>));
            using var fs = new FileStream(path, FileMode.Create);
            serializer.Serialize(fs, figures);
        }

        public static IEnumerable<Figure> ReadFromBinary(string path)
        {
            using var fs = new FileStream(path, FileMode.Open);
            var formatter = new BinaryFormatter();
            return (List<Figure>)formatter.Deserialize(fs);
        }

        public static IEnumerable<Figure> ReadFromJson(string path)
        {
            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            return JsonSerializer.Deserialize<List<Figure>>(json, options);
        }

        public static IEnumerable<Figure> ReadFromXml(string path)
        {
            var serializer = new XmlSerializer(typeof(List<Figure>));
            using var fs = new FileStream(path, FileMode.Open);
            return (List<Figure>)serializer.Deserialize(fs);
        }
    }
}
