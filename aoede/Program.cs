﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using MoonSharp.Interpreter;
using System.Runtime.InteropServices;

namespace aoede
{

    class Program
    {
        Program()
        {
        }


        static void Main(string[] args)
        {
            //createGui();
            //moonsharptesting();            
            //playlistTest();
            //metadataTest();
            Application.Init();
            var win = new Window("SDFS:DFA");

            var tree = new Interface.MultiDragTreeView();
            win.Add(tree);
            TreeViewColumn fuck = new TreeViewColumn();
            fuck.Title = "SDLF";
            tree.AppendColumn(fuck);
            var list = new Interface.ListStoreMovable(typeof(string));//new ListStore(typeof(string));
            tree.Model = list;


            var fuckCell = new CellRendererText();
            fuck.PackStart(fuckCell, true);
            fuck.AddAttribute(fuckCell, "text", 0);
            for(int i = 0; i < 10; i++)
            {
                list.AppendValues(i.ToString());
            }

            win.ShowAll();
             
            Application.Run();


         }

        static void metadataTest()
        {
            FMOD.System system;
            FMOD.Factory.System_Create(out system);

            system.init(512, FMOD.INITFLAGS.NORMAL, (IntPtr)0);
            FMOD.Sound sound;
            system.createSound("reaper.flac", FMOD.MODE.DEFAULT, out sound);

            int numtags, numupdated;
            sound.getNumTags(out numtags, out numupdated);
            
            FMOD.TAG tag;
            for (int i = 0; i < numtags; i++)
            {
                sound.getTag(null, i, out tag);
                var str = Marshal.PtrToStringAnsi(tag.data);
                Console.WriteLine(tag.name + " | data: " + str);
            }

            Console.ReadKey();

            
        }

        static void playlistTest()
        {

            var w = new Audio.Walkman();
            var play = w.createPlaylist("s.mp3", "s.flac", "t.mp3", "r.mp3");

            w.tagMaster.add(new Audio.Music("s.mp3"), "mp3");
            w.tagMaster.add(new Audio.Music("t.mp3"), "mp3");
            w.tagMaster.add(new Audio.Music("r.mp3"), "mp3");

            var mp3 = w.query(play, new Audio.Tag("mp3"));

			Script script = new Script ();


			string test = @"
function test(x)
	return x * x
end

return test";

			var t = script.DoString(test);
			var fun = t.Function;
			Console.WriteLine (fun.GetType ());
			var res = fun.Call (2);
			Console.WriteLine (res.CastToNumber ());

            Console.WriteLine(mp3);
            Console.ReadKey();

        }

        static void createGui()
        {
            Application.Init();
            Interface.GUI gui = new Interface.GUI();
            gui.ShowAll();
            Application.Run();

        }

        static void moonsharptesting()
        {
            var script = new Script();
            string newRepl = @"
function print_results(...)

    if select('#', ...) > 1 then
        print(select(2, ...))
    end
end

function repl(input)
	local f, err = load('return ' .. input)
	if err then
		f = load(input)
	end
	
    local res, e = pcall(f)
	if (f ~= nil) and res then
		print_results(pcall(f))
	else
		print(err)
	end
end

";
            script.DoString(newRepl);
            while (true)
            {
                string input = Console.ReadLine();
                var v = script.Call(script.Globals["repl"], input);

            }
        }

    }
}
