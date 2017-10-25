namespace WPFGesture
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    public class GestureContainer
    {
        private readonly UIElement uiElement;
        private readonly IGestureRecognizer gestureRecognizer;
        private bool isDoubleTapping;
        private Point originalPoint;

        public GestureContainer(UIElement uiElement, IGestureRecognizer gestureRecognizer)
        {
            this.uiElement = uiElement;
            this.gestureRecognizer = gestureRecognizer;
        }

        public void EnableGestureRecognizing()
        {
            if (uiElement != null)
            {
                //enabled manipulation events on this UIElement.
                uiElement.IsManipulationEnabled = true;

                //Occurs when the input device loses contact with the UIElement object during a manipulation and inertia begins.
                uiElement.ManipulationInertiaStarting += UiElementOnManipulationInertiaStarting;

                //Occurs when the manipulation processor is first created.
                uiElement.ManipulationStarting += UiElementOnManipulationStarting;

                //Occurs when an input device begins a manipulation on the UIElement object.
                uiElement.ManipulationStarted += UiElementOnManipulationStarted;

                //Occurs when the input device changes position during a manipulation.
                uiElement.ManipulationDelta += UiElementOnManipulationDelta;

                //Occurs when a manipulation and inertia on the UIElement object is complete.
                uiElement.ManipulationCompleted += UiElementOnManipulationCompleted;
            }
        }

        public void DisableGestureRecognizing()
        {
            if (uiElement != null && uiElement.IsManipulationEnabled)
            {
                //disable manipulation events on this UIElement.
                uiElement.IsManipulationEnabled = false;

                //Occurs when the input device loses contact with the UIElement object during a manipulation and inertia begins.
                uiElement.ManipulationInertiaStarting -= UiElementOnManipulationInertiaStarting;

                //Occurs when the manipulation processor is first created.
                uiElement.ManipulationStarting -= UiElementOnManipulationStarting;

                //Occurs when an input device begins a manipulation on the UIElement object.
                uiElement.ManipulationStarted -= UiElementOnManipulationStarted;

                //Occurs when the input device changes position during a manipulation.
                uiElement.ManipulationDelta -= UiElementOnManipulationDelta;

                //Occurs when a manipulation and inertia on the UIElement object is complete.
                uiElement.ManipulationCompleted -= UiElementOnManipulationCompleted;
            }
        }

        private void UiElementOnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            var gesture = gestureRecognizer.Gesture;

            if (isDoubleTapping && Math.Abs(e.TotalManipulation.Translation.X) < 0.02)
            {
                //Double Touch logic
            }
            else if (gesture == TouchGestureType.MoveRightToLeft ||
                     gesture == TouchGestureType.MoveLeftToRight)
            {
                //Swipe logic
            }
            else if (gestureRecognizer.Gesture == TouchGestureType.None)
            {
                //Single Touch logic
            }
        }

        private void UiElementOnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            foreach (var m in e.Manipulators)
            {
                gestureRecognizer.TrackTouch(m.Id, m.GetPosition(uiElement));
            }

            switch (gestureRecognizer.Gesture)
            {
                case TouchGestureType.Pinch:
                    break;

                case TouchGestureType.MoveRightToLeft:
                    break;

                case TouchGestureType.MoveLeftToRight:
                    break;

                case TouchGestureType.MoveBottomToUp:
                    break;

                case TouchGestureType.MoveTopToBottom:
                    break;
            }
        }

        private void UiElementOnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            isDoubleTapping = false;

            if (uiElement != null)
            {
                originalPoint = uiElement.PointToScreen(e.ManipulationOrigin);
            }

            if (gestureRecognizer.IsDoubleTap(originalPoint))
            {
                isDoubleTapping = true;
            }
        }

        private void UiElementOnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UiElementOnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            gestureRecognizer.ClearTrackTouch();
            e.ManipulationContainer = uiElement;
            e.Handled = true;
        }
    }
}