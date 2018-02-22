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
        public T TruthyValue { get; }
        public T ToggleValue { get; }
        public byte BitMask { get; }
        public bool ShouldEdit { get; }
        public bool ShouldFreeze { get; }
        private readonly Func<T, string> _transFunc; // FUNCTIONAL! (ithink)

        public Address(int hexAddress, Control uiElement, T truthyValue = default(T), T toggleValue = default(T), byte bitMask = 0b0,
            Func<T, string> transFunc = null, bool shouldEdit = true, bool shouldFreeze = false) {
            this.HexAddress = hexAddress;
            this.UiElement = uiElement;
            this.TruthyValue = truthyValue;
            this.ToggleValue = toggleValue;
            this.BitMask = bitMask;
            this._transFunc = transFunc;
            this.ShouldEdit = shouldEdit;
            this.ShouldFreeze = shouldFreeze;
        }

        override
        public void UpdateUIElement(MemorySharp memEdit) {
            if (memEdit == null) {
                return;
            }

            // TODO wrap memory calls in try catch or BOOM (sometimes)
            var val = memEdit.Read<T>(new IntPtr(HexAddress), isRelative: false);
            if (UiElement.GetType() == typeof(TextBox)) {
                string finalValue = val.ToString();
                if (_transFunc != null)
                {
                    finalValue = _transFunc(val);
                }
                ((TextBox) UiElement).Text = finalValue;
            }
            else if (UiElement.GetType() == typeof(CheckBox)) {
                // We need to look at a specific bit in the byte response
                if (Type == typeof(byte) && BitMask != 0b0) {
                    var mask = (Convert.ToInt32(val) & BitMask);
                    ((CheckBox)UiElement).IsChecked = mask == BitMask;
                    //((CheckBox) UiElement).IsChecked = ((Convert.ToInt32(val) & BitMask) == 1);
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
