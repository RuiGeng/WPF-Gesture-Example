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
        private Vector linearVelocity;

        private const int MinimumMoveDelta = 10;
        private const double LinearVelocityX = 0.04;
        private const double LinearVelocityY = 0.04;

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            //store values of horizontal & vertical cumulative translation
            cumulativeDeltaX = e.CumulativeManipulation.Translation.X;
            cumulativeDeltaY = e.CumulativeManipulation.Translation.Y;

            //store value of linear velocity
            linearVelocity = e.Velocities.LinearVelocity;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            TouchGestureType.TouchGesture gesture;

            //Get the swipe gesture type
            var isSwipeGesture = GetSwipeGesture(cumulativeDeltaX, cumulativeDeltaY, linearVelocity, out gesture);

            if (isSwipeGesture)
            {
                switch (gesture)
                {
                    case TouchGestureType.TouchGesture.None:
                        GestureText = @"Gesture None";
                        break;

                    case TouchGestureType.TouchGesture.Pinch:
                        GestureText = @"Pinch gesture";
                        break;

                    case TouchGestureType.TouchGesture.MoveUp:
                        GestureText = @"Swipe form Bottom to Top";
                        break;

                    case TouchGestureType.TouchGesture.MoveRight:
                        GestureText = @"Swipe form Left to Right";
                        break;

                    case TouchGestureType.TouchGesture.MoveDown:
                        GestureText = @"Swipe form Top to Bottom";
                        break;

                    case TouchGestureType.TouchGesture.MoveLeft:
                        GestureText = @"Swipe form Right to Left";
                        break;

                    default:
                        GestureText = @"Not a swipe gesture";
                        break;
                }
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
        /// <param name="velocity"></param>
        /// <param name="gesture"></param>
        /// <returns></returns>
        private bool GetSwipeGesture(
            double deltaX,
            double deltaY,
            Vector velocity,
            out TouchGestureType.TouchGesture gesture)
        {
            bool isSwipeGesture = false;

            gesture = TouchGestureType.TouchGesture.None;

            if (Math.Abs(deltaY) > MinimumMoveDelta && Math.Abs(deltaY) > Math.Abs(deltaX) && Math.Abs(velocity.Y) >= LinearVelocityY)
            {
                gesture = (deltaY > 0) ? TouchGestureType.TouchGesture.MoveDown : TouchGestureType.TouchGesture.MoveUp;
                isSwipeGesture = true;
            }

            if (Math.Abs(deltaX) > MinimumMoveDelta && Math.Abs(deltaX) > Math.Abs(deltaY) && Math.Abs(velocity.X) >= LinearVelocityX)
            {
                gesture = (deltaX > 0) ? TouchGestureType.TouchGesture.MoveRight : TouchGestureType.TouchGesture.MoveLeft;
                isSwipeGesture = true;
            }

            return isSwipeGesture;
        }

        //private void MainWindow_OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        //{
        //    var gesture = e.SystemGesture;

        //    switch (gesture)
        //    {
        //        case SystemGesture.None:
        //            GestureText = @"Gesture None";
        //            break;
        //        case SystemGesture.Tap:
        //            GestureText = @"Gesture Tap";
        //            break;
        //        case SystemGesture.RightTap:
        //            GestureText = @"Gesture RightTap";
        //            break;
        //        case SystemGesture.Drag:
        //            GestureText = @"Gesture Drag";
        //            break;
        //        case SystemGesture.RightDrag:
        //            GestureText = @"Gesture RightDrag";
        //            break;
        //        case SystemGesture.HoldEnter:
        //            GestureText = @"Gesture HoldEnter";
        //            break;
        //        case SystemGesture.HoldLeave:
        //            GestureText = @"Gesture HoldLeave";
        //            break;
        //        case SystemGesture.HoverEnter:
        //            GestureText = @"Gesture HoverEnter";
        //            break;
        //        case SystemGesture.HoverLeave:
        //            GestureText = @"Gesture HoverLeave";
        //            break;
        //        case SystemGesture.Flick:
        //            GestureText = @"Gesture Flick";
        //            break;
        //        case SystemGesture.TwoFingerTap:
        //            GestureText = @"Gesture TwoFingerTap";
        //            break;
        //        default:
        //            GestureText = @"Gesture detected";
        //            break;
        //    }
        //}
    }
}