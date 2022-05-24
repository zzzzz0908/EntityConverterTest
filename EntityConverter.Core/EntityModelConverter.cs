using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityConverter.Core
{
    public class EntityModelConverter
    {
        private static ConcurrentDictionary<Type, List<PropertyInfo>> _propertyInfoCache = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        private static ConcurrentDictionary<PropertyInfo, CrmAttributeAttribute> _attributeCache = new ConcurrentDictionary<PropertyInfo, CrmAttributeAttribute>();
        
        // инициализировать на старте программы, в Startup например
        public static List<CrmAttributeConverter> DefaultAttributeConverters { get; set; } // какие лучше модификаторы?

        public static EntityModelConverter CreateDefault()
        {
            return new EntityModelConverter(DefaultAttributeConverters);
        }



        private Dictionary<(Type, Type), CrmAttributeConverter> _attributeConverters;

        public EntityModelConverter(IEnumerable<CrmAttributeConverter> attributeConverters)
        {
            _attributeConverters = (attributeConverters ?? new List<CrmAttributeConverter>())
                .ToDictionary(x => (x.PropertyType, x.CrmAttributeType));
        }

        public T ConvertToModel<T>(Entity entity) where T : class, new()  // было ограничение на BaseModel, но нужно ли оно?
        {
            var model = new T();

            var type = typeof(T);

            // не уверен, что нужен кэш

            //var properties = type.GetProperties()
            //    .Where(x => x.IsDefined(typeof(CrmAttributeAttribute)))
            //    .ToList();

            var properties = _propertyInfoCache.GetOrAdd(type,
                t => t.GetProperties()
                .Where(x => x.IsDefined(typeof(CrmAttributeAttribute)))
                .ToList());



            foreach (var propertyInfo in properties)
            {
                //var crmAttr = propertyInfo.GetCustomAttribute<CrmAttributeAttribute>();

                var crmAttr = _attributeCache.GetOrAdd(propertyInfo, p => p.GetCustomAttribute<CrmAttributeAttribute>());


                if (entity.Contains(crmAttr.AttributeName))
                {
                    object convertedValue;

                    object attributeValue = (entity[crmAttr.AttributeName] is AliasedValue aliasedValue)
                        ? aliasedValue.Value
                        : entity[crmAttr.AttributeName];

                    var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    var attributeType = attributeValue.GetType();

                    if (crmAttr.ConverterType != null)
                    {
                        var converterType = crmAttr.ConverterType;

                        var converter = (CrmAttributeConverter)Activator.CreateInstance(converterType);

                        convertedValue = converter.GetConvertedValue(entity, crmAttr.AttributeName);
                    }
                    else if (propertyType == attributeType)
                    {
                        convertedValue = attributeValue;
                    }
                    else if (_attributeConverters.TryGetValue((propertyType, attributeType), out var converter))
                    {
                        convertedValue = converter.GetConvertedValue(entity, crmAttr.AttributeName);
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(attributeValue, propertyInfo.PropertyType);
                    }

                    propertyInfo.SetValue(model, convertedValue);
                }
            }
            return model;
        }
    }
}
