using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gdk;
using Gtk;
using MoonSharp.Interpreter;

namespace aoede.Interface
{

    delegate Audio.Playlist playlistType(string name, params string[] filepaths);

    class AConsole : VBox
    {
        Label label = new Label("");
        TextView text = new TextView();
        ConsoleInput input;
        EventBox labelWrapper = new EventBox();
        ScrolledWindow scrolledText = new ScrolledWindow();
        VBox layout = new VBox();
        Audio.Walkman player;

        Script script = new Script();

        CommandHistory history = new CommandHistory();

        public AConsole(Audio.Walkman walk)
        {
            player = walk;
            label.SetAlignment(0, 0);

            input = new ConsoleInput(this);

            script.Options.DebugPrint = s => { println(s); };
            MoonSharp.Interpreter.UserData.RegisterType<Audio.Walkman>();
            MoonSharp.Interpreter.UserData.RegisterType<Audio.Playlist>();
            MoonSharp.Interpreter.UserData.RegisterType<Audio.Music>();
            MoonSharp.Interpreter.UserData.RegisterType<Audio.Tag>();
            MoonSharp.Interpreter.UserData.RegisterType<Audio.PlaylistList>();
            MoonSharp.Interpreter.UserData.RegisterType<LuaTest>();
            DynValue luaWalk = MoonSharp.Interpreter.UserData.Create(player);
            var t = MoonSharp.Interpreter.UserData.Create(new LuaTest(player));

            //debugging purposes, will remove later
            script.Globals.Set("system", luaWalk);

            script.Globals["volume"] = (Action<double>)player.Volume;
            script.Globals["play"] = (System.Action)player.Play;
            script.Globals["pause"] = (System.Action)player.Pause;
            script.Globals["stop"] = (System.Action)player.Stop;
            script.Globals["next"] = (System.Action)player.Next;
            script.Globals["previous"] = (System.Action)player.Previous;
            script.Globals["createPlaylist"] = (playlistType)player.createPlaylist;


            //Gdk.Color col = new Gdk.Color();
            //Gdk.Color.Parse("purple", ref col);
            //labelWrapper.ModifyBg(StateType.Normal, col);
            labelWrapper.Add(label);

            //awful and hacky way to make lua evalute expressions, but it works
            string newRepl = @"
                        sm = setmetatable
                        function infix(f)
                          local mt = { __div = function(self, b) return f(self[1], b) end }
                          return sm({}, { __div = function(a, _) return sm({ a }, mt) end })
                        end

                        p = infix(function(a, b) return b(a) end)

                        function print_results(...)
                            if select('#', ...) > 1 then
                                print(select(2, ...))
                            end
                        end

                        function repl(input)
                            local f, err = load('return ' .. input)
                            local is_statement = false
                            if err then
                                is_statement = true
                                f = load(input)
                            elseif f == nil then
                                is_statement = true
                            end
                            
                            if f then
                                if is_statement then
                                    pcall(f)
                                else
                                    print_results(pcall(f))
                                end
                            else
                                print(err)
                            end
                        end
";
            script.DoString(newRepl);
                

            input.Activated += inputTextHandler;

            scrolledText.Add(text);


            PackStart(scrolledText, true, true, 0);
            PackStart(input, false, false, 0);
            HeightRequest = 200;

            println("//Use the global variable 'player' to play music, etc.");
            println("//For example, player.play() plays the current track and player.stop() stops");

        }

        private void ConnectSignals()
        {
            input.KeyPressEvent += KeyPressHandler;
        }

        public void HistoryKeyHandler(Gdk.Key key)
        {
            if (key == Gdk.Key.Up)
            {
                history.Previous();
                input.Text = history.Get();
            }
            else if (key == Gdk.Key.Down)
            {
                history.Next();
                input.Text = history.Get();
            }

        }

        [GLib.ConnectBefore]
        private void KeyPressHandler(object o, KeyPressEventArgs args)
        {
            Console.WriteLine("PRESSING");
            if (args.Event.Key == Gdk.Key.Up)
            {
                history.Previous();
                input.Text = history.Get();
            }
            else if (args.Event.Key == Gdk.Key.Down)
            {
                history.Next();
                input.Text = history.Get();
            }
        }

        private void inputTextHandler(object obj, EventArgs args)
        {
            var inp = (Entry)obj;
            evaluateScript(inp.Text);
            history.Add(inp.Text);
            inp.Text = "";
        }

        private void evaluateScript(string code)
        {
            try
            {
                println(">" + code);
                DynValue value = script.Call(script.Globals["repl"], code);
            }
            catch (Exception e)
            {
                println("Error");
            }
        } 

        private void print(string str)
        {
            text.Buffer.Text += str;
        }

        private void println(string str)
        {
            //probably extra string allocations, I don't care 
            var end = text.Buffer.EndIter;
            text.Buffer.Insert(ref end, str + "\n");
            ScrollToEnd(); 
        }

        private void ScrollToEnd()
        {
            var end = text.Buffer.EndIter;
            text.ScrollToIter(end, 0, false, 0, 0);
        }
    }

    class LuaTest
    {
        Audio.Walkman play;
        public LuaTest(Audio.Walkman p)
        {
            play = p;
        }

        public DynValue createPlaylist(string name, params string[] path)
        {
            return UserData.Create(play.createPlaylist(name, path));
        }

    }

    class ConsoleWriter : System.IO.TextWriter
    {

        TextView text;
        public ConsoleWriter(TextView view)
        {
            text = view;
        }

        public override void Write(char value)
        {
            var end = text.Buffer.EndIter;
            text.Buffer.Insert(ref end, value + "\n");
            end = text.Buffer.EndIter;
            text.ScrollToIter(end, 0, false, 0, 0);
        }

        public override void Write(string value)
        {

        }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.ASCII;
            }
        }
    }

    class ConsoleInput : Entry
    {
        AConsole parent;

        public ConsoleInput(AConsole console)
        {
            parent = console;
            this.KeyPressEvent += ConsoleInput_KeyPressEvent;
        }

        private void ConsoleInput_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Up || args.Event.Key == Gdk.Key.Down)
            {
                parent.HistoryKeyHandler(args.Event.Key);
                args.RetVal = true;
            }
        }

        protected override bool OnKeyPressEvent(EventKey evnt)
        {
            return base.OnKeyPressEvent(evnt);
        }
    }

    class CommandHistory
    {
        public List<string> History { get; private set; } = new List<string>();
        public int index = 0;

        public CommandHistory()
        {

        }

        public void Add(string command)
        {
            History.Add(command);
            index = History.Count - 1;
        }

        public void Previous()
        {
            index = Misc.Clamp(index - 1, 0, History.Count - 1);
        }

        public void Next()
        {
            index = Misc.Clamp(index + 1, 0, History.Count - 1);
        }

        public string Get()
        {
            return History[index];
        }


    }
}
