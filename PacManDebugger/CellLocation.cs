using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PacManDebugger
{
    public readonly struct CellLocation
    {
        public int X { get; }
        public int Y { get; }
        public CellLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X},{Y}";
    }

    public class CellLocationConverter : JsonConverter<CellLocation>
    {
        public override CellLocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int x = 0;
            int y = 0;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (reader.GetString() == "X")
                    {
                        if (reader.Read())
                            x = reader.GetInt32();
                    }
                    else if (reader.GetString() == "Y")
                    {
                        if (reader.Read())
                            y = reader.GetInt32();
                    }
                }

            }
            return new CellLocation(x, y);
        }

        public override void Write(Utf8JsonWriter writer, CellLocation value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
