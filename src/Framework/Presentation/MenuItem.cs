using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kogler.Framework
{
    [ContentProperty("Items")]
    public class MenuItem : PropertyChangedBase
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
            set
            {
                Set(ref groupName, value);
                HierrarchyGroupName = value;
            }
        }

        public const string GroupNameSeparator = "||";

        internal string HierrarchyGroupName;

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

        //private Image image;
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

        internal IEnumerable<MenuItem> Deep => new[] {this}.Union(Items.SelectMany(i => i.Deep));

        private ICommand command;
        public ICommand Command
        {
            get { return command; }
            set { Set(ref command, value); }
        }

        private object commandParameter;
        public object CommandParameter
        {
            get { return commandParameter; }
            set { Set(ref commandParameter, value); }
        }

        public MenuItemsCollection Items { get; }
    }
}