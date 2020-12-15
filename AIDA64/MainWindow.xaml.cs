using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace AIDA64
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AIDA64Base m_Base { get; } = new AIDA64Base();
        DispatcherTimer UpdateTimer { get; } = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            UpdateData();
            UpdateTimer.Interval = TimeSpan.FromSeconds(0.5);
            UpdateTimer.Tick += OnTime;
            UpdateTimer.Start();

            this.DataContext = m_Base;
        }

        private void OnTime(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            using (var sf = new SharedMemSaver())
            {
                sf.OpenView();
                m_Base.Parser(sf.GetData());
            }
        }
    }
}
