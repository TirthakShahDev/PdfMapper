using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PDFMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string BOOLCONSTANT = "bool";

        public const string STRINGCONSTANT = "string";
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Click handlers
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Stream myStream = null;
            MemoryStream memorystream = new MemoryStream();
            byte[] bytesarray = new byte[byte.MaxValue];
            StringBuilder str = new StringBuilder();
            List<PropertiesArray> listitems = new List<PropertiesArray>();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF Files (*.pdf)|*.pdf";
            dlg.InitialDirectory = txtinitial.Text;
            dlg.Multiselect = false;
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                if ((myStream = dlg.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        myStream.CopyTo(memorystream);
                        bytesarray = memorystream.ToArray();
                    }
                }
            }
            if (bytesarray != null)
            {
                PdfReader pdfReader = new PdfReader(bytesarray);
                var fields = pdfReader.AcroFields.Fields;

                foreach (var acrofields in fields)
                {
                    PropertiesArray prop = new PropertiesArray();
                    str.AppendLine($"{acrofields.Key}");
                    if (!acrofields.Key.Contains("~"))
                    {
                        prop.Key = acrofields.Key;
                        listitems.Add(prop);
                    }
                }
                textBox.Text = str.ToString();
                textBox.SelectAll();

                dataGrid.ItemsSource = listitems;
                dataGrid.Visibility = Visibility.Visible;
                button2.Visibility = Visibility.Visible;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox.Text);
            MessageBox.Show("Copied!");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ListOfProperties = GetProperties();

                if (ListOfProperties != null)
                {
                    textBlock.Text = BuildClass(ListOfProperties);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        #endregion

        #region Business Logic

        public List<PropertiesArray> GetProperties()
        {
            List<PropertiesArray> arr = new List<PropertiesArray>();
            PropertiesArray prop = null;
            foreach (PropertiesArray item in dataGrid.Items.SourceCollection)
            {
                prop = new PropertiesArray();
                prop.Key = item.Key;
                prop.IsBool = item.IsBool;
                prop.IsDecimal = item.IsDecimal;
                prop.IsDate = item.IsDate;
                arr.Add(prop);

            }

            return arr;
        }

        public string BuildClass(List<PropertiesArray> propArray)
        {
            StringBuilder clsString = new StringBuilder();
            string type, propname = string.Empty;
            foreach (var item in propArray)
            {
                type = item.IsBool ? BOOLCONSTANT : STRINGCONSTANT;
                propname = item.Key;
                clsString.AppendLine($"public {type} {propname.Replace(" ", "_").Replace("#", "POUND").Replace("%", "PERCENTAGE").Replace("/", "FORSLASH")} {{ get; set; }}");
                clsString.Append(Environment.NewLine);
            }

            return clsString.ToString();
        }

        #endregion

    }

}
