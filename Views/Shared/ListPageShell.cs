using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace RestaurantApp.Views.Shared
{
    [ContentProperty(nameof(Body))]
    public partial class ListPageShell : UserControl
    {
        public ListPageShell()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ListPageShell),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty CountTextProperty =
            DependencyProperty.Register(
                nameof(CountText),
                typeof(string),
                typeof(ListPageShell),
                new PropertyMetadata(string.Empty));

        public string CountText
        {
            get => (string)GetValue(CountTextProperty);
            set => SetValue(CountTextProperty, value);
        }

        public static readonly DependencyProperty ToolbarContentProperty =
    DependencyProperty.Register(
        nameof(ToolbarContent),
        typeof(object),
        typeof(ListPageShell),
        new PropertyMetadata(null));

        public object ToolbarContent
        {
            get => GetValue(ToolbarContentProperty);
            set => SetValue(ToolbarContentProperty, value);
        }


        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register(nameof(Body), typeof(object), typeof(ListPageShell),
                new PropertyMetadata(null));

        public object Body
        {
            get => GetValue(BodyProperty);
            set => SetValue(BodyProperty, value);
        }


    }
}
