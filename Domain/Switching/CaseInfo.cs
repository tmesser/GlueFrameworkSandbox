using System;

namespace Domain.Switching
{
    public class CaseInfo
    {
        public bool IsDefault { get; set; }
        public Type Target { get; set; }
        public Action<object> Action { get; set; }
    }
}
