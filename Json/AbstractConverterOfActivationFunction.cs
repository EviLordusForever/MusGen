using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ELFMusGen
{
	public class AbstractConverterOfActivationFunction : JsonConverter
	{
		static JsonSerializerSettings _specifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(ActivationFunction));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			reader.SupportMultipleContent = true;

			JObject jo = JObject.Load(reader);
			reader = jo.CreateReader();
			if (jo["_type"] != null)
				switch (jo.GetValue("_type").ToString())
				{
					case "Avocado":
						return serializer.Deserialize(reader, typeof(Avocado));
					case "ELU":
						return serializer.Deserialize(reader, typeof(ELU));
					case "Linear":
						return serializer.Deserialize(reader, typeof(Linear));
					case "ReLU":
						return serializer.Deserialize(reader, typeof(ReLU));
					case "Sigmoid":
						return serializer.Deserialize(reader, typeof(Sigmoid));
					case "SoftPlus":
						return serializer.Deserialize(reader, typeof(SoftPlus));
					case "SoftSign":
						return serializer.Deserialize(reader, typeof(SoftSign));
					case "TanH":
						return serializer.Deserialize(reader, typeof(TanH));
					case "LeakyReLU":
						return serializer.Deserialize(reader, typeof(LeakyReLU));
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
