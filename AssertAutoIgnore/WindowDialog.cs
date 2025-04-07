namespace AssertAutoIgnore
{
    internal class WindowDialog
    {
        public string Title { get; }
        public int ButtonIndex { get; }

        public WindowDialog(string title, int buttonIndex)
        {
            Title = title;
            ButtonIndex = buttonIndex;
        }
    }
}