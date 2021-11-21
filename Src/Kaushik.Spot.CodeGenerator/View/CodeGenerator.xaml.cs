using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Reflection;

namespace Kaushik.Spot.CodeGenerator.View
{
    /// <summary>
    /// Interaction logic for CodeGenerator.xaml
    /// </summary>
    public partial class CodeGenerator : Window
    {
        public CodeGenerator()
        {
            InitializeComponent();

            this.DataContext = new ViewModel.ServiceProxyMetaDataViewModel(new Model.ServiceProxyMetaData());
        }
    }
}
