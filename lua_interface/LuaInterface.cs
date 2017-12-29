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
        public delegate void Callback (IntPtr lua_State);

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
        private static extern int lua_getglobal(IntPtr luaState, string name);

        [DllImport("lua_core.dll")]
        private static extern int lua_tointegerx(IntPtr luaState, int ignore0, int ignore1);

        [DllImport("lua_core.dll")]
        private static extern int lua_pcallk(IntPtr luaState, int nargs, int nresults, int errfunc, int ignore0, int ignore1);

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
