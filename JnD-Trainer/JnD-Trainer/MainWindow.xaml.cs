using Binarysharp.MemoryManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using JnD_Trainer.Trainers;

namespace JnD_Trainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
//            Console.WriteLine("test");
//
//            var pcsx2 = Process.GetProcessesByName("pcsx2").First();
//            var sharp = new MemorySharp(pcsx2);
//
//            IntPtr address = new IntPtr(hexToInt("2000 0000"));
//
//            var bytes = sharp.Read<byte>(address, 16, isRelative: false);
//
//            for(int i = 0; i < bytes.Length; i++)
//            {
//                bytes[i]++;
//            }
//
//            sharp.Write<byte>(address, bytes, isRelative: false);
//
//            Console.WriteLine("Ye");
            
        }

        /// move me
        private int hexToInt(string hex)
        {
            // Remove all whitespace
            hex = Regex.Replace(hex, @"\s+", "");
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        private void Jak1_Release_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void Jak1_Demo_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void Jak2_Release_Click(object sender, RoutedEventArgs e)
        {
            // TODO implement
            var newTrainer = new Trainers.Jak2Release.Trainer();
            newTrainer.Show();
            this.Close();
        }

        private void Jak2_Demo_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void Jak3_Release_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void Jak3_Demo_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void JakX_Release_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void JakX_Demo_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void JakTLF_Release_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }

        private void JakTLF_Release1_Click(object sender, RoutedEventArgs e)
        {
            // Stub
        }
    }
}
