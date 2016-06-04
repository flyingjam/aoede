using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using MoonSharp.Interpreter;

namespace aoede.Interface
{
    class AConsole : VBox
    {
        Label label;
        Entry input;
        EventBox labelWrapper;
        Audio.Walkman player;

        Script script;

        public AConsole(Audio.Walkman walk)
        {
            player = walk;
            label = new Label("");
            input = new Entry();
            labelWrapper = new EventBox();

            script = new Script();
            script.Options.DebugPrint = s => { println(s); };
            MoonSharp.Interpreter.UserData.RegisterType<Audio.Walkman>();
            MoonSharp.Interpreter.UserData.RegisterType<Audio.Playlist>();
            DynValue luaWalk = MoonSharp.Interpreter.UserData.Create(player);
            script.Globals.Set("player", luaWalk);


            Gdk.Color col = new Gdk.Color();
            Gdk.Color.Parse("purple", ref col);
            labelWrapper.ModifyBg(StateType.Normal, col);
            labelWrapper.Add(label);

            //awful and hacky way to make lua evalute expressions, but it works
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
                            
                            if f then
                                print_results(pcall(f))
                            else
                                print(err)
                            end
                        end
";
            script.DoString(newRepl);
                

            input.Activated += inputTextHandler;
            Add(labelWrapper);
            Add(input);

            println("//Use the global variable 'player' to play music, etc.");
            println("//For example, player.play() plays the current track and player.stop() stops");

        }

        private void inputTextHandler(object obj, EventArgs args)
        {
            var inp = (Entry)obj;
            evaluateScript(inp.Text);
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
            label.Text += str;
        }

        private void println(string str)
        {
           
            //probably extra string allocations, I don't care 
            label.Text += ("\n" + str);
        }
    }
}
