using System;
using System.Collections.Generic;
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
        private const double LinearVelocity = 0.04;

        private const double MaximumDoubleTapDistance = 20;

        private const double MaximumDoubleTapInterval = 0.3;

        private bool isDoubleTap;

        private Point firstTap;
        private Point currentPoint;
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// track the touches by device id
        /// </summary>
        private readonly Dictionary<int, List<Point>> tracker;

        public MainWindow()
        {
            InitializeComponent();

            tracker = new Dictionary<int, List<Point>>();
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
            tracker.Clear();
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

            //store value of Touch Point
            foreach (var m in e.Manipulators)
            {
                // track the touches
                TrackTouch(m.Id, m.GetPosition(TheWindow));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            TouchGestureType gesture;

            if (tracker.Count == 2)
            {
                GestureText = @"Gesture Pinch";
                return;
            }

            if (!isDoubleTap &&
                GetSwipeGesture(cumulativeDeltaX, cumulativeDeltaY, linearVelocity, out gesture))
            {
                switch (gesture)
                {
                    case TouchGestureType.MoveBottomToUp:
                        GestureText = @"Swipe form Bottom to Top";
                        break;

                    case TouchGestureType.MoveLeftToRight:
                        GestureText = @"Swipe form Left to Right";
                        break;

                    case TouchGestureType.MoveTopToBottom:
                        GestureText = @"Swipe form Top to Bottom";
                        break;

                    case TouchGestureType.MoveRightToLeft:
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
            out TouchGestureType gesture)
        {
            bool isSwipeGesture = false;

            gesture = TouchGestureType.None;

            if (Math.Abs(deltaY) > MinimumMoveDelta && Math.Abs(deltaY) > Math.Abs(deltaX) && Math.Abs(velocity.Y) >= LinearVelocity)
            {
                gesture = (deltaY > 0) ? TouchGestureType.MoveTopToBottom : TouchGestureType.MoveBottomToUp;
                isSwipeGesture = true;
            }

            if (Math.Abs(deltaX) > MinimumMoveDelta && Math.Abs(deltaX) > Math.Abs(deltaY) && Math.Abs(velocity.X) >= LinearVelocity)
            {
                gesture = (deltaX > 0) ? TouchGestureType.MoveLeftToRight : TouchGestureType.MoveRightToLeft;
                isSwipeGesture = true;
            }

            return isSwipeGesture;
        }

        public bool IsDoubleTap(Point point)
        {
            currentPoint = point;
            var withinRange = GetDistance(firstTap, point) < MaximumDoubleTapDistance;

            TimeSpan elapsed = stopwatch.Elapsed;
            var withinTime = elapsed != TimeSpan.Zero &&
                              elapsed < TimeSpan.FromSeconds(MaximumDoubleTapInterval);

            firstTap = point;
            stopwatch.Restart();

            return withinRange && withinTime;
        }

        /// <summary>
        /// Calculate the distance between two points
        /// </summary>
        /// <param name="p1">first point</param>
        /// <param name="p2">second point</param>
        /// <returns>the distance between the two points</returns>
        private double GetDistance(Point p1, Point p2)
        {
            double deltaX = p1.X - p2.X;
            double deltaY = p1.Y - p2.Y;
            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }

        private void MainWindow_OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            isDoubleTap = false;

            if (IsDoubleTap(e.ManipulationOrigin))
            {
                GestureText = @"Double Tap gesture";
                isDoubleTap = true;
            }
        }

        private void MainWindow_OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            //May be better to handle tap gesture here
            if (e.SystemGesture == SystemGesture.Tap)
            {
                GestureText = @"Tap gesture";
            }
        }

        /// <summary>
        /// Track the touch by device id
        /// </summary>
        /// <param name="deviceId">The device id</param>
        /// <param name="point">The touch point</param>
        public void TrackTouch(int deviceId, Point point)
        {
            if (!tracker.ContainsKey(deviceId))
            {
                tracker.Add(deviceId, new List<Point>());
            }

            tracker[deviceId].Add(point);
            currentPoint = point;
        }
    }
}