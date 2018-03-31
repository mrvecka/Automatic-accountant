using OCR_BusinessLayer.Classes.Client;
using OCR_BusinessLayer.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_BusinessLayer.Classes.Client
{
    [TypeConverter(typeof(ClinetCollectionConverter))]
    public class ClientCollection : CollectionBase, ICustomTypeDescriptor
    {
        public ClientCollection(List<Client> list)
        {
            foreach (Client c in list)
                Add(c);
        }
        public ClientCollection() { }

        /// <summary>
        /// Adds an client object to the collection
        /// </summary>
        /// <param name="cl"></param>
        public void Add(Client cl)
        {
            this.List.Add(cl);
        }


        /// <summary>
        /// Removes an client object from the collection
        /// </summary>
        /// <param name="emp"></param>
        public void Remove(Client cl)
        {
            this.List.Remove(cl);
        }

        /// <summary>
        /// Returns an client object at index position.
        /// </summary>
        public Client this[int index]
        {
            get
            {
                if (index < this.List.Count)
                    return (Client)this.List[index];
                else
                    return null;
            }
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            //// Create a collection object to hold property descriptors
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++)
            {
                // Create a property descriptor for the employee item and add to the property descriptor collection
                ClientCollectionPropertyDecriptor pd = new ClientCollectionPropertyDecriptor(this, i);
                pds.Add(pd);
            }
            // return the property descriptor collection
            return pds;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }
}
