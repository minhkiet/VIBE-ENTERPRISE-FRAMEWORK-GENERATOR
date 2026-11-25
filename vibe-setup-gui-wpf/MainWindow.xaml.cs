using System.Windows;
using System.Windows.Input;
using CursorSetupWpf.Services;
using CursorSetupWpf.ViewModels;

namespace CursorSetupWpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LocalizationService.CultureChanged += () => { };
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is not MainViewModel vm) return;
            var modifiers = Keyboard.Modifiers;

            if (modifiers == ModifierKeys.Control && e.Key == Key.K)
            {
                NavSearchBox.Focus();
                NavSearchBox.SelectAll();
                e.Handled = true;
                return;
            }

            if (modifiers == ModifierKeys.Control && e.Key == Key.L)
            {
                vm.ToggleLogCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape && NavSearchBox.IsFocused)
            {
                vm.NavSearchText = "";
                NavSearchBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
                return;
            }

            if (modifiers != ModifierKeys.Control) return;
            int? page = e.Key switch
            {
                Key.D1 => 0,
                Key.D2 => 1,
                Key.D3 => 2,
                Key.D4 => 3,
                Key.D5 => 4,
                Key.D6 => 5,
                Key.D7 => 6,
                Key.D8 => 7,
                Key.D9 => 8,
                Key.D0 => 9,
                _ => null,
            };
            if (page.HasValue)
            {
                vm.GoToPageCommand.Execute(page.Value);
                e.Handled = true;
            }
        }
    }
}
