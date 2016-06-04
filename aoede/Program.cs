using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using MoonSharp.Interpreter;

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
            playlistTest();
        }

        static void playlistTest()
        {

            var w = new Audio.Walkman();
            var play = w.createPlaylist("s.mp3", "s.flac", "t.mp3", "r.mp3");
            w.tagMaster.add(new Audio.Music("s.mp3"), "mp3");
            w.tagMaster.add(new Audio.Music("t.mp3"), "mp3");
            w.tagMaster.add(new Audio.Music("r.mp3"), "mp3");

            var mp3 = w.query(play, new Audio.Tag("mp3"));

            //find all musics with tag mp3 in playlist

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
