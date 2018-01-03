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
            print('Lua: Hello World, I am', _VERSION)
            
            print('Lua: ...let\'s call a C# function without any parameters!')
            sharpFunc0()
            
            print('Lua: ...now let\'s try to call a C# function with an integer and a string parameter!')
            sharpFunc1(0xFACADE, 'Hello C#')

            print('Lua: ...now we\'ll call a C# function which returns a value!')            
            n = sharpFunc2()
            print('Lua: sharpFunc2 returned', string.format('0x%X', n))

            io.write('Lua: Did it work? [y/n] ')
            
            i = ''
            while true do
                i = io.read()
                if i:upper() == 'Y' or i:upper() == 'N' then
                    break
                else
                    io.write('Lua: I\'m afraid I can only accept \'y\' or \'n\' as input...')
                end
            end

            function luaFunc0()
                r = 0xDEADBEEF
                print('Lua: luaFunc0 - I will return', string.format('0x%X', r))
                return r
            end

            function luaFunc1()
                tick = 0
                while true do
                    io.write('\rLua: luaFunc1 - tick = ', tick)
                    tick = tick + 1
                    sharpSleep(1000)
                end
            end";

        static void luaThread()
        {
            IntPtr luaState = Lua.NewState();

            Lua.Callback sharpFunc0 = (IntPtr _luaState) =>
            {
                Console.WriteLine("C#: sharpFunc0");
                return 0;
            };

            Lua.Callback sharpFunc1 = (IntPtr _luaState) =>
            {
                Tuple<bool, int> arg0 = Lua.GetIntegerArg(luaState, 1);
                Tuple<bool, int, string> arg1 = Lua.GetStringArg(_luaState, 2);

                if (arg0.Item1 && arg1.Item1 && (arg1.Item2 != 0))
                {
                    Console.WriteLine("C#: sharpFunc1 - Args [Integer: 0x{0:X}, String (length {1}): '{2}']", arg0.Item2, arg1.Item2, arg1.Item3);
                }

                return 0;
            };

            Lua.Callback sharpFunc2 = (IntPtr _luaState) =>
            {
                UInt64 retVal = 0xBAADF00D;
                Console.WriteLine("C#: sharpFunc2 - I will return 0x{0:X}", retVal);

                Lua.ReturnInteger(_luaState, retVal);
                return 1;
            };

            Lua.Callback sharpSleep =
                (IntPtr _luaState) =>
                {
                    int delay = 100;
                    if (Lua.NumArgs(_luaState) == 1)
                    {
                        Tuple<bool, int> arg = Lua.GetIntegerArg(luaState, 1);
                        delay = arg.Item1 ? arg.Item2 : delay;

                    }
                    Thread.Sleep(delay);
                    return 0;
                };

            if (luaState != IntPtr.Zero)
            {
                Lua.ExposeFunction(luaState, sharpFunc0, "sharpFunc0");
                Lua.ExposeFunction(luaState, sharpFunc1, "sharpFunc1");
                Lua.ExposeFunction(luaState, sharpFunc2, "sharpFunc2");
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
