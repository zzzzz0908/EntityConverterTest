using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    // может и не статический
    public static class EntityModelConverter
    {
        private static Dictionary<(Type, Type), CrmAttributeConverter> _attributeConverters;


        static EntityModelConverter()
        {
            // как лучше регистрировать дефолтные конвертеры? 
            // как-нибудь через DI, тогда этот класс сделать не статическим?
            var convertersList = new List<CrmAttributeConverter>()
            {
                new EntityReferenceConverter(),
                new OptionSetConverter(),
                new MoneyDecimalConverter()
                // multioptionset ?
            };

            _attributeConverters = convertersList
                .ToDictionary(x => (x.PropertyType, x.CrmAttributeType));
        }

        public static T ToModel<T>(this Entity entity) where T : BaseModel, new()
        {
            var model = new T();

            var type = model.GetType();


            // cache properties metadata (ConcurrentDictionary<Type, List<PropertyInfo>>  ???)
            var properties = type.GetProperties().Where(x => x.IsDefined(typeof(CrmAttributeAttribute))).ToList();


            foreach (var propertyInfo in properties)
            {
                var crmAttr = propertyInfo.GetCustomAttribute<CrmAttributeAttribute>();

                if (entity.Contains(crmAttr.AttributeName))
                {
                    object convertedValue;

                    // extract to method? used in converters
                    object attributeValue = (entity[crmAttr.AttributeName] is AliasedValue aliasedValue)
                        ? aliasedValue.Value 
                        : entity[crmAttr.AttributeName];

                    var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    var attributeType = attributeValue.GetType();

                    //converter from attribute
                    //else
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

        //private static object GetEntityValueByAttibuteCode(Entity entity, CrmAttributeAttribute crmAttr, string aliasName)
        //{
        //    var isAliasAttr = !string.IsNullOrEmpty(aliasName);
        //    var attrName = $"{aliasName}{crmAttr.AttributeName}";
        //    var entityAttrValue = isAliasAttr ? entity.GetAttributeValue<AliasedValue>(attrName).Value : entity[attrName];
        //    object attributeValue;
        //    switch (crmAttr.AttributeTypeCode)
        //    {
        //        case AttributeCode.Lookup:
        //        case AttributeCode.Owner:
        //        case AttributeCode.Customer:
        //            {
        //                var entityRef = (EntityReference)entityAttrValue;
        //                attributeValue = new LookupModel()
        //                {
        //                    Id = entityRef.Id,
        //                    LogicalName = crmAttr.AdditionalParam,
        //                    Name = entityRef.Name
        //                };
        //                break;
        //            }
        //        case AttributeCode.Picklist:
        //        case AttributeCode.State:
        //        case AttributeCode.Status:
        //            {
        //                var optionSetLabel = entity.FormattedValues[attrName];
        //                var optionSetValue = ((OptionSetValue)entityAttrValue).Value;
        //                attributeValue = new OptionSetModel()
        //                {
        //                    Label = optionSetLabel,
        //                    Value = optionSetValue
        //                };
        //                break;
        //            }
        //        case AttributeCode.Money:
        //            {
        //                var moneyLabel = entity.FormattedValues[attrName];
        //                attributeValue = new MoneyModel()
        //                {
        //                    Label = moneyLabel,
        //                    Value = ((Money)entityAttrValue).Value
        //                };
        //                break;
        //            }
        //        case AttributeCode.PicklistMultiple:
        //            {
        //                var optionsLabels = entity.FormattedValues[attrName].Split(';');
        //                var otionSets = (OptionSetValueCollection)entityAttrValue;

        //                var optionSetModelList = new List<OptionSetModel>();
        //                for (var i = 0; i < otionSets.Count; i++)
        //                {
        //                    optionSetModelList.Add(new OptionSetModel()
        //                    {
        //                        Label = optionsLabels[i],
        //                        Value = otionSets[i].Value
        //                    });
        //                }
        //                attributeValue = optionSetModelList;
        //                break;
        //            }
        //        default:
        //            {
        //                attributeValue = entityAttrValue;
        //                break;
        //            }
        //    }
        //    return attributeValue;
        //}    
    }
}
