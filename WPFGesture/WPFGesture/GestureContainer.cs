using System;
using System.Windows;
using System.Windows.Input;

namespace WPFGesture
{
    public class GestureContainer : IGestureContainer
    {
        private readonly UIElement uiElement;

        private GestureRecognizer gestureRecognizer;
        private bool isDoubleTapping;
        private Point originalPoint;

        public Action DoubleTouchAction;
        public Action PinchAction;
        public Action SingleTapAction;
        public Action SwipDownAction;
        public Action SwipLeftAction;
        public Action SwipRigthAction;
        public Action SwipUpAction;

        public GestureContainer(UIElement uiElement)
        {
            this.uiElement = uiElement;
        }

        public void EnableGestureRecognizing()
        {
            if (uiElement != null)
            {
                gestureRecognizer = new GestureRecognizer();

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
                gestureRecognizer = null;

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
            }
            else if (gesture == TouchGestureType.MoveRightToLeft)
            {
            }
            else if (gesture == TouchGestureType.MoveLeftToRight)
            {
            }
            else if (gesture == TouchGestureType.MoveBottomToUp)
            {
            }
            else if (gesture == TouchGestureType.MoveTopToBottom)
            {
            }
            else if (gesture == TouchGestureType.SingleTap)
            {
            }
        }

        private void UiElementOnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            foreach (var m in e.Manipulators)
                gestureRecognizer.TrackTouch(m.Id, m.GetPosition(uiElement));
        }

        private void UiElementOnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            isDoubleTapping = false;

            if (uiElement != null)
                originalPoint = uiElement.PointToScreen(e.ManipulationOrigin);

            if (gestureRecognizer.IsDoubleTap(originalPoint))
                isDoubleTapping = true;
        }


        private void UiElementOnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // Provides data for the ManipulationInertiaStarting event.

            //// Decrease the velocity of the Rectangle's movement by 
            //// 10 inches per second every second.
            //// (10 inches * 96 pixels per inch / 1000ms^2)
            //e.TranslationBehavior.DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0);

            //// Decrease the velocity of the Rectangle's resizing by 
            //// 0.1 inches per second every second.
            //// (0.1 inches * 96 pixels per inch / (1000ms^2)
            //e.ExpansionBehavior.DesiredDeceleration = 0.1 * 96 / (1000.0 * 1000.0);

            //// Decrease the velocity of the Rectangle's rotation rate by 
            //// 2 rotations per second every second.
            //// (2 * 360 degrees / (1000ms^2)
            //e.RotationBehavior.DesiredDeceleration = 720 / (1000.0 * 1000.0);

            //e.Handled = true;
        }

        private void UiElementOnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            gestureRecognizer.ClearTrackTouch();
            e.ManipulationContainer = uiElement;
            e.Handled = true;
        }
    }
}