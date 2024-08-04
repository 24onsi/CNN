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
using System.Net.Mail;
using ART.Model;

namespace ART.View
{
    public partial class Ask : Page
    {
        public Ask()
        {
            InitializeComponent();
        }

        public static Send_Email MailInfo = new Send_Email();
        public static Email Email = new Email();

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Clear();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            Init_EmailInfo();

            Email.Send_Email();

            MessageBox.Show("메일 전송이 완료되었습니다.");

        }

        private void Init_EmailInfo()
        {
            MailInfo.To = "uy1224@naver.com";
            MailInfo.From = ask_email.Text.ToString();
            MailInfo.Titile = ask_title.Text.ToString();
            MailInfo.Content = ask_content.Text.ToString();
        }
    }
}
