using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_console
{
    public class MoneyDecimalConverter : CrmAttributeConverter<decimal, Money>
    {
        public override decimal GetTypedConvertedValue(Entity entity, string attributeName)
        {
            var moneyValue = (entity[attributeName] is AliasedValue aliasedValue)
                ? aliasedValue.Value as Money
                : entity[attributeName] as Money;

            return moneyValue.Value;            
        }
    }
}
