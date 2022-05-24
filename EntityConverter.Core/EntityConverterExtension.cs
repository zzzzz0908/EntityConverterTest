using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityConverter.Core
{
    public static class EntityConverterExtension
    {
        public static T ToModel<T>(this Entity entity) where T : class, new()
        {
            return EntityModelConverter.CreateDefault().ConvertToModel<T>(entity);
        }

        public static T GetUnaliasedAttributeValue<T>(this Entity entity, string attributeLogicalName) // имя так себе)
        {
            return (entity[attributeLogicalName] is AliasedValue aliasedValue)
                ? (T)aliasedValue.Value
                : (T)entity[attributeLogicalName];
        }
    }
}
