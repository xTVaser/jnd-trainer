using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Binarysharp.MemoryManagement;

namespace JnD_Trainer
{
    public abstract class Address {

        public abstract void UpdateUIElement(MemorySharp memEdit);

    }

    // T - Type for Memory Address that we read/write to
    public class Address<T> : Address
    {
        public int HexAddress { get; }
        public Control UiElement { get; }
        public bool ShouldEdit { get; }
        public bool ShouldFreeze { get; }
        private Func<T, string> TransFunc; // FUNCTIONAL! (ithink)

        public Address(int hexAddress, Control uiElement, Func<T, string> transFunc = null, bool shouldEdit = true, bool shouldFreeze = false) {
            this.HexAddress = hexAddress;
            this.UiElement = uiElement;
            this.TransFunc = transFunc;
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
            string finalValue = val.ToString();
            if (TransFunc != null) {
                finalValue = TransFunc(val);
            }
            if (UiElement.GetType() == typeof(TextBox)) {
                ((TextBox) UiElement).Text = finalValue;
            }
            else {
                Console.Write("Handle a New One Bud");
            }
        }

        public Type Type => typeof(T);
    }
}

/*
 * if something breaks
 *     public class Setting {
     private string _name;
     public string Name {
      get {
       if (string.IsNullOrEmpty(_name)) {

        throw new ApplicationException("Property [Name] in class [Setting] is null or empty");
       }
       return _name;
      }
      set {
       _name = value;
      }
     }
    }
    [Serializable]
    public class Setting < T > : Setting {
     private T _value;
     public T Value {
      get {
       return _value;
      }
      set {
       _value = value;
      }
     }
     public Type Type {
      get {
       return typeof(T);
      }
     }
     /// <summary>
     /// Define a default constructor for serialization purposes
     /// </summary>
     public Setting() {}
    }

    */