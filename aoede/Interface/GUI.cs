using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using Gtk;

namespace aoede
{
    namespace Interface
    {
        class GUI : Window
        {
            Audio.Walkman player;
            Label label;
            VBox box;
            AConsole console;
            bool consoleHide;

            Menu menu;

            PlaylistViewContainer music;

            public GUI() : base("yo")
            {
                //GLib.Timeout.Add(5, GlobalKeyChecker);
                Destroyed += quit;
                KeyPressEvent += OpenConsole;

                player = new Audio.Walkman();
                consoleHide = false; 
                console = new AConsole(player);
                menu = new Menu(player);

                var p = player.createPlaylist("s.flac", "s.mp3", "reaper.flac", "t.mp3");
                player.setPlaylist(p);
                
                label = new Label("shit");
                box = new VBox();
                music = new PlaylistViewContainer(p, player);

                box.Add(menu);
                box.Add(music);
                box.Add(console);

                Add(box); 
            }

            private void ButtonPlay(object obj, EventArgs args)
            {
                player.play();
            }

            private void ButtonStop(object obj, EventArgs args)
            {
                player.stop();
            }

            private void ButtonPause(object obj, EventArgs arg)
            {
                player.pause();
            }

            private void ButtonNext(object obj, EventArgs arg)
            {
                player.next();
            }

            private void ButtonPrevious(object obj, EventArgs arg)
            {
                player.previous();
            }


            private void quit(object obj, EventArgs arg)
            {
                Application.Quit();
                player.release();
            }

            [GLib.ConnectBefore]
            private void OpenConsole(object obj, KeyPressEventArgs args)
            {
                if (args.Event.Key == Gdk.Key.Tab)
                    consoleHide = !consoleHide;

                if (consoleHide)
                    console.Hide();
                else
                    console.Show();

            }



        }
    }
}
