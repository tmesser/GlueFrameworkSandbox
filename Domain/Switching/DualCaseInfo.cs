using System;

namespace Domain.Switching
{
    public class DualCaseInfo
    {
        public bool IsDefault { get; set; }
        public Type FirstTarget { get; set; }
        public Type SecondTarget { get; set; }
        public Action<object, object> Action { get; set; }
    }
}
