using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Memory
{
    class MemoryHack
    {
        private enum ProcessAccessFlags : uint
        {
            All = 0x1f0fff,
            CreateThread = 2,
            DupHandle = 0x40,
            QueryInformation = 0x400,
            SetInformation = 0x200,
            Synchronize = 0x100000,
            Terminate = 1,
            VMOperation = 8,
            VMRead = 0x10,
            VMWrite = 0x20

        }
        private enum VirtualMemoryProtection : uint
        {
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOACCESS = 1,
            PAGE_NOCACHE = 0x200,
            PAGE_READONLY = 2,
            PAGE_READWRITE = 4,
            PAGE_WRITECOPY = 8,
            PROCESS_ALL_ACCESS = 0x1f0fff

        }
        public static bool debugMode = false;
        private IntPtr baseAddress;
        private ProcessModule processModule;
        private Process[] mainProcess;
        private IntPtr processHandle;
        public string processName
        {
            get;
            set;
        }
        public long getBaseAddress
        {
            get
            {
                this.baseAddress = (IntPtr)0;
                this.processModule = this.mainProcess[0].MainModule;
                this.baseAddress = this.processModule.BaseAddress;
                return (long)this.baseAddress;
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, uint lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, uint lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        public MemoryHack()
        {
        }
        public MemoryHack(string pProcessName)
        {
            this.processName = pProcessName;
        }
        public bool CheckProcess()
        {
            bool result;
            if (this.processName != null)
            {
                this.mainProcess = Process.GetProcessesByName(this.processName);
                if (this.mainProcess.Length == 0)
                {
                    this.ErrorProcessNotFound(this.processName);
                    result = false;
                }
                else
                {
                    this.processHandle = MemoryHack.OpenProcess(2035711u, false, this.mainProcess[0].Id);
                    if (this.processHandle == IntPtr.Zero)
                    {
                        this.ErrorProcessNotFound(this.processName);
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            else
            {

                result = false;
            }
            return result;
        }

        private void ErrorProcessNotFound(string p)
        {
            throw new NotImplementedException();
        }
        public byte[] ReadByteArray(IntPtr pOffset, uint pSize)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            byte[] result;
            try
            {
                uint flNewProtect;
                MemoryHack.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pSize, 4u, out flNewProtect);
                byte[] array = new byte[pSize];
                MemoryHack.ReadProcessMemory(this.processHandle, pOffset, array, pSize, 0u);
                MemoryHack.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)pSize, flNewProtect, out flNewProtect);
                result = array;
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadByteArray" + ex.ToString());
                }
                result = new byte[1];
            }
            return result;
        }
        public string ReadStringUnicode(IntPtr pOffset, uint pSize)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            string result;
            try
            {
                result = Encoding.Unicode.GetString(this.ReadByteArray(pOffset,

                 pSize), 0, (int)pSize);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadStringUnicode" + ex.ToString());
                }
                result = "";
            }
            return result;
        }
        public string ReadStringASCII(IntPtr pOffset, uint pSize)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            string result;
            try
            {
                result = Encoding.ASCII.GetString(this.ReadByteArray(pOffset,

                 pSize), 0, (int)pSize);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadStringASCII" + ex.ToString());
                }
                result = "";
            }
            return result;
        }
        public char ReadChar(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            char result;
            try
            {
                result = BitConverter.ToChar(this.ReadByteArray(pOffset, 1u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadChar" + ex.ToString());
                }
                result = ' ';
            }
            return result;
        }
        public bool ReadBoolean(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = BitConverter.ToBoolean(this.ReadByteArray(pOffset, 1u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadByte" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public byte ReadByte(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            byte result;
            try
            {
                result = this.ReadByteArray(pOffset, 1u)[0];
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadByte" + ex.ToString());
                }
                result = 0;
            }
            return result;
        }
        public short ReadInt16(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            short result;
            try
            {
                result = BitConverter.ToInt16(this.ReadByteArray(pOffset, 2u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadInt16" + ex.ToString());
                }
                result = 0;
            }
            return result;
        }
        public short ReadShort(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            short result;
            try
            {
                result = BitConverter.ToInt16(this.ReadByteArray(pOffset, 2u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadInt16" + ex.ToString());
                }
                result = 0;
            }
            return result;
        }
        public int ReadInt32(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            int result;
            try
            {
                result = BitConverter.ToInt32(this.ReadByteArray(pOffset, 4u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadInt32" + ex.ToString());
                }
                result = 0;
            }
            return result;
        }
        public int ReadInteger(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            int result;
            try
            {
                result = BitConverter.ToInt32(this.ReadByteArray(pOffset, 4u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadInteger" + ex.ToString());
                }
                result = 0;
            }
            return result;
        }
        public long ReadInt64(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            long result;
            try
            {
                result = BitConverter.ToInt64(this.ReadByteArray(pOffset, 8u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadInt64" + ex.ToString());
                }
                result = 0L;
            }
            return result;
        }
        public long ReadLong(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            long result;
            try
            {
                result = BitConverter.ToInt64(this.ReadByteArray(pOffset, 8u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadLong" + ex.ToString());
                }
                result = 0L;
            }
            return result;
        }
        public ushort ReadUInt16(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            ushort result;
            try
            {
                result = BitConverter.ToUInt16(this.ReadByteArray(pOffset, 2u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadUInt16" + ex.ToString());
                }
                result = 0;
            }
            return result;
        }
        public ushort ReadUShort(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            ushort result;
            try
            {
                result = BitConverter.ToUInt16(this.ReadByteArray(pOffset, 2u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadUShort" + ex.ToString());
                }
                result = 0;
            }
            return result;
        }
        public uint ReadUInt32(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            uint result;
            try
            {
                result = BitConverter.ToUInt32(this.ReadByteArray(pOffset, 4u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadUInt32" + ex.ToString());
                }
                result = 0u;
            }
            return result;
        }
        public uint ReadUInteger(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            uint result;
            try
            {
                result = BitConverter.ToUInt32(this.ReadByteArray(pOffset, 4u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadUInteger" + ex.ToString());
                }
                result = 0u;
            }
            return result;
        }
        public ulong ReadUInt64(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            ulong result;
            try
            {
                result = BitConverter.ToUInt64(this.ReadByteArray(pOffset, 8u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadUInt64" + ex.ToString());
                }
                result = 0uL;
            }
            return result;
        }
        public long ReadULong(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            long result;
            try
            {
                result = (long)BitConverter.ToUInt64(this.ReadByteArray(pOffset,

                 8u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadULong" + ex.ToString());
                }
                result = 0L;
            }
            return result;
        }
        public float ReadFloat(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            float result;
            try
            {
                result = BitConverter.ToSingle(this.ReadByteArray(pOffset, 4u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadFloat" + ex.ToString());
                }
                result = 0f;
            }
            return result;
        }
        public double ReadDouble(IntPtr pOffset)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            double result;
            try
            {
                result = BitConverter.ToDouble(this.ReadByteArray(pOffset, 8u), 0);
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: ReadDouble" + ex.ToString());
                }
                result = 0.0;
            }
            return result;
        }
        public bool WriteByteArray(IntPtr pOffset, byte[] pBytes)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                uint flNewProtect;
                MemoryHack.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), 4u, out flNewProtect);
                bool flag = MemoryHack.WriteProcessMemory(this.processHandle, pOffset, pBytes, (uint)pBytes.Length, 0u);
                MemoryHack.VirtualProtectEx(this.processHandle, pOffset, (UIntPtr)((ulong)((long)pBytes.Length)), flNewProtect, out flNewProtect);
                result = flag;
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteByteArray" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteStringUnicode(IntPtr pOffset, string pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, Encoding.Unicode.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteStringUnicode" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteStringASCII(IntPtr pOffset, string pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, Encoding.ASCII.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteStringASCII" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteBoolean(IntPtr pOffset, bool pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteBoolean" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteChar(IntPtr pOffset, char pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteChar" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteByte(IntPtr pOffset, byte pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes((short)pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteByte" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteInt16(IntPtr pOffset, short pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteInt16" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteShort(IntPtr pOffset, short pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteShort" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteInt32(IntPtr pOffset, int pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteInt32" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteInteger(IntPtr pOffset, int pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteInt" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteInt64(IntPtr pOffset, long pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteInt64" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteLong(IntPtr pOffset, long pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteLong" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteUInt16(IntPtr pOffset, ushort pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteUInt16" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteUShort(IntPtr pOffset, ushort pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteShort" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteUInt32(IntPtr pOffset, uint pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteUInt32" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteUInteger(IntPtr pOffset, uint pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteUInt" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteUInt64(IntPtr pOffset, ulong pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteUInt64" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteULong(IntPtr pOffset, ulong pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteULong" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteFloat(IntPtr pOffset, float pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteFloat" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
        public bool WriteDouble(IntPtr pOffset, double pData)
        {
            if (this.processHandle == IntPtr.Zero)
            {
                this.CheckProcess();
            }
            bool result;
            try
            {
                result = this.WriteByteArray(pOffset, BitConverter.GetBytes(pData));
            }
            catch (Exception ex)
            {
                if (MemoryHack.debugMode)
                {
                    Console.WriteLine("Error: WriteDouble" + ex.ToString());
                }
                result = false;
            }
            return result;
        }
    }

}

