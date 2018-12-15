using System.Windows.Controls;

namespace Gihan.Wpf
{
    /// <summary>
    /// Interaction logic for DoubleEntry.xaml
    /// </summary>
    public partial class DoubleEntry : UserControl
    {
        public Grid Grid { get; }
        public TextBox TextBox1 { get; }
        public TextBox TextBox2 { get; }
        public Border MiddleBorder { get; }
        public CheckBox CheckBox { get; }


        public string Text1 => TextBox1.Text;
        public string Text2 => TextBox2.Text;
        public bool? IsChecked { get => CheckBox.IsChecked; set => CheckBox.IsChecked = value; }
        public object Data { get; set; }


        public DoubleEntry()
        {
            InitializeComponent();
            Grid = Grd;
            TextBox1 = Txt1;
            TextBox2 = Txt2;
            MiddleBorder = BrdMid;
            CheckBox = Chk;
        }
    }
}
