using EntityConverter.Core;
using ExampleWebApi.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExampleWebApi.Converters
{
    public class EntityReferenceConverter : CrmAttributeConverter<EntityReferenceModel, EntityReference>
    {
        public override EntityReferenceModel GetTypedConvertedValue(Entity entity, string attributeName)
        {
            var entityReference = entity.GetUnaliasedAttributeValue<EntityReference>(attributeName);

            return new EntityReferenceModel
            {
                Id = entityReference.Id,
                Name = entityReference.Name,
                LogicalName = entityReference.LogicalName
            };
        }
    }
}