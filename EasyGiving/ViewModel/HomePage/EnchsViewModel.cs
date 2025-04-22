using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;

namespace EasyGiving.ViewModel.HomePage
{
    public class EnchsViewModel
    {
        public ObservableCollection<Ench> AddedEnchs { get; private set; } = new ObservableCollection<Ench>();
        public EnchsViewModel() { }

        public Guid Add(string EnchID, string EnchDisplayName, string EnchLvl)
        {
            if (GetEnchByAnyEnchElements(EnchID, EnchElements.ID) != Ench.Empty) throw new EnchAlreadyExistsException($"ID为{EnchID}的魔咒已添加");
            var _enchGuid = Guid.NewGuid();
            var _ench = new Ench(_enchGuid, EnchID, EnchDisplayName, EnchLvl);
            this.AddedEnchs.Add(_ench);
            return _enchGuid;
        }

        public bool DelOneEnch(Guid enchGuid)
        {
            foreach (var ench in this.AddedEnchs) if (ench.EnchGuid == enchGuid) return this.AddedEnchs.Remove(ench);
            return false;
        }

        public Ench GetEnchByAnyEnchElements(string Element, EnchElements ElementType)
        {
            if (this.AddedEnchs == null) throw new EnchListIsNullException("列表AddedEnchs为空，数据类型为ObservableCollection<Ench>");
            if (ElementType == EnchElements.Guid) throw new EnchElementsTypeError("参数2输入为EnchElements.Guid时参数1不应为string，应为Guid");
            else if (ElementType == EnchElements.ID) foreach (var e in this.AddedEnchs) if (e.ID == Element) return e;
            else foreach (var f in this.AddedEnchs) if (f.DisplayName == Element) return f;
            return Ench.Empty;
        }

        public Ench GetEnchByAnyEnchElements(Guid Element)
        {
            if (this.AddedEnchs == null) throw new EnchListIsNullException("列表AddedEnchs为空，数据类型为ObservableCollection<Ench>");
            foreach (var e in this.AddedEnchs) if (e.EnchGuid == Element) return e;
            return Ench.Empty;
        }

        public (Guid, int) GetEnchGuidAndIndexByID(string ID)
        {
            foreach (var e in this.AddedEnchs) if (e.ID == ID) return (e.EnchGuid, this.AddedEnchs.IndexOf(e));
            return (Guid.Empty, -1);
        }

        public void Clear() => this.AddedEnchs.Clear();

        public void All255()
        {
            foreach (var e in this.AddedEnchs) e.EnchLvl = 255;
        }

        public enum EnchElements
        {
            Guid = 0,
            ID = 1,
            DisplayName = 2
        }

        public class EnchNotFoundException : Exception
        {
            public EnchNotFoundException() { } 
            public EnchNotFoundException(string message) : base(message) { }
            public EnchNotFoundException(string message, Exception inner) : base(message, inner) { }
        }

        public class EnchListIsNullException : Exception
        {
            public EnchListIsNullException() { }
            public EnchListIsNullException(string message) : base(message) { }
            public EnchListIsNullException(string message, Exception inner) : base(message, inner) { }
        }

        public class EnchElementsTypeError : Exception
        {
            public EnchElementsTypeError() { }
            public EnchElementsTypeError(string message) : base(message) { }
            public EnchElementsTypeError(string message, Exception inner) : base(message, inner) { }
        }

        public class EnchAlreadyExistsException : Exception
        {
            public EnchAlreadyExistsException() { }
            public EnchAlreadyExistsException(string message) : base(message) { }
            public EnchAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
        }
    }

    public class Ench
    {
        public readonly static Ench Empty = new Ench
        {
            EnchGuid = Guid.Empty,
            ID = string.Empty,
            DisplayName = string.Empty,
            EnchLvl = 0
        };

        public Guid EnchGuid { get; init; }
        public string ID { get; init; }
        public string DisplayName { get; init; }
        public byte EnchLvl { get; set; }
        public DecimalFormatter LvlFormatter
        {
            get
            {
                return _LvlFormatter;
            }
        }

        private readonly static DecimalFormatter _LvlFormatter = new DecimalFormatter()
        {
            IntegerDigits = 1,
            FractionDigits = 0,
        };

        public Ench() { }
        public Ench(Guid EnchGuid, string ID, string DisplayName, string EnchLvl)
        {
            this.EnchGuid = EnchGuid;
            this.ID = ID;
            this.DisplayName = DisplayName;
            this.EnchLvl = Convert.ToByte(EnchLvl);
        }
    }
}
