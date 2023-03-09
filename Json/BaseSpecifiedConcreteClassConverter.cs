using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ELFMusGen
{
	public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
	{
		protected override JsonConverter ResolveContractConverter(Type objectType)
		{
			if (typeof(Layer).IsAssignableFrom(objectType) && typeof(ActivationFunction).IsAssignableFrom(objectType) && !objectType.IsAbstract)
				return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
			return base.ResolveContractConverter(objectType);
		}
	}
}
