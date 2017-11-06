using DotNetFiddle.IntelligentCompletion;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
using System.Xml.Serialization;

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
            button3.Visibility = Visibility.Hidden;
            // IterateClass(typeof(SampleClass));
            //var garage = new SampleClass();

            //// TODO init your garage..

            //XmlSerializer xs = new XmlSerializer(typeof(SampleClass));
            //TextWriter tw = new StreamWriter("test.xml");
            //xs.Serialize(tw, garage);

            //Console.Write("asd");

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

                if (fields.Count > 0)
                {
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

                    SampleClass fakeObject = new SampleClass();
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(fakeObject);


                    dataGrid.ItemsSource = listitems;
                    dataGrid.Visibility = Visibility.Visible;
                    button2.Visibility = Visibility.Visible;
                }
                else
                {
                    textBox.Text = string.Empty;
                    textBlock.Text = string.Empty;
                    dataGrid.Visibility = Visibility.Hidden;
                    button2.Visibility = Visibility.Hidden;
                    button3.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                textBox.Text = string.Empty;
                textBlock.Text = string.Empty;
                dataGrid.Visibility = Visibility.Hidden;
                button2.Visibility = Visibility.Hidden;
                button3.Visibility = Visibility.Hidden;
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
                    button3.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBlock.Text);
            MessageBox.Show("Copied!");
        }

        private void txtproperties_KeyUp(object sender, KeyEventArgs e)
        {
            PerformKeyUpEvent(sender);
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

        private List<AutoCompleteItem> Autocompletetest(string query)
        {

            var source = File.ReadAllText(@"D:\Projects\PracticeThing\GIT\PDFMapper\PDFMapper\SampleClass.cs");

            var sourceMain = $@"using System;
                using PDFMapper;

                    public class Program
                    {{
                        public static void Main()
                        {{
                            var caseInformation = new SampleClass();
                            {query}
                        }}
                    }}";

            var service = new CSharpLanguageService();

            var files = new Dictionary<string, string>()
            {
                {"dog.cs", source},
                {"main.cs", sourceMain}
            };

            var project = service.GetProject(files);

            int dotIndex = sourceMain.LastIndexOf(".") + 1;
            var autoCompleteItems = service.GetAutoCompleteItems(project, "main.cs", dotIndex);

            return autoCompleteItems.Distinct().ToList();
        }

        private void addItem(string text, object sndr = null)
        {
            TextBlock block = new TextBlock();

            // Add the text 
            block.Text = text;

            // A little style... 
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events 
            block.MouseLeftButtonUp += (sender, e) =>
            {
                (sndr as TextBox).Text = (sndr as TextBox).Text + (sender as TextBlock).Text + ".";
                PerformKeyUpEvent((sndr as TextBox));
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel 
            resultStack.Children.Add(block);
        }

        public void PerformKeyUpEvent(object sender)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;

            string query = (sender as TextBox).Text;

            if (query.Length == 0)
            {
                // Clear 
                resultStack.Children.Clear();
                border.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                border.Visibility = System.Windows.Visibility.Visible;
            }

            // Clear the list 
            resultStack.Children.Clear();

            // Add the result 

            if (query.Trim() != "" && query.ToString().EndsWith("."))
            {
                foreach (var obj in Autocompletetest(query).ToList())
                {
                    addItem(obj.Name, sender);
                    found = true;
                }
            }
            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }
        #endregion
    }

}
