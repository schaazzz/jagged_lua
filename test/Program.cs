using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JaggedLua;
using System.Threading;

namespace test
{
    class Program
    {
        static string luaScript =
            @"
            print('Lua: Hello World, I am', _VERSION);
            print('Lua: ...and now I will call a C# function!');
            sharpFunc0();

            function luaFunc0()
                r = 0xDEADBEEF;
                print('Lua: luaFunc0 - I will return ', string.format('0x%X', r));
                return r;
            end;

            function luaFunc1()
                tick = 0
                while true do
                    io.write('\rLua: luaFunc1 - tick = ', tick);
                    tick = tick + 1;
                    sharpSleep();
                end;
            end;";

        static void luaThread()
        {
            IntPtr luaState = Lua.NewState();

            Lua.Callback sharpFunc0 =
                (IntPtr _luaState) =>
                {
                    Console.WriteLine("C#: sharpFunc0: ...");
                };

            Lua.Callback sharpSleep =
                (IntPtr _luaState) =>
                {
                    Thread.Sleep(1000);
                };

            if (luaState != IntPtr.Zero)
            {
                Lua.ExposeFunction(luaState, sharpFunc0, "sharpFunc0");
                Lua.ExposeFunction(luaState, sharpSleep, "sharpSleep");

                if (Lua.LoadString(luaState, luaScript) != 0)
                {
                    Lua.Close(luaState);
                }

                Lua.CallFunction(luaState, "luaFunc0");
                Console.WriteLine("C#: luaFunc0 returned 0x{0:X}", Lua.GetIntegerReturn(luaState));

                Lua.CallFunction(luaState, "luaFunc1");
            }
        }

        static void Main(string[] args)
        {
            var t = Task.Run(() => luaThread());

            t.Wait();
            Console.WriteLine("Waiting...");
        }
    }
}
