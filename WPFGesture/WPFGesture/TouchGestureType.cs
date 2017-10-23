namespace WPFGesture
{
    internal static class TouchGestureType
    {
        public enum TouchGesture
        {
            /// <summary>
            /// Not a manipulation gesture
            /// </summary>
            None = 0,

            /// <summary>
            /// two touch points moved closer
            /// </summary>
            Pinch = 1,

            /// <summary>
            /// towards top of screen
            /// </summary>
            MoveUp = 2,

            /// <summary>
            /// towards right of screen
            /// </summary>
            MoveRight = 4,

            /// <summary>
            /// towards bottom of screen
            /// </summary>
            MoveDown = 8,

            /// <summary>
            /// towards left of screen
            /// </summary>
            MoveLeft = 16,
        }
    }
}