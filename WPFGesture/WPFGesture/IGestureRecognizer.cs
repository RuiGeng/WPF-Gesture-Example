namespace WPFGesture
{
    using System.Windows;

    public interface IGestureRecognizer
    {
        TouchGestureType Gesture { get; }

        double MaxRightPosition { get; }

        double MaxLeftPosition { get; }

        Point CurrentPoint { get; }

        void TrackTouch(int deviceId, Point point);

        void ClearTrackTouch();

        bool IsDoubleTap(Point point);
    }
}