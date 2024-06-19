using Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Figures.Converters
{
    public class FigureJsonConverter : JsonConverter<Figure>
    {
        public override Figure? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "FigureType")
            {
                throw new JsonException();
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }
            var figureType = (FigureType)reader.GetInt32();
            Figure result = figureType switch
            {
                FigureType.Circle => new Circle(),
                FigureType.Rectangle => new Rectangle(),
                FigureType.Triangle => new Triangle(),
                _ => throw new JsonException()
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case "SizeX":
                        result.SizeX = reader.GetInt32();
                        break;
                    case "SizeY":
                        result.SizeY = reader.GetInt32();
                        break;
                    case "Coordinates":
                        result.Coordinates = JsonSerializer.Deserialize<Point>(ref reader, options);
                        break;
                    case "SpeedVector":
                        result.SpeedVector = JsonSerializer.Deserialize<Point>(ref reader, options);
                        break;
                    case "IsMoving":
                        result.IsMoving = reader.GetBoolean();
                        break;
                    case "Color":
                        result.Color = JsonSerializer.Deserialize<Color>(ref reader, options);
                        break;
                    default:
                        continue;
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Figure value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("FigureType", (int)value.FigureType);
            writer.WriteNumber("SizeX", value.SizeX);
            writer.WriteNumber("SizeY", value.SizeY);
            writer.WritePropertyName("Coordinates");
            JsonSerializer.Serialize(writer, value.Coordinates, options);
            writer.WritePropertyName("SpeedVector");
            JsonSerializer.Serialize(writer, value.SpeedVector, options);
            writer.WriteBoolean("IsMoving", value.IsMoving);
            writer.WritePropertyName("Color");
            JsonSerializer.Serialize(writer, value.Color, options);
            writer.WriteEndObject();
        }
    }
}
