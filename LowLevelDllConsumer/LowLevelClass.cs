using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LowLevelDllConsumer
{
    /// <summary>
    /// Definuje rozhraní, které potřebuje každá C# třída, která je napojena na C++ třídu
    /// SafeHandle udělá to, že při GC automaticky zavolá Dispose, které volá destruktor. Proto GC může uklízet i C++ objekty.
    /// </summary>
    public abstract class LowLevelClass : SafeHandle, IDisposable
    {
        public LowLevelClass() : base(IntPtr.Zero, true)
        { }

        public override bool IsInvalid => Ptr == IntPtr.Zero;

        /// <summary>
        /// Ukazatel na C++ objekt, který se vytvořil v DLL
        /// </summary>
        protected IntPtr Ptr { get; set; }

        protected override bool ReleaseHandle()
        {
            Dispose();
            return true;
        }
    }
}