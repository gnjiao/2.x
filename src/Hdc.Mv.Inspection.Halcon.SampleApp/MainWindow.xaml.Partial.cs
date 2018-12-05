using System.Windows;
using Microsoft.Practices.Prism.Commands;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public partial class MainWindow
    {
        public DelegateCommand OpenFile00Command { get; set; }
        public DelegateCommand OpenFile01Command { get; set; }
        public DelegateCommand OpenFile02Command { get; set; }
        public DelegateCommand OpenFile03Command { get; set; }
        public DelegateCommand OpenFile04Command { get; set; }
        public DelegateCommand OpenFile05Command { get; set; }
        public DelegateCommand OpenFile06Command { get; set; }
        public DelegateCommand OpenFile07Command { get; set; }
        public DelegateCommand OpenFile08Command { get; set; }

        public DelegateCommand OpenFile100Command { get; set; }
        public DelegateCommand OpenFile101Command { get; set; }
        public DelegateCommand OpenFile102Command { get; set; }
        public DelegateCommand OpenFile103Command { get; set; }
        public DelegateCommand OpenFile104Command { get; set; }
        public DelegateCommand OpenFile105Command { get; set; }
        public DelegateCommand OpenFile106Command { get; set; }
        public DelegateCommand OpenFile107Command { get; set; }
        public DelegateCommand OpenFile108Command { get; set; }

        public DelegateCommand OpenFile200Command { get; set; }
        public DelegateCommand OpenFile201Command { get; set; }
        public DelegateCommand OpenFile202Command { get; set; }
        public DelegateCommand OpenFile203Command { get; set; }
        public DelegateCommand OpenFile204Command { get; set; }
        public DelegateCommand OpenFile205Command { get; set; }
        public DelegateCommand OpenFile206Command { get; set; }
        public DelegateCommand OpenFile207Command { get; set; }
        public DelegateCommand OpenFile208Command { get; set; }

        private void OpenFileUseSeparateSchema00CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile00Command.Execute();
        private void OpenFileUseSeparateSchema01CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile01Command.Execute();
        private void OpenFileUseSeparateSchema02CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile02Command.Execute();
        private void OpenFileUseSeparateSchema03CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile03Command.Execute();
        private void OpenFileUseSeparateSchema04CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile04Command.Execute();
        private void OpenFileUseSeparateSchema05CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile05Command.Execute();
        private void OpenFileUseSeparateSchema06CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile06Command.Execute();
        private void OpenFileUseSeparateSchema07CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile07Command.Execute();
        private void OpenFileUseSeparateSchema08CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile08Command.Execute();

        private void OpenFileUseSeparateSchema100CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile100Command.Execute();
        private void OpenFileUseSeparateSchema101CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile101Command.Execute();
        private void OpenFileUseSeparateSchema102CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile102Command.Execute();
        private void OpenFileUseSeparateSchema103CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile103Command.Execute();
        private void OpenFileUseSeparateSchema104CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile104Command.Execute();
        private void OpenFileUseSeparateSchema105CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile105Command.Execute();
        private void OpenFileUseSeparateSchema106CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile106Command.Execute();
        private void OpenFileUseSeparateSchema107CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile107Command.Execute();
        private void OpenFileUseSeparateSchema108CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile108Command.Execute();

        private void OpenFileUseSeparateSchema200CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile200Command.Execute();
        private void OpenFileUseSeparateSchema201CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile201Command.Execute();
        private void OpenFileUseSeparateSchema202CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile202Command.Execute();
        private void OpenFileUseSeparateSchema203CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile203Command.Execute();
        private void OpenFileUseSeparateSchema204CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile204Command.Execute();
        private void OpenFileUseSeparateSchema205CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile205Command.Execute();
        private void OpenFileUseSeparateSchema206CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile206Command.Execute();
        private void OpenFileUseSeparateSchema207CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile207Command.Execute();
        private void OpenFileUseSeparateSchema208CommandButton_OnClick(object sender, RoutedEventArgs e) => OpenFile208Command.Execute();

        private void CtorOpenFileCommands()
        {
            OpenFile00Command = new DelegateCommand(() => OpenFileXCommandInner(0));
            OpenFile01Command = new DelegateCommand(() => OpenFileXCommandInner(1));
            OpenFile02Command = new DelegateCommand(() => OpenFileXCommandInner(2));
            OpenFile03Command = new DelegateCommand(() => OpenFileXCommandInner(3));
            OpenFile04Command = new DelegateCommand(() => OpenFileXCommandInner(4));
            OpenFile05Command = new DelegateCommand(() => OpenFileXCommandInner(5));
            OpenFile06Command = new DelegateCommand(() => OpenFileXCommandInner(6));
            OpenFile07Command = new DelegateCommand(() => OpenFileXCommandInner(7));
            OpenFile08Command = new DelegateCommand(() => OpenFileXCommandInner(8));

            OpenFile100Command = new DelegateCommand(() => OpenFileXCommandInner(100));
            OpenFile101Command = new DelegateCommand(() => OpenFileXCommandInner(101));
            OpenFile102Command = new DelegateCommand(() => OpenFileXCommandInner(102));
            OpenFile103Command = new DelegateCommand(() => OpenFileXCommandInner(103));
            OpenFile104Command = new DelegateCommand(() => OpenFileXCommandInner(104));
            OpenFile105Command = new DelegateCommand(() => OpenFileXCommandInner(105));
            OpenFile106Command = new DelegateCommand(() => OpenFileXCommandInner(106));
            OpenFile107Command = new DelegateCommand(() => OpenFileXCommandInner(107));
            OpenFile108Command = new DelegateCommand(() => OpenFileXCommandInner(108));

            OpenFile200Command = new DelegateCommand(() => OpenFileXCommandInner(200));
            OpenFile201Command = new DelegateCommand(() => OpenFileXCommandInner(201));
            OpenFile202Command = new DelegateCommand(() => OpenFileXCommandInner(202));
            OpenFile203Command = new DelegateCommand(() => OpenFileXCommandInner(203));
            OpenFile204Command = new DelegateCommand(() => OpenFileXCommandInner(204));
            OpenFile205Command = new DelegateCommand(() => OpenFileXCommandInner(205));
            OpenFile206Command = new DelegateCommand(() => OpenFileXCommandInner(206));
            OpenFile207Command = new DelegateCommand(() => OpenFileXCommandInner(207));
            OpenFile208Command = new DelegateCommand(() => OpenFileXCommandInner(208));
        }
    }
}