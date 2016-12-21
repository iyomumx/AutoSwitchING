using System;
using System.Linq;

namespace AutoSwitchING
{
    public enum SwitchType
    {
        Index,
        TabName
    }

    public static class SwitchTypeHelper
    {
        public class SwitchTypeName
        {
            public string Name { get; internal set; }
            public SwitchType Value { get; internal set; }
        }

        static SwitchTypeHelper()
        {
            Names = Enum.GetValues(typeof(SwitchType)).OfType<SwitchType>().Select(e => new SwitchTypeName() { Name = Enum.GetName(typeof(SwitchType), e), Value = e }).ToArray();
        }

        public static SwitchTypeName[] Names { get; private set; }
    }
}