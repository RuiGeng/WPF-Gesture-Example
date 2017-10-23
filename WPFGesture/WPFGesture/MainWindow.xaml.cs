using System.Windows;

namespace WPFGesture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty GestureTextProperty =
            DependencyProperty.Register(
                "GestureText",
                typeof(string),
                typeof(MainWindow),
                new PropertyMetadata(string.Empty));

        public string GestureText
        {
            get
            {
                return (string)GetValue(GestureTextProperty);
            }
            set
            {
                SetValue(GestureTextProperty, value);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}