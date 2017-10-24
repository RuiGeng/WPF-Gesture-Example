using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace WPFGesture
{
    public enum TouchGestureType
    {
        /// <summary>
        /// Not a manipulation gesture
        /// </summary>
        None,

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
        /// minimum movement to be recognized as a gesture
        /// </summary>
        private const int MinimumMove = 10;

        /// <summary>
        /// minimum angle to be recognized as non-axial swipe
        /// </summary>
        private const int MinimumAngle = 6;

        /// <summary>
        /// maximum distance between two points to be considered double tap
        /// </summary>
        private const double MaximumDoubleTapDistance = 20;

        /// <summary>
        /// maximum time interval between two touches to be considered double tap
        /// </summary>
        private const double MaximumDoubleTapInterval = 0.3;

        /// <summary>
        /// stop watch for recognizing double tap
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// track the touches by device id
        /// </summary>
        private Dictionary<int, List<Point>> tracker;

        /// <summary>
        /// hold the first tap for double tap
        /// </summary>
        private Point firstTap;

        /// <summary>
        /// The single touch action current point
        /// </summary>
        private Point currentPoint;


        /// <summary>
        /// Gets the maximum right position of the manipulation
        /// </summary>
        public double MaxRightPosition
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the maximum left position of the manipulation
        /// </summary>
        public double MaxLeftPosition
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the gesture type
        /// </summary>
        public TouchGestureType Gesture => InterpretGesture();

        public GestureRecognizer()
        {
            tracker = new Dictionary<int, List<Point>>();
            firstTap = new Point();
            currentPoint = new Point();
        }



        /// <summary>
        /// Recognize the gesture by the tracked data
        /// </summary>
        /// <returns>the type of gesture, if recognized</returns>
        private TouchGestureType InterpretGesture()
        {
            TouchGestureType resultType = TouchGestureType.None;

            if (tracker.Count == 2)
            {
                // for now, assume it is a pinch/expand type
                resultType = TouchGestureType.Pinch;
            }
            else if (tracker.Count == 1)
            {
                IEnumerator it = tracker.Values.GetEnumerator();
                if (!it.MoveNext())
                {
                    resultType = TouchGestureType.None;
                }
                else
                {
                    List<Point> points = (List<Point>)it.Current;

                    // get first and last point from the list of points
                    Point firstPoint = (Point)points[0];
                    Point lastPoint = (Point)points[points.Count - 1];

                    // horizontal and vertical movement
                    double deltaX = lastPoint.X - firstPoint.X;
                    double deltaY = lastPoint.Y - firstPoint.Y;

                    ////  Update the left & right moving edge
                    if (deltaX > 0 && deltaX > MaxRightPosition)
                    {
                        MaxRightPosition = deltaX;
                    }
                    else if (deltaX < 0 && deltaX < MaxLeftPosition)
                    {
                        MaxLeftPosition = deltaX;
                    }

                    // check if distance travelled on X and Y axis is more the minimum 
                    // gesture length specified above
                    if (Math.Abs(deltaY) > MinimumMove && Math.Abs(deltaY) > Math.Abs(deltaX))
                    {
                        // if yDiff is negative, we moved upwards
                        resultType = (deltaY > 0) ? TouchGestureType.MoveBottomToUp : TouchGestureType.MoveTopToBottom;
                    }

                    if (Math.Abs(deltaX) > MinimumMove && Math.Abs(deltaX) > Math.Abs(deltaY))
                    {
                        // if xDiff is negative, we moved left
                        resultType = (deltaX > 0) ? TouchGestureType.MoveLeftToRight : TouchGestureType.MoveRightToLeft;
                    }
                }
            }

            return resultType;
        }
    }
}