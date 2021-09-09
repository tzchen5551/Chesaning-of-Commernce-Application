using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DirectorsPortalWPF.Controls
{
    /// <summary>
    /// Interaction logic for ExtraField.xaml
    /// </summary>
    public partial class ExtraField : UserControl
    {
        public string GStrFieldName { get; set; } = string.Empty;

        public ExtraField()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
