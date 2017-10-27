namespace WPFGesture
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;

    public enum TouchGestureType
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        SingleTap,

        /// <summary>
        /// two touch points moved closer
        /// </summary>
        Pinch,

        /// <summary>
        /// touch point(s) moved in same direction: towards top of screen
        /// </summary>
        MoveBottomToUp,

        /// <summary>
        /// touch point(s) moved in same direction: towards right of screen
        /// </summary>
        MoveLeftToRight,

        /// <summary>
        /// touch point(s) moved in same direction: towards bottom of screen
        /// </summary>
        MoveTopToBottom,

        /// <summary>
        /// touch point(s) moved in same direction: towards left of screen
        /// </summary>
        MoveRightToLeft
    }

    public class GestureRecognizer
    {
        /// <summary>
        /// maximum distance between two points to be considered double tap
        /// </summary>
        private const double MaximumDoubleTapDistance = 20;

        /// <summary>
        /// maximum time interval between two touches to be considered double tap
        /// </summary>
        private const double MaximumDoubleTapInterval = 0.3;

        /// <summary>
        /// minimum angle to be recognized as non-axial swipe
        /// </summary>
        private const int MinimumAngle = 6;

        /// <summary>
        /// minimum movement to be recognized as a gesture
        /// </summary>
        private const int MinimumMove = 10;

        /// <summary>
        /// stop watch for recognizing double tap
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// hold the first tap for double tap
        /// </summary>
        private Point firstTap;

        /// <summary>
        /// track the touches by device id
        /// </summary>
        private readonly Dictionary<int, List<Point>> pointTracker;

        public GestureRecognizer()
        {
            pointTracker = new Dictionary<int, List<Point>>();
            firstTap = new Point();
            CurrentPoint = new Point();
        }

        /// <summary>
        /// Gets the gesture type
        /// </summary>
        public TouchGestureType Gesture => InterpretGesture();

        /// <summary>
        /// Gets the maximum left position of the manipulation
        /// </summary>
        public double MaxLeftPosition { get; private set; }

        /// <summary>
        /// Gets the maximum right position of the manipulation
        /// </summary>
        public double MaxRightPosition { get; private set; }

        public Point CurrentPoint { get; private set; }

        /// <summary>
        /// Clear the tracked touches
        /// </summary>
        public void ClearTrackTouch()
        {
            pointTracker.Clear();
            MaxLeftPosition = 0;
            MaxRightPosition = 0;
        }

        /// <summary>
        /// Check if it is a double tap at the point
        /// </summary>
        /// <param name="point">The touch point</param>
        /// <returns>True if it is a double tap, false otherwise</returns>
        public bool IsDoubleTap(Point point)
        {
            CurrentPoint = point;
            bool withinRange = GetDistance(firstTap, point) < MaximumDoubleTapDistance;

            TimeSpan elapsed = stopwatch.Elapsed;
            bool withinTime = elapsed != TimeSpan.Zero &&
                              elapsed < TimeSpan.FromSeconds(MaximumDoubleTapInterval);

            firstTap = point;
            stopwatch.Restart();

            return withinRange && withinTime;
        }

        /// <summary>
        /// Track the touch by device id
        /// </summary>
        /// <param name="deviceId">The device id</param>
        /// <param name="point">The touch point</param>
        public void TrackTouch(int deviceId, Point point)
        {
            if (!pointTracker.ContainsKey(deviceId))
            {
                pointTracker.Add(deviceId, new List<Point>());
            }

            pointTracker[deviceId].Add(point);
            CurrentPoint = point;
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

        /// <summary>
        /// Recognize the gesture by the tracked data
        /// </summary>
        /// <returns>the type of gesture, if recognized</returns>
        private TouchGestureType InterpretGesture()
        {
            TouchGestureType resultType = TouchGestureType.None;

            if (pointTracker.Count == 2)
            {
                resultType = TouchGestureType.Pinch;
            }
            else if (pointTracker.Count == 1)
            {
                IEnumerator it = pointTracker.Values.GetEnumerator();
                if (!it.MoveNext())
                {
                    resultType = TouchGestureType.SingleTap;
                }
                else
                {
                    List<Point> points = (List<Point>)it.Current;

                    // get first and last point from the list of points
                    if (points != null)
                    {
                        Point firstPoint = points[0];
                        Point lastPoint = points[points.Count - 1];

                        // horizontal and vertical movement
                        double deltaX = lastPoint.X - firstPoint.X;
                        double deltaY = lastPoint.Y - firstPoint.Y;

                        // Update the left & right moving edge
                        if (deltaX > 0 && deltaX > MaxRightPosition)
                        {
                            MaxRightPosition = deltaX;
                        }
                        else if (deltaX < 0 && deltaX < MaxLeftPosition)
                        {
                            MaxLeftPosition = deltaX;
                        }

                        // check if distance traveled on X and Y axis is more the minimum
                        // gesture length specified above
                        if (Math.Abs(deltaY) > MinimumMove && Math.Abs(deltaY) > Math.Abs(deltaX))
                        {
                            // if yDiff is negative, we moved upwards
                            resultType = (deltaY > 0) ? TouchGestureType.MoveTopToBottom : TouchGestureType.MoveBottomToUp;
                        }

                        if (Math.Abs(deltaX) > MinimumMove && Math.Abs(deltaX) > Math.Abs(deltaY))
                        {
                            // if xDiff is negative, we moved left
                            resultType = (deltaX > 0) ? TouchGestureType.MoveLeftToRight : TouchGestureType.MoveRightToLeft;
                        }
                    }
                }
            }

            return resultType;
        }
    }
}