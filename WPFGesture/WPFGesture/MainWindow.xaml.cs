using System;
using System.Diagnostics;
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
        private Vector linearVelocity;

        private const int MinimumMoveDelta = 10;
        private const double LinearVelocityX = 0.04;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Use the ManipulationInertiaStarting event to set the desired deceleration value for the given manipulation behavior,
        /// e.g., ExpansionBehavior and RotationBehavior.
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

            //store value of linear velocity
            linearVelocity = e.Velocities.LinearVelocity;
        }

        private void MainWindow_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Console.WriteLine($@"**cumulativeDeltaX = {cumulativeDeltaX}**");
            Console.WriteLine($@"**cumulativeDeltaY = {cumulativeDeltaY}**");
            Console.WriteLine($@"**linearVelocityX = {linearVelocity.X}**");
            Console.WriteLine($@"**linearVelocityY = {linearVelocity.Y}**");

            //Get the swipe gesture type
            var swipeGesture = GetSwipeGesture(cumulativeDeltaX, cumulativeDeltaY, linearVelocity);

            switch (swipeGesture)
            {
                case TouchGestureType.TouchGesture.None:
                    GestureText = @"Not a swipe gesture";
                    break;
                case TouchGestureType.TouchGesture.Pinch:
                    GestureText = @"Pinch gesture";
                    break;
                case TouchGestureType.TouchGesture.MoveUp:
                    GestureText = @"Swipe form Down to Up";
                    break;
                case TouchGestureType.TouchGesture.MoveRight:
                    GestureText = @"Swipe form Left to Right";
                    break;
                case TouchGestureType.TouchGesture.MoveDown:
                    GestureText = @"Swipe form Up to Down";
                    break;
                case TouchGestureType.TouchGesture.MoveLeft:
                    GestureText = @"Swipe form Right to Left";
                    break;
                default:
                    GestureText = @"Not a swipe gesture";
                    break;
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
        /// <param name="linearVelocity"></param>
        /// <returns></returns>
        private TouchGestureType.TouchGesture GetSwipeGesture(double deltaX, double deltaY, Vector linearVelocity)
        {
            TouchGestureType.TouchGesture resultType = TouchGestureType.TouchGesture.None;

            if (Math.Abs(deltaY) > MinimumMoveDelta && Math.Abs(deltaY) > Math.Abs(deltaX))
            {
                resultType = (deltaY > 0) ? TouchGestureType.TouchGesture.MoveDown : TouchGestureType.TouchGesture.MoveUp;
            }

            if (Math.Abs(deltaX) > MinimumMoveDelta && Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                resultType = (deltaX > 0) ? TouchGestureType.TouchGesture.MoveRight : TouchGestureType.TouchGesture.MoveLeft;
            }

            return resultType;

            //bool result = Math.Abs(deltaY) <= DeltaY && Math.Abs(deltaX) >= DeltaX && Math.Abs(linear) >= LinearVelocityX;

            //return result;
        }
    }
}