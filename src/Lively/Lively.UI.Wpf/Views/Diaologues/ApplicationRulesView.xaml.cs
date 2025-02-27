﻿using Lively.UI.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Lively.UI.Wpf.Views.Dialogues
{
    /// <summary>
    /// Interaction logic for ApplicationRulesView.xaml
    /// </summary>
    public partial class ApplicationRulesView : Window
    {
        public ApplicationRulesView()
        {
            InitializeComponent();
            var vm = App.Services.GetRequiredService<ApplicationRulesViewModel>();
            this.DataContext = vm;
            this.Closing += vm.OnWindowClosing;
        }
    }
}
