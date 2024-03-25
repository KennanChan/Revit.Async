#region Reference

using System.Windows;
using System.Windows.Controls;

#endregion

namespace TestCommand
{
    internal class TestWindow : Window
    {
        #region Constructors

        public TestWindow()
        {
            InitializeComponents();
        }

        #endregion

        #region Others

        private void InitializeComponents()
        {
            Width = 200;
            Height = 100;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var button = new Button
            {
                Content = new TextBlock { Text = "Save Random Family" },
                Command = new SaveFamilyCommand(),
                CommandParameter = true,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Content = button;
        }

        #endregion
    }
}