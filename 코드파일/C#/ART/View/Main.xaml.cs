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
using ART.Model;

namespace ART.View
{
    public partial class Main : Page
    {
        public static Server CppSV = new Server();
        public static Data data = new Data();
        public static Check check = new Check();

        public Main()
        {
            InitializeComponent();
        }

        private void btn_painter_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/Paint.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/Login.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            if (check.Login_Check)
            {
                Uri uri = new Uri("/View/Search.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else
            {
                MessageBox.Show("로그인이 필요한 서비스 입니다.");
            }
        }

        private void btn_ask_Click(object sender, RoutedEventArgs e)
        {
            if(check.Login_Check)
            {
                Uri uri = new Uri("/View/Ask.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else
            {
                MessageBox.Show("로그인이 필요한 서비스 입니다.");
            }
        }
    }
}
