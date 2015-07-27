using System;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kogler.Framework
{
    [ContentProperty("Items")]
    public class MenuItem : Model
    {
        public MenuItem()
        {
            Items = new MenuItemsCollection(this);
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { Set(ref text, value); }
        }

        private string groupName;
        public string GroupName
        {
            get { return groupName; }
            set { Set(ref groupName, value); }
        } 

        private bool isSeparator;
        public bool IsSeparator
        {
            get { return isSeparator; }
            set { Set(ref isSeparator, value); }
        }

        private bool isCheckable;
        public bool IsCheckable
        {
            get { return isCheckable; }
            set { Set(ref isCheckable, value); }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { Set(ref isChecked, value); }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { Set(ref isEnabled, value); }
        }

        private bool staysOpenOnClick;
        public bool StaysOpenOnClick
        {
            get { return staysOpenOnClick; }
            set { Set(ref staysOpenOnClick, value); }
        }

        private string imageUri;
        public string ImageUri
        {
            get { return imageUri; }
            set { Set(ref imageUri, value, nameof(ImageUri), nameof(Image)); }
        }

        private Image image;
        public Image Image
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUri)) return null;
                return new Image()
                {
                    Source = new BitmapImage(new Uri(ImageUri, UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.None
                };
            }
        }

        private MenuItem parent;
        public MenuItem Parent
        {
            get { return parent; }
            set { Set(ref parent, value); }
        }

        public MenuItemsCollection Items { get; }
    }
}