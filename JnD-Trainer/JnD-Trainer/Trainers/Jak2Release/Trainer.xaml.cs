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
using JnD_Trainer.src;

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

        // TODO PAL conversion
        protected static List<Address> EditableAddresses;


        private void Window_Loaded(object sender, RoutedEventArgs e) {

            retryEmuConnection.Tick += new EventHandler(ConnectToPcsx2);
            retryEmuConnection.Interval = new TimeSpan(0, 0, 2);
            retryEmuConnection.Start();

            updateTrainer.Tick += new EventHandler(trainerFrame);
            updateTrainer.Interval = TimeSpan.FromMilliseconds(50); // every 50 milliseconds 
            updateTrainer.Start();
            

            // -----
            // Init all Addresses with their respective component
            EditableAddresses = new List<Address> {
                // Position values are multipled by 4096 or 2^64
                // And should be displayed to 4 digits precision
                new Address<float>(0x20197790, JakPosX, transFunc: (val) => (val / 4096).ToString("0.0000")),
                new Address<float>(0x20197794, JakPosY, transFunc: (val) => (val / 4096).ToString("0.0000")),
                new Address<float>(0x20197798, JakPosZ, transFunc: (val) => (val / 4096).ToString("0.0000")),

                new Address<float>(0x2019C5C0, JakHP),
                new Address<float>(0x20622F28, JakEco),
                // RAWB: TODO disabled tempoarily because combo box implemention is annoying atm
                // new Address<int>  (0x20622F50, SelectedGun),

                new Address<float>(0x20622F58, ScatterAmmo),
                new Address<float>(0x20622F54, BlasterAmmo),
                new Address<float>(0x20622F5C, VulcanAmmo),
                new Address<float>(0x20622F60, PeacemakerAmmo),

                new Address<float>(0x20622F1C, OrbCount),
                new Address<float>(0x20622F14, SkullgemCount)
            };


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

            // If we havn't connected to PCSX2 yet, then don't do anything
            // TODO if disconnected, then restart the connect to pcsx2 timer
            if (memEdit == null || emuProcess == null || emuProcess.HasExited) {
                return;
            }

            // Loop through all addresses and update their values
            foreach (Address addr in EditableAddresses) {
                addr.UpdateUIElement(memEdit);
            }

            // RAWB: TODO update lateral and vertical speed
        }


        private void returnToSplash_Click(object sender, RoutedEventArgs e)
        {

        }
        
    }
}
