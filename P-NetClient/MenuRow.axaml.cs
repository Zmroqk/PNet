using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace PNetClient
{
    public partial class MenuRow : UserControl
    {
        public static readonly StyledProperty<EventHandler<RoutedEventArgs>> MenuButtonClickProperty =
            AvaloniaProperty.Register<MenuRow, EventHandler<RoutedEventArgs>>(nameof(MenuButtonClick));

        public static readonly StyledProperty<IImage> FirstImagePathProperty =
             AvaloniaProperty.Register<MenuRow, IImage>(nameof(FirstImagePath));

        public static readonly StyledProperty<IImage> SecondImagePathProperty =
            AvaloniaProperty.Register<MenuRow, IImage>(nameof(SecondImagePath));

        public static readonly StyledProperty<bool> UseButtonProperty =
            AvaloniaProperty.Register<MenuRow, bool>(nameof(UseButton));

        public static readonly StyledProperty<bool> UseSubmenuProperty =
            AvaloniaProperty.Register<MenuRow, bool>(nameof(UseButton));

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<MenuRow, string>(nameof(Text));

        public IImage FirstImagePath
        {
            get { return GetValue(FirstImagePathProperty); }
            set { SetValue(FirstImagePathProperty, value); }
        }

        public IImage SecondImagePath
        {
            get { return GetValue(SecondImagePathProperty); }
            set { SetValue(SecondImagePathProperty, value); }
        }

        public bool UseButton
        {
            get { return GetValue(UseButtonProperty); }
            set { SetValue(UseButtonProperty, value); }
        }

        public bool UseSubmenu
        {
            get { return GetValue(UseSubmenuProperty); }
            set { SetValue(UseSubmenuProperty, value); }
        }

        public string Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public EventHandler<RoutedEventArgs> MenuButtonClick
        {
            get { return GetValue(MenuButtonClickProperty); }
            set { SetValue(MenuButtonClickProperty, value); }
        }

        public MenuRow()
        {
            InitializeComponent();
            DataContext = this;
            Text = "Testy: ";
            UseButton = true;
            UseSubmenu = false;
            SubmenuStack = this.FindControl<StackPanel>("SubmenuStack");
            SubmenuStack.DataContext = SubmenuStack;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            MenuButtonClick?.Invoke(sender, e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
