using System;

namespace Kogler.Framework
{
    public interface IShellViewModel { }

    public interface IShellWindow
    {
        double Left { get; set; }
        double Top { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        bool IsMaximized { get; set; }

        event EventHandler Closed;

        void Show();
        void Close();
    }
}