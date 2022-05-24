using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    public abstract class CrmAttributeConverter
    {
        public abstract Type PropertyType { get; }
        public abstract Type CrmAttributeType { get; }
        public abstract object GetConvertedValue(Entity entity, string attributeName);
    }

    public abstract class CrmAttributeConverter<TProperty, TCrmAttribute> : CrmAttributeConverter
    {
        public override Type PropertyType => typeof(TProperty);
        public override Type CrmAttributeType => typeof(TCrmAttribute);

        //public abstract TProperty GetConvertedValue<TProperty>(Entity entity, string attributeName);

        public sealed override object GetConvertedValue(Entity entity, string attributeName)
        {
            return GetTypedConvertedValue(entity, attributeName);
        }

        public abstract TProperty GetTypedConvertedValue(Entity entity, string attributeName);
    }
}
