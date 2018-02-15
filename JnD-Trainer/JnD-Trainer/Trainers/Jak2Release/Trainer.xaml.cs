using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using Binarysharp.MemoryManagement;
using System.Windows.Threading;

namespace JnD_Trainer.Trainers.Jak2Release
{
    /// <summary>
    /// Interaction logic for Trainer.xaml
    /// </summary>
    public partial class Trainer : Window {
        static DispatcherTimer retryEmuConnection = new DispatcherTimer();
        static DispatcherTimer updateTrainer = new DispatcherTimer();
        private static Process emuProcess = null;
        private static MemorySharp memEdit = null;

        private bool connected;
        public Trainer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            retryEmuConnection.Tick += new EventHandler(ConnectToPcsx2);
            retryEmuConnection.Interval = new TimeSpan(0, 0, 2);
            retryEmuConnection.Start();

            updateTrainer.Tick += new EventHandler(trainerFrame);
            updateTrainer.Interval = TimeSpan.FromMilliseconds(50); // every 50 milliseconds 
            updateTrainer.Start();


            //
            //            IntPtr address = new IntPtr(0x20000000);
            //
            //            var bytes = sharp.Read<byte>(address, 16, isRelative: false);
            //
            //            for (int i = 0; i < bytes.Length; i++)
            //            {
            //                bytes[i]++;
            //            }
            //
            //            sharp.Write<byte>(address, bytes, isRelative: false);

            Console.WriteLine("Ye");
        }


        /// <summary>
        /// Ensures a connection to PCSX2, will repeatedly attempt using a background thread until connected
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void ConnectToPcsx2(Object o, EventArgs args) {
            // TODO will need to support when pcsx2 is closed after detect and re-connect
            try
            {
                emuProcess = Process.GetProcessesByName("pcsx2").First();
                memEdit = new MemorySharp(emuProcess);
                EmulatorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 150, 0));
                EmulatorStatus.Content = "Connected";
                retryEmuConnection.Stop();
            }
            catch (System.InvalidOperationException exception)
            {
                Console.WriteLine("PCSX2 Still not opened"); // TODO no console output pls
                EmulatorStatus.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                EmulatorStatus.Content = "Connecting...";
            }
        }

        private void trainerFrame(Object o, EventArgs args) {

            if (memEdit == null || emuProcess == null || emuProcess.HasExited) {
                return;
            }

            float val = memEdit.Read<float>(new IntPtr(0x20622F58), isRelative: false);
            ScatterGunAmmo.Text = val.ToString();
        }


        private void returnToSplash_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exportValues_Click(object sender, RoutedEventArgs e)
        {

        }

        private void importValues_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
