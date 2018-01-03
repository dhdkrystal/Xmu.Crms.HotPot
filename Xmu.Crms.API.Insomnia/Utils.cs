using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Type = Xmu.Crms.Shared.Models.Type;

namespace Xmu.Crms.Insomnia
{
    public static class Utils
    {
        public static long Id(this ClaimsPrincipal user) => long.Parse(user.Claims.Single(c => c.Type == "id").Value);

        public static Type Type(this ClaimsPrincipal user) => Enum.Parse<Type>(user.Claims.Single(c => c.Type == "type").Value, true);

        private static readonly List<JsonConverter> _StringEnumConverter =
            new List<JsonConverter> { new StringEnumConverter() };

        public static JsonSerializerSettings Ignoring(params string[] strs) => new JsonSerializerSettings
        {
            ContractResolver = ShouldSerializeContractResolverFactory.Get(new HashSet<string>(strs)),
            Converters = _StringEnumConverter
        };
    }

    public static class ShouldSerializeContractResolverFactory
    {
        private static readonly SortedDictionary<HashSet<string>, ShouldSerializeContractResolver> _Instances =
            new SortedDictionary<HashSet<string>, ShouldSerializeContractResolver>(
                Comparer<HashSet<string>>.Create((a, b) =>
                    a.IsProperSubsetOf(b)
                        ? 1
                        : (a.IsProperSupersetOf(b) ? -1 : 0)));

        public static ShouldSerializeContractResolver Get(HashSet<string> ignored)
        {
            _Instances.TryGetValue(ignored, out var v);
            return v ?? new ShouldSerializeContractResolver(ignored, _Instances);
        }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        internal readonly HashSet<string> Ignored;

        public ShouldSerializeContractResolver(HashSet<string> ignored, SortedDictionary<HashSet<string>, ShouldSerializeContractResolver> ins)
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = true,
                OverrideSpecifiedNames = true
            };
            Ignored = ignored;
            ins.Add(ignored, this);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (Ignored.Any(s =>
                s.EndsWith('*')
                    ? property.PropertyName.StartsWith(s.TrimEnd('*'), StringComparison.InvariantCultureIgnoreCase)
                    : property.PropertyName.Equals(s, StringComparison.InvariantCultureIgnoreCase)))
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}
