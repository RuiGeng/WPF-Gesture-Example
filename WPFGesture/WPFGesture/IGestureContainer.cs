namespace WPFGesture
{
    using System;

    public interface IGestureContainer
    {
        void EnableGestureRecognizing();

        void DisableGestureRecognizing();

        Action DoubleTapAction { get; set; }
        Action PinchAction { get; set; }
        Action SingleTapAction { get; set; }
        Action SwipeDownAction { get; set; }
        Action SwipeLeftAction { get; set; }
        Action SwipeRightAction { get; set; }
        Action SwipeUpAction { get; set; }
    }
}