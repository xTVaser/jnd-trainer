using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Binarysharp.MemoryManagement;

namespace JnD_Trainer {
    public abstract class Address {
        public abstract void UpdateUIElement(MemorySharp memEdit);
    }

    // T - Type for Memory Address that we read/write to
    public class Address<T> : Address {
        public int HexAddress { get; }
        public Control UiElement { get; }
        public T OnValue { get; }
        public T OffValue { get; } // TODO rename to on and off values
        public byte BitMask { get; }
        public bool ShouldEdit { get; }
        public bool ShouldFreeze { get; }
        private readonly Func<T, string> _transFunc; // FUNCTIONAL! (ithink)

        // State
        // TODO: consider storing MemorySharp instance in address objects
        // TODO: might be able to combine into a single flag, since we check the type anyway, leave it alone for now
        private bool ToggleCheckbox = false;
        private bool WriteToTextbox = false;
        private string NewValue;
        private bool SkipWrite = false;

        public Address(int hexAddress, Control uiElement, T onValue = default(T), T offValue = default(T), byte bitMask = 0b0,
            Func<T, string> transFunc = null, bool shouldEdit = true, bool shouldFreeze = false) {
            this.HexAddress = hexAddress;
            this.UiElement = uiElement;
            this.OnValue = onValue;
            this.OffValue = offValue;
            this.BitMask = bitMask;
            this._transFunc = transFunc;
            this.ShouldEdit = shouldEdit;
            this.ShouldFreeze = shouldFreeze;

            // Setup event listeners on UI control element
            if (UiElement.GetType() == typeof(CheckBox)) {
                ((CheckBox)UiElement).Checked += (sender, args) => {
                    if (!SkipWrite) {
                        ToggleCheckbox = true;
                    }
                };
                ((CheckBox)UiElement).Unchecked += (sender, args) => {
                    if (!SkipWrite) {
                        ToggleCheckbox = true;
                    }
                };
            }
            else if (UiElement.GetType() == typeof(TextBox)) {
                ((TextBox) UiElement).TextChanged += (sender, args) => {
                    if (!SkipWrite) {
                        WriteToTextbox = true;
                        NewValue = ((TextBox)UiElement).Text;
                        if (NewValue == "") {
                            NewValue = "0";
                        }
                    }
                };
            }
        }

        override
        public void UpdateUIElement(MemorySharp memEdit) {
            if (memEdit == null) {
                return;
            }

            // Write to the address first if applicable
            // TODO separate this into a read and write function
            if (UiElement.GetType() == typeof(CheckBox)) {
                if (ToggleCheckbox) {
                    // If we are using bit masks, we need to prepare the write to only flip that one bit
                    if (Type == typeof(byte) && BitMask != 0b0) {
                        var currentValue = memEdit.Read<T>(new IntPtr(HexAddress), isRelative: false);
                        byte toBeWritten = (byte) (Convert.ToByte(currentValue) ^ BitMask);

                        memEdit.Write<byte>(new IntPtr(HexAddress), toBeWritten, isRelative: false);
                    }
                    else {
                        memEdit.Write<T>(new IntPtr(HexAddress), OnValue, isRelative: false);
                    }
                    ToggleCheckbox = false;
                }
            }
            else if (UiElement.GetType() == typeof(TextBox)) {
                if (WriteToTextbox) {
                    // Writing requires we explicitly know the Type
                    if (Type == typeof(int) && int.TryParse(NewValue, out var newValueInt)) {
                        memEdit.Write<int>(new IntPtr(HexAddress), newValueInt, isRelative: false);
                    }
                    else if (Type == typeof(float) && float.TryParse(NewValue, out var newValueFloat)) {
                        memEdit.Write<float>(new IntPtr(HexAddress), newValueFloat, isRelative: false);
                    }
                    WriteToTextbox = false;
                }
            }

            // TODO wrap memory calls in try catch or BOOM (sometimes)
            var val = memEdit.Read<T>(new IntPtr(HexAddress), isRelative: false);
            if (UiElement.GetType() == typeof(TextBox) && !((TextBox)UiElement).IsFocused) {
                string finalValue = val.ToString();
                if (_transFunc != null)
                {
                    finalValue = _transFunc(val);
                }

                SkipWrite = true;
                ((TextBox) UiElement).Text = finalValue;
                SkipWrite = false;
            }
            else if (UiElement.GetType() == typeof(CheckBox)) {
                // We need to look at a specific bit in the byte response
                if (Type == typeof(byte) && BitMask != 0b0) {
                    var mask = (Convert.ToInt32(val) & BitMask);
                    SkipWrite = true;
                    ((CheckBox)UiElement).IsChecked = mask == BitMask;
                    SkipWrite = false;
                }
                // TODO else, just compare with the truthy value
                
            }
            else {
                Console.Write("Handle a New One Bud");
            }
        }

        public Type Type => typeof(T);
    }
}
