namespace WPFGesture
{
    using System;

    public interface IGestureContainer
    {
        void EnableGestureRecognizing();

        void DisableGestureRecognizing();

        Action DoubleTapAction { get; set; }
    }
}