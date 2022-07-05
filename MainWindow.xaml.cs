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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Globalization;
using System.IO;
using Microsoft.Win32;

namespace WallpaperingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int buttonSize = 64;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeWindow();

            meBackground.Source = new Uri(Directory.GetCurrentDirectory() + "\\" + Properties.Settings.Default.backgroundImage, UriKind.Absolute);

            switch (Properties.Settings.Default.invertClock)
            {
                case true:
                    {
                        lblClock.Foreground = Brushes.White;
                        lblDate.Foreground = Brushes.White;
                        break;
                    }
                case false:
                    {
                        lblClock.Foreground = Brushes.Black;
                        lblDate.Foreground = Brushes.Black;
                        break;
                    }
            }

            DispatcherTimer clockTimer = new DispatcherTimer();
            clockTimer.Tick += delegate (object? sender, EventArgs e)
            {
                UpdateClock();
            };
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            clockTimer.Start();

            InvertClock.IsChecked = Properties.Settings.Default.invertClock;
            InvertClock2.IsChecked = Properties.Settings.Default.invertClock;
            TwelveHr.IsChecked = Properties.Settings.Default.twelveHr;
            TwelveHr2.IsChecked = Properties.Settings.Default.twelveHr;

            LoadButtons();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void ResizeWindow()
        {
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            this.Top = SystemParameters.WorkArea.Top;
            this.Left = SystemParameters.WorkArea.Left;
        }

        private void UpdateClock()
        {
            lblClock.Content = DateTime.Now.ToString((Properties.Settings.Default.twelveHr) ? @"hh\:mm\:ss tt" : @"HH\:mm\:ss", CultureInfo.CurrentCulture);
            lblDate.Content = DateTime.Now.Date.ToString("dddd, MMMM d");
        }

        private void UpdateBackground(string filename)
        {
            Properties.Settings.Default.backgroundImage = System.IO.Path.GetFileName(filename);
            File.Copy(filename, Directory.GetCurrentDirectory() + "\\" + Properties.Settings.Default.backgroundImage, true);
            meBackground.Source = new Uri(Directory.GetCurrentDirectory() + "\\" + Properties.Settings.Default.backgroundImage, UriKind.Absolute);
        }

        private void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Video Files (*.mp4;*.mov;*.webm)|*.mp4;*.mov;*.webm|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == true) //ShowDialog is type bool? so yes you do have to do this (Shush)
            {
                UpdateBackground(ofd.FileName);
            }
        }

        private void meBackground_MediaEnded(object sender, RoutedEventArgs e)
        {
            meBackground.Stop();
            meBackground.Position = new TimeSpan(0);
            meBackground.Play();
        }

        private void meBackground_Loaded(object sender, RoutedEventArgs e)
        {
            meBackground.Play();
        }

        private void InvertClock_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.invertClock = true;
            lblClock.Foreground = Brushes.White;
            lblDate.Foreground = Brushes.White;
        }

        private void InvertClock_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.invertClock = false;
            lblClock.Foreground = Brushes.Black;
            lblDate.Foreground = Brushes.Black;
        }

        private void TwelveHr_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.twelveHr = true;
            TwelveHr.IsChecked = true;
            TwelveHr2.IsChecked = true;
            UpdateClock();
        }

        private void TwelveHr_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.twelveHr = false;
            TwelveHr.IsChecked = false;
            TwelveHr2.IsChecked = false;
            UpdateClock();
        }

        private void ShowHide_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Minimized;
                        break;
                    }
                case WindowState.Minimized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        ShowHide.Header = "Hide";
                        ShowHide2.Header = "Hide";
                        break;
                    }
                case WindowState.Minimized:
                    {
                        ShowHide.Header = "Show";
                        ShowHide2.Header = "Show";
                        break;
                    }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddCustomButton(string path)
        {
            string icon = "";

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "%USERPROFILE%\\Pictures";
            ofd.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.ico)|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.ico|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = false;
            ofd.Title = "Choose a new icon";

            if (ofd.ShowDialog() == true)
            {
                    icon = ofd.FileName;
            }

            string sanitizedPath = path;

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                sanitizedPath = sanitizedPath.Replace(c.ToString(), string.Empty);
            }

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\icons"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\icons");
            }
            
            File.Copy(icon, $"{Directory.GetCurrentDirectory()}\\icons\\{sanitizedPath}{System.IO.Path.GetExtension(icon)}", true);

            Button button = new Button();
            button.Content = "";
            button.Click += delegate (object sender, RoutedEventArgs e)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                });
            };
            button.Width = buttonSize;
            button.Height = buttonSize;
            button.Content = new Image()
            {
                Source = new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\icons\\{sanitizedPath}{System.IO.Path.GetExtension(icon)}", UriKind.Absolute)),
                Stretch = Stretch.UniformToFill,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            button.Margin = new Thickness(10);
            button.BorderThickness = new Thickness(0);
            button.Background = Brushes.Transparent;

            wrpButtons.Children.Add(button);

            SaveButton(path, $"{Directory.GetCurrentDirectory()}\\icons\\{sanitizedPath}{System.IO.Path.GetExtension(icon)}");
        }

        private void AddCustomButton(string path, string icon)
        {
            Button button = new Button();
            button.Content = "";
            button.Click += delegate (object sender, RoutedEventArgs e)
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                });
            };
            button.Width = buttonSize;
            button.Height = buttonSize;
            button.Content = new Image() {
                Source = new BitmapImage(new Uri(icon, UriKind.Absolute)),
                Stretch = Stretch.UniformToFill,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            button.Margin = new Thickness(10);
            button.BorderThickness = new Thickness(0);
            button.Background = Brushes.Transparent;

            wrpButtons.Children.Add(button);
        }

        private void SaveButton(string path, string icon)
        {
            bool append = File.Exists(Directory.GetCurrentDirectory() + "\\buttons.cfg");

            using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\buttons.cfg", append))
            {
                sw.WriteLine(path + "|" + icon);
            }
        }

        private void LoadButtons()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\buttons.cfg"))
            {
                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "\\buttons.cfg"))
                {
                    while (!sr.EndOfStream)
                    {
                        string? line = sr.ReadLine();
                        if (line != null)
                        {
                            AddCustomButton(line.Split("|")[0], line.Split("|")[1]);
                        }
                    }
                }
            }
        }

        private void AddApplication(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Choose a file or application";
            ofd.Filter = "All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.InitialDirectory = "c:\\";

            if (ofd.ShowDialog() == true)
            {
                AddCustomButton(ofd.FileName);
            }
        }

        private void AddSteamGame(object sender, RoutedEventArgs e)
        {
            InputDialog input = new InputDialog("Enter a Steam AppID:");

            if (input.ShowDialog() == true)
            {
                AddCustomButton("steam://rungameid/" + input.Answer);
            }
        }

        private void AddWebsite(object sender, RoutedEventArgs e)
        {
            InputDialog input = new InputDialog("Enter a URL:");

            if (input.ShowDialog() == true)
            {
                AddCustomButton((input.Answer.Contains("http://") || input.Answer.Contains("https://")) ? input.Answer : "http://" + input.Answer);
            }
        }
    }
}
