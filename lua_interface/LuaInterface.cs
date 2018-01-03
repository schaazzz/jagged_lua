using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JaggedLua
{
    public class Lua
    {
        public const int LUA_MULTRET = (-1);
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int Callback (IntPtr lua_State);

        [DllImport("lua_core.dll")]
        private static extern IntPtr luaL_newstate();

        [DllImport("lua_core.dll")]
        private static extern void lua_pushcclosure(IntPtr luaState, [MarshalAs(UnmanagedType.FunctionPtr)] Callback func, int n);

        [DllImport("lua_core.dll")]
        private static extern void lua_setglobal(IntPtr luaState, string name);

        [DllImport("lua_core.dll")]
        private static extern void luaL_openlibs(IntPtr luaState);

        [DllImport("lua_core.dll")]
        private static extern void lua_close(IntPtr luaState);

        [DllImport("lua_core.dll")]
        private static extern int luaL_loadstring(IntPtr luaState, string s);

        [DllImport("lua_core.dll")]
        private static extern void lua_pushinteger(IntPtr luaState, UInt64 n);

        [DllImport("lua_core.dll")]
        private static extern int lua_getglobal(IntPtr luaState, string name);

        [DllImport("lua_core.dll")]
        private static extern int lua_tointegerx(IntPtr luaState, int ignore0, int ignore1);

        [DllImport("lua_core.dll")]
        private static extern int lua_pcallk(IntPtr luaState, int nargs, int nresults, int errfunc, int ignore0, int ignore1);

        [DllImport("lua_core.dll")]
        private static extern int lua_gettop(IntPtr luaState);

        [DllImport("lua_core.dll")]
        private static extern int luaL_checkinteger(IntPtr luaState, int arg);

        [DllImport("lua_core.dll")]
        private static extern IntPtr luaL_checklstring(IntPtr luaState, int arg, ref int length);

        private static string PtrToStringUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return "";

            int length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
                length++;

            if (length == 0)
                return "";

            byte[] array = new byte[length];
            Marshal.Copy(ptr, array, 0, length);
            return System.Text.Encoding.UTF8.GetString(array);
        }

        public static IntPtr NewState()
        {
            IntPtr luaState;

            if (!Environment.Is64BitOperatingSystem)
            {
                throw new Exception("JaggedLua only supports 64-bit builds for now...");
            }

            luaState = luaL_newstate();

            if (luaState != IntPtr.Zero)
            {
                luaL_openlibs(luaState);
            }

            return luaState;
        }

        public static int LoadString(IntPtr luaState, string s)
        {
            if (luaL_loadstring(luaState, s) != 0)
                return 1;
            return lua_pcallk(luaState, 0, LUA_MULTRET, 0, 0, 0);
        }

        public static void ExposeFunction(IntPtr luaState, Callback func, string name)
        {
            lua_pushcclosure(luaState, func, 0);
            lua_setglobal(luaState, name);
        }

        public static void CallFunction(IntPtr luaState, string name)
        {
            lua_getglobal(luaState, name);
            lua_pcallk(luaState, 0, 1, 0, 0, 0);
        }

        public static int GetIntegerReturn(IntPtr luaState)
        {
            return lua_tointegerx(luaState, -1, 0);
        }

        public static void ReturnInteger(IntPtr luaState, UInt64 value)
        {
            lua_pushinteger(luaState, value);
        }

        public static int NumArgs(IntPtr luaState)
        {
            return lua_gettop(luaState);
        }

        public static Tuple<bool, int> GetIntegerArg(IntPtr luaState, int index)
        {
            int retVal = 0;
            bool success = false;

            if (lua_gettop(luaState) >= 1)
            {
                retVal = luaL_checkinteger(luaState, index);
                success = true;
            }

            return Tuple.Create(success, retVal);
        }

        public static Tuple<bool, int, string> GetStringArg(IntPtr luaState, int index)
        {
            string retVal = "";
            int length = 0;
            bool success = false;

            if (lua_gettop(luaState) >= 1)
            {
                retVal = PtrToStringUtf8(luaL_checklstring(luaState, index, ref length));
                success = true;
            }

            return Tuple.Create(success, length, retVal);
        }

        public static void Close(IntPtr luaState)
        {
            lua_close(luaState);
        }

        /*
        public static void LogError()
        {
           //log error
           string errMsg = null;
           if (Lua.lua_isstring(m_lua_State, -1) &gt; 0)
               errMsg = Lua.lua_tostring(m_lua_State, -1);

           //clear the Lua stack
           Lua.lua_settop(m_lua_State, 0);

           //log or show the error somewhere in your program
        }
        */
    }
}
