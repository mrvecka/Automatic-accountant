using System;
using System.ComponentModel;
using System.Text;

namespace OCR_BusinessLayer.Classes.Client
{
    public class ClientCollectionPropertyDecriptor : PropertyDescriptor
    {
        private ClientCollection collection = null;
        private int index = -1;

        public ClientCollectionPropertyDecriptor(ClientCollection collection, int index) : base("#"+index.ToString(),null)
        {
            this.collection = collection;
            this.index = index;
        }

        public override AttributeCollection Attributes
        {
            get
            {
                return new AttributeCollection(null);
            }
        }
        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get
            {
                return this.collection.GetType();
            }
        }
        //public override string DisplayName
        //{
        //    get
        //    {
        //        Client cl = this.collection[index];
        //        return cl.Name;
        //    }
        //}
        public override string Description
        {
            get
            {
                Client cl = this.collection[index];
                if (cl != null)
                {
                StringBuilder sb = new StringBuilder();
                sb.Append(cl.Name);
                sb.Append(",");
                sb.Append(cl.Street);
                sb.Append(",");
                sb.Append(cl.PSCCity);
                sb.Append(",");
                sb.Append(cl.State);

                return sb.ToString();

                }
                return "";
            }
        }
        public override object GetValue(object component)
        {
            return this.collection[index];
        }

        public override bool IsReadOnly => false;
        public override string Name => "#"+index.ToString();

        public override Type PropertyType
        {
            get
            {
                if (this.collection[index] == null)
                    return null;
                return this.collection[index].GetType();

            }
        }



        public override void ResetValue(object component)
        {

        }
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            // this.collection[index] = value;
        }

    }
}
