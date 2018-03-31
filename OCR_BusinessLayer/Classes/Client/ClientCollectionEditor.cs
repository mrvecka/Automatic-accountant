using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing.Design;

namespace OCR_BusinessLayer.Classes.Client
{
    public class ClientCollectionEditor : CollectionEditor

    {

        // Define a static event to expose the inner PropertyGrid's

        // PropertyValueChanged event args...
        public static EventHandler CollectionChanged;

        protected PropertyGrid ownerGrid;

        // Inherit the default constructor from the standard

        // Collection Editor...

        public ClientCollectionEditor(Type type) : base(type) { }



        // Override this method in order to access the containing user controls

        // from the default Collection Editor form or to add new ones...

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {


            PropertyInfo ownerGridProperty = provider.GetType().GetProperty("OwnerGrid", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            ownerGrid = (PropertyGrid)ownerGridProperty.GetValue(provider);

            return base.EditValue(context, provider, value);
        }

        protected override CollectionForm CreateCollectionForm()

        {

            CollectionForm collectionForm = base.CreateCollectionForm();
            Form frm = collectionForm as Form;
            if (frm != null)
            {
                // Get OK button of the Collection Editor...
                Button button = frm.AcceptButton as Button;
                var addButton = (ButtonBase)frm.Controls.Find("addButton", true).First();
                var removeButton = (ButtonBase)frm.Controls.Find("removeButton", true).First();


                // Handle click event of the button
                button.Click += new EventHandler(OnCollectionChanged);


            }
            return collectionForm;

        }


        void OnCollectionChanged(object sender, EventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }




    }
}
