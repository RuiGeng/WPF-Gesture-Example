namespace WPFGesture
{
    // https://github.com/DC-Shi/WPF-Touch-Support-Test

    using System.Windows;

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

            IGestureContainer gestureContainer = new GestureContainer(this);

            gestureContainer.EnableGestureRecognizing();

            gestureContainer.DoubleTapAction +=
                () =>
                {
                    GestureText = @"Double Tap Gesture";
                };

            gestureContainer.PinchAction +=
                () =>
                {
                    GestureText = @"Pinch Gesture";
                };

            gestureContainer.SingleTapAction +=
                () =>
                {
                    GestureText = @"Single Tap Gesture";
                };

            gestureContainer.SwipeDownAction +=
                () =>
                {
                    GestureText = @"Swipe Towards Bottom Gesture";
                };

            gestureContainer.SwipeUpAction +=
                () =>
                {
                    GestureText = @"Swipe Towards Top Gesture";
                };

            gestureContainer.SwipeLeftAction +=
                () =>
                {
                    GestureText = @"Swipe Towards Left Gesture";
                };

            gestureContainer.SwipeRightAction +=
                () =>
                {
                    GestureText = @"Swipe Towards Right Gesture";
                };
        }
    }
}