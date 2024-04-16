using Enhanced.Models.Shared;
using Newtonsoft.Json;
using System.Collections;
using System.Reflection;

namespace Enhanced.Models.AmazonData
{
    public class ParameterBased
    {
        public virtual List<KeyValuePair<string, string>> GetParameters()
        {
            List<KeyValuePair<string, string>> queryParameters = new();

            PropertyInfo[] pi = this.GetType().GetProperties();

            foreach (PropertyInfo p in pi)
            {
                var value = p.GetValue(this);

                if (value != null)
                {
                    var propertyType = p.PropertyType;
                    var propTypeName = p.PropertyType.Name;

                    string output = "";

                    if (propertyType == typeof(DateTime) || propertyType == typeof(Nullable<DateTime>))
                    {
                        output = ((DateTime)value).ToString(Constant.DateISO8601Format);
                    }
                    else if (propTypeName == typeof(String).Name)
                    {
                        output = value.ToString()!;
                    }
                    else if (p.PropertyType.IsEnum || IsNullableEnum(p.PropertyType))
                    {
                        output = value.ToString()!;
                    }
                    else if (IsEnumerableOfEnum(p.PropertyType) || IsEnumerable(p.PropertyType))
                    {
                        var data = ((IEnumerable)value).Cast<object>().Select(a => a.ToString());

                        if (data!.Any())
                        {
                            var result = data.ToArray();
                            output = String.Join(",", result);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        output = JsonConvert.SerializeObject(value);
                    }

                    var propName = p.Name;

                    queryParameters.Add(new KeyValuePair<string, string>(propName, output));
                }
            }

            return queryParameters;
        }

        private static bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t)!;
            return (u != null) && u.IsEnum;
        }

        private static bool IsEnumerableOfEnum(Type type)
        {
            return GetEnumerableTypes(type).Any(t => t.IsEnum);
        }

        private static bool IsEnumerable(Type type)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType
                    && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        || type.GetGenericTypeDefinition() == typeof(IList<>)
                        || type.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<Type> GetEnumerableTypes(Type type)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    yield return type.GetGenericArguments()[0];
                }
            }

            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    yield return intType.GetGenericArguments()[0];
                }
            }
        }
    }
}
