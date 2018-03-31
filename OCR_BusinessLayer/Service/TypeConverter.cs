using OCR_BusinessLayer.Classes;
using OCR_BusinessLayer.Classes.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Service
{
    internal class EvidenceConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is Client)
            {
                // Cast the value to an Employee type
                Evidence ev = (Evidence)value;

                // Return department and department role separated by comma.
                return ev.OrderNumber;
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }

    internal class ClientConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is Client)
            {
                // Cast the value to an Employee type
                Client cl = (Client)value;

                // Return department and department role separated by comma.
                return cl.Name;
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }

    internal class ClinetCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is ClientCollection)
            {
                // Return department and department role separated by comma.
                return "Clients data";
            }
            return base.ConvertTo(context, culture, value, destType);
        }

    }
}
