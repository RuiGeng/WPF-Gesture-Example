using System;
using System.Windows;
using System.Windows.Input;

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

        private double cumulativeDeltaX;
        private double cumulativeDeltaY;
        private double linearVelocity;
        private double linearVelocitx;

        private const double DeltaX = 50;
        private const double DeltaY = 50;
        private const double LinearVelocityX = 0.04;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Use the ManipulationInertiaStarting event to set the desired deceleration value for the given manipulation behavior,
        /// e.g., ExpansionBehaviour and RotationBehaviour.
        /// After the ManipulationInertiaStarting is called,
        /// it will call ManipulationDelta until velocity becomes zero.
        /// Set initial velocity of the expansion behavior and desired deceleration here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.ExpansionBehavior = new InertiaExpansionBehavior()
            {
                InitialVelocity = e.InitialVelocities.ExpansionVelocity,
                DesiredDeceleration = 10.0 * 96.0 / 1000000.0
            };
        }

        /// <summary>
        /// Utilize the ManipulationStarting event to set values for ManipulationContainer and Handled property.
        /// ManipulationContainer allows specifying that the position should be relative to another element.
        /// Handled allows specifying how the event is handled by the class handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
        }

        private void MainWindow_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            //store values of horizontal & vertical cumulative translation
            cumulativeDeltaX = e.CumulativeManipulation.Translation.X;
            cumulativeDeltaY = e.CumulativeManipulation.Translation.Y;

            //store value of linear velocity into horizontal direction
            linearVelocity = e.Velocities.LinearVelocity.X;
            //store value of linear velocity into vertical direction
            linearVelocitx = e.Velocities.LinearVelocity.Y;
        }

        private void MainWindow_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            bool isRightToLeftSwipe = cumulativeDeltaX < 0;

            //check if this is swipe gesture
            if (IsSwipeGesture(cumulativeDeltaX, cumulativeDeltaY, linearVelocity))
            {
                GestureText = isRightToLeftSwipe ? @"Swipe From Right To Left" : @"Swipe From Left To Right";
            }
            else
            {
                GestureText = @"Not a swipe Gesture";
            }
        }

        /// <summary>
        /// isSwipeGesture is a method to determine the characteristics of a swipe gesture.
        /// Consider deltaX, deltaY, and velocity to decide the swipe gesture’s characteristics.
        /// Here, DeltaX and DeltaY are defined as constant values that define the maximum allowed horizontal and vertical movement,
        /// respectively, LinearVelocityX is defined as a constant value that defines the maximum allowed velocity.
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        /// <param name="linearY"></param>
        /// <returns></returns>
        private bool IsSwipeGesture(double deltaX, double deltaY, double linearY)
        {
            bool result = Math.Abs(deltaY) <= DeltaY && Math.Abs(deltaX) >= DeltaX && Math.Abs(linearY) >= LinearVelocityX;

            return result;
        }
    }
}