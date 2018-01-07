using Newtonsoft.Json;
using SimpleJsonDataSource.Models;
using System;
using System.Globalization;

namespace SimpleJsonDataSource.ViewModels.Converters
{
	public class DataPointConverter<T> : JsonConverter
	{
		public static readonly DataPointConverter<T> Instance = new DataPointConverter<T>();
		
		private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private DataPointConverter() { }

		public override bool CanConvert(Type objectType) => objectType == typeof(DataPoint<T>);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected token {0} when parsing data point.", reader.TokenType));
			}

			if (!reader.Read())
			{
				throw new JsonSerializationException("Unexpected end when reading JSON.");
			}
			
			return new DataPoint<T>
			(
				value: serializer.Deserialize<T>(reader),
				dateTime: UnixEpoch.AddTicks(serializer.Deserialize<long>(reader) * TimeSpan.TicksPerMillisecond)
			);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var dataPoint = (DataPoint<T>)value;

			writer.WriteStartArray();
			serializer.Serialize(writer, dataPoint.Value);
			serializer.Serialize(writer, (dataPoint.DateTime - UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond);
			writer.WriteEndArray();
		}
	}
}
