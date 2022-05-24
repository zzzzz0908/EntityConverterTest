using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    public class EntityReferenceConverter : CrmAttributeConverter<EntityReferenceModel, EntityReference>
    {
        //public override EntityReferenceModel GetConvertedValue(Entity entity, string attributeName)
        //{
        //    var entityReference = entity.GetAttributeValue<EntityReference>(attributeName);

        //    return new EntityReferenceModel
        //    {
        //        Id = entityReference.Id,
        //        Name = entityReference.Name,
        //        LogicalName = entityReference.LogicalName
        //    };
        //}

        public override EntityReferenceModel GetTypedConvertedValue(Entity entity, string attributeName)
        {
            var entityReference = (entity[attributeName] is AliasedValue aliasedValue)
                            ? aliasedValue.Value as EntityReference
                            : entity[attributeName] as EntityReference;

            //var entityReference = entity.GetAttributeValue<EntityReference>(attributeName);

            return new EntityReferenceModel
            {
                Id = entityReference.Id,
                Name = entityReference.Name,
                LogicalName = entityReference.LogicalName
            };
        }
    }
}
