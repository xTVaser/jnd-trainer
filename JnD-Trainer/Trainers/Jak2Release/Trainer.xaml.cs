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

        public Trainer() {
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
                // ----
                // General Information
                // ----
                // Position values are multipled by 4096 or 2^64
                // And should be displayed to 4 digits precision
                new Address<float>(0x2019_7790, JakPosX, transFunc: (val) => (val / 4096).ToString("0.0000")),
                new Address<float>(0x2019_7794, JakPosY, transFunc: (val) => (val / 4096).ToString("0.0000")),
                new Address<float>(0x2019_7798, JakPosZ, transFunc: (val) => (val / 4096).ToString("0.0000")),

                new Address<float>(0x2019_C5C0, JakHP),
                new Address<float>(0x2062_2F28, JakEco),
                // RAWB: TODO disabled tempoarily because combo box implemention is annoying atm
                // new Address<int>  (0x20622F50, SelectedGun),

                new Address<float>(0x2062_2F58, ScatterAmmo),
                new Address<float>(0x2062_2F54, BlasterAmmo),
                new Address<float>(0x2062_2F5C, VulcanAmmo),
                new Address<float>(0x2062_2F60, PeacemakerAmmo),

                new Address<float>(0x2062_2F1C, OrbCount),
                new Address<float>(0x2062_2F14, SkullgemCount),

                // ----
                // Inventory
                // ----
                new Address<byte>(0x2062_2F30, InventoryGuns,    truthyValue: 1, bitMask: 0b0010_0000),
                new Address<byte>(0x2062_2F30, InventoryBlaster, truthyValue: 1, bitMask: 0b0100_0000),
                new Address<byte>(0x2062_2F30, InventoryScatter, truthyValue: 1, bitMask: 0b1000_0000),

                new Address<byte>(0x2062_2F31, InventoryVulcan, truthyValue: 1, bitMask: 0b0000_0001),
                new Address<byte>(0x2062_2F31, InventoryPeacemaker, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2062_2F31, InventoryJetboard, truthyValue: 1, bitMask: 0b0000_0100),
                new Address<byte>(0x2062_2F31, InventoryDaxter, truthyValue: 1, bitMask: 0b0001_0000),
                new Address<byte>(0x2062_2F31, InventoryDarkJak, truthyValue: 1, bitMask: 0b0010_0000),
                new Address<byte>(0x2062_2F31, InventoryScatterUpgrade, truthyValue: 1, bitMask: 0b0100_0000),
                new Address<byte>(0x2062_2F31, InventoryAmmoUpgrade, truthyValue: 1, bitMask: 0b1000_0000),

                new Address<byte>(0x2062_2F32, InventoryDamageUpgrade, truthyValue: 1, bitMask: 0b0000_0001),
                new Address<byte>(0x2062_2F32, InventoryRedBarrier, truthyValue: 1, bitMask: 0b0000_0100),
                new Address<byte>(0x2062_2F32, InventoryGreenBarrier, truthyValue: 1, bitMask: 0b0000_1000),
                new Address<byte>(0x2062_2F32, InventoryYellowBarrier, truthyValue: 1, bitMask: 0b0001_0000),
                new Address<byte>(0x2062_2F32, InventoryDarkBomb, truthyValue: 1, bitMask: 0b0100_0000),
                new Address<byte>(0x2062_2F32, InventoryDarkBlast, truthyValue: 1, bitMask: 0b1000_0000),

                new Address<byte>(0x2062_2F33, InventoryDarkInvul, truthyValue: 1, bitMask: 0b0000_0001),
                new Address<byte>(0x2062_2F33, InventoryDarkGiant, truthyValue: 1, bitMask: 0b0000_0010),

                // ----
                // Mission Information
                // ----
                new Address<float>(0x2062_2F8C, CurrentObjectiveCounter),
                new Address<int>(0x2062_2F78, CurrentObjectiveTimer),
                new Address<float>(0x2062_2F68, CurrentObjectiveScore),
                // TODO guard status, another combo box
                // TODO find praxis 1 addresses
                new Address<int>(0x201B_5858, Praxis2BombHits),
                // TODO find praxis 2 position vectors
                new Address<int>(0x201B_442C, KrewHP),
                new Address<int>(0x201B_4620, KrewCloneCounter),
                
                new Address<float>(0x201B_47E0, KrewPositionX), // TODO maybe implement ranges to hide values to disable box when not on the boss
                new Address<float>(0x201B_47E4, KrewPositionY),
                new Address<float>(0x201B_47E8, KrewPositionZ),

                // TODO kor addresses, find

                // ----
                // Camera Controls
                // ----
                // TODO find camera addresses
                new Address<float>(0x2082_7600, FOVControl),

                // ----
                // Secrets and Misc
                // ----
                new Address<byte>(0x2062_2F40, SecretToggleGoatee, truthyValue: 1, bitMask: 0b0000_0001),
                new Address<byte>(0x2062_2F40, SecretMirrorWorld, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2062_2F40, SecretUnlimitedAmmo, truthyValue: 1, bitMask: 0b0000_0100),
                new Address<byte>(0x2062_2F40, SecretInvulnerability, truthyValue: 1, bitMask: 0b0000_1000),
                new Address<byte>(0x2062_2F40, SecretUnlimitedDarkJak, truthyValue: 1, bitMask: 0b0001_0000),
                new Address<byte>(0x2062_2F42, SecretBigHeadMode, truthyValue: 1, bitMask: 0b0000_0001),
                new Address<byte>(0x2062_2F42, SecretSmallHeadMode, truthyValue: 1, bitMask: 0b0000_0010),

                new Address<int>(0x2062_2F94, StatsNumberOfAttacks),
                new Address<float>(0x2062_2F2C, StatsNumberOfEcoCollected),

                // ----
                // Debug Functions
                // ----
                new Address<byte>(0x2014_9620, DebugBugReport, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2035_DB90, DebugProfiler, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2062_30C8, DebugMinimapFlashing, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_5B04, DebugLeafIndex, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_5F70, DebugLevelBuffer, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_66D8, DebugLevelInfo, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_6878, Debug2D3DInfo, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_8C14, DebugRenderingEffects, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_9618, DebugCwCcwStrdl, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_AA18, DebugCameraPosition, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_AA00, DebugAnimationChannels, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2014_AAC8, DebugEmptyChannels, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2035_E564, DebugStopRenderWalls, truthyValue: 1, bitMask: 0b0000_0010),
                new Address<byte>(0x2082_6E44, DebugInvertCamera, truthyValue: 1, bitMask: 0b0000_0010)
            };

            // TODO animation controls are all functions, need to find a new way to do those
            // TODO freeze position functions in mission information
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


        private void returnToSplash_Click(object sender, RoutedEventArgs e) {

        }
    }
}
