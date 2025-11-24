using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.UI.ViewModels;
using System;

namespace Mil.Paperwork.UI.Windows
{
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            // ensure theme variant follows application (native) theme
            if (Application.Current?.RequestedThemeVariant != null)
                this.RequestedThemeVariant = Application.Current.RequestedThemeVariant;


            if (DataContext is IWindowViewModel<DialogResult> vm)
            {
                vm.RequestClose += result =>
                {
                    // Close with result so ShowDialog returns it
                    this.Close(result);
                };
            }
        }
    }
}