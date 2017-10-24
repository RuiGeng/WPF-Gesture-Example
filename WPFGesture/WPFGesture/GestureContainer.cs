using System;
using System.Windows;
using System.Windows.Input;

namespace WPFGesture
{
    public class GestureContainer
    {
        private UIElement uiElement;

        public GestureContainer(UIElement uiElement)
        {
            this.uiElement = uiElement;

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

        private void UiElementOnManipulationCompleted(object sender, ManipulationCompletedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void UiElementOnManipulationDelta(object sender, ManipulationDeltaEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void UiElementOnManipulationStarted(object sender, ManipulationStartedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void UiElementOnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void UiElementOnManipulationStarting(object sender, ManipulationStartingEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}