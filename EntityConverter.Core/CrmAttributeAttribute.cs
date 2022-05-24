using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityConverter.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CrmAttributeAttribute : Attribute // or CrmFieldAttribute      атрибут из црм перемешивается с атрибутом C#, как лучше назвать поле из црм
    {
        private Type converterType;

        public string AttributeName { get; set; }

        public Type ConverterType
        {
            get => converterType;
            set
            {
                converterType = value.IsSubclassOf(typeof(CrmAttributeConverter))
                    ? value
                    : throw new ArgumentException("Type is not Converter");
            }

        }

        public CrmAttributeAttribute(string attributeName)
        {
            AttributeName = attributeName;
        }
    }
}
