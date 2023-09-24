using System;
using Shared.Mailchimp.Model.Enum;

namespace Shared.Mailchimp.Model.Attributes
{
    public class DataTypeAttribute : Attribute
    {
        public DataType DataType { get; set; }

        public DataTypeAttribute(DataType dataType)
        {
            DataType = dataType;
        }
    }
}