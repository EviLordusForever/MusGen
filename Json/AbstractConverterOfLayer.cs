using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ELFMusGen
{
	public class AbstractConverterOfLayer : JsonConverter
	{
		static JsonSerializerSettings _specifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(Layer));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			reader.SupportMultipleContent = true;

			JObject jo = JObject.Load(reader);
			reader = jo.CreateReader();
			if (jo["_type"] != null)
				switch (jo.GetValue("_type").ToString())
				{
					case "perceptron":
						return serializer.Deserialize(reader, typeof(LayerPerceptron));
					case "megatron":
						return serializer.Deserialize(reader, typeof(LayerMegatron));
					case "cybertron":
						return serializer.Deserialize(reader, typeof(LayerCybertron));
					default:
						throw new Exception();
				}
			throw new NotImplementedException();
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException(); // won't be called because CanWrite returns false
		}
	}
}
