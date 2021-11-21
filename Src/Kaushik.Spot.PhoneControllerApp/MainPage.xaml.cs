using System;
using System.Windows;
using System.Windows.Controls;
using Kaushik.Spot.Library;
using Microsoft.Phone.Controls;

namespace Kaushik.Spot.PhoneControllerApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        SpotServiceTest spotServiceProxy = null;
        SwitchService switchServiceProxy = null;
        BlinkLedService blinkLedServiceProxy = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            string encryptionKey = "avbi4s8h4a9l5kfadujskh4i9k50dj39405fk";
            ICryptographicServiceProvider cipher = new RC4Cipher(encryptionKey);

            spotServiceProxy = new SpotServiceTest(null, "192.167.1.107", 8080, SpotSocketProvider.Tcp);
            switchServiceProxy = new SwitchService(null, "192.167.1.107", 8080, SpotSocketProvider.Tcp);
            blinkLedServiceProxy = new BlinkLedService(null, "192.167.1.107", 8080, SpotSocketProvider.Tcp);

            addBlinkingStyles();
        }

        private void addBlinkingStyles()
        {
            ComboBoxItem item = null;

            item = new ComboBoxItem();
            item.Content = "Simple Blink";
            blinkingPattern.Items.Add(item);

            item = new ComboBoxItem();
            item.Content = "Progressive Blink";
            blinkingPattern.Items.Add(item);

            item = new ComboBoxItem();
            item.Content = "Always On";
            blinkingPattern.Items.Add(item);

            item = new ComboBoxItem();
            item.Content = "Always Off";
            blinkingPattern.Items.Add(item);

            blinkingPattern.SelectedIndex = 0;
        }


        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int sum = spotServiceProxy.Add(Convert.ToInt32(item1.Text), Convert.ToInt32(item2.Text));

                sumResult.Text = sum.ToString();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void sayHelloButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                helloResult.Text = spotServiceProxy.SayHello(name.Text);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void applyBlinkingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = blinkLedServiceProxy.ApplyPattern(blinkingPattern.SelectedIndex + 1);

                blinkLedResult.Text = String.Format("Blinking Style '{0}' done.", ((System.Windows.Controls.ContentControl)(blinkingPattern.SelectedValue)).Content.ToString());
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void switchButton1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool switchOn = (switchButton1.Content.ToString().IndexOf(" On ") > -1);

                bool result = switchServiceProxy.ApplySwitchState(switchOn, 1);

                if (switchOn)
                {
                    switchButton1.Content = "Switch Off # 1";
                }
                else
                {
                    switchButton1.Content = "Switch On # 1";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void switchButton2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool switchOn = (switchButton2.Content.ToString().IndexOf(" On ") > -1);

                bool result = switchServiceProxy.ApplySwitchState(switchOn, 2);

                if (switchOn)
                {
                    switchButton2.Content = "Switch Off # 2";
                }
                else
                {
                    switchButton2.Content = "Switch On # 2";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void switchButton3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool switchOn = (switchButton3.Content.ToString().IndexOf(" On ") > -1);

                bool result = switchServiceProxy.ApplySwitchState(switchOn, 3);

                if (switchOn)
                {
                    switchButton3.Content = "Switch Off # 3";
                }
                else
                {
                    switchButton3.Content = "Switch On # 3";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void switchButton4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool switchOn = (switchButton3.Content.ToString().IndexOf(" On ") > -1);

                bool result = switchServiceProxy.ApplySwitchState(switchOn, 4);

                if (switchOn)
                {
                    switchButton3.Content = "Switch Off # 4";
                }
                else
                {
                    switchButton3.Content = "Switch On # 4";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

    }
}