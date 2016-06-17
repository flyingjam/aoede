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
                player.Volume(0.3);
                consoleHide = false; 
                console = new AConsole(player);
                menu = new Menu(player);

                
                label = new Label("shit");
                box = new VBox();
                music = new PlaylistViewContainer(player);

                //box.Add(menu);
                //box.Add(music);
                //box.Add(console);
                var menutest = new MenuBar();
                menutest.Add(new Label("test"));
                menutest.Style = menu.Style;
                //menutest.HeightRequest = 30;
                //box.PackStart(menutest, false, false, 0);
                menu.HeightRequest = 30;

                var hboxtest = new HBox();
                hboxtest.HeightRequest = 30;
                box.PackStart(menu, false, false, 0);
                box.PackStart(music, true, true, 0);
                box.PackStart(console, false, false, 0);
                this.SetDefaultSize(1280, 720);

                var p = player.createPlaylist("Play1", "s.flac", "s.mp3", "reaper.flac", "t.mp3");
                player.createPlaylist("Test", "r.mp3", "reaper.flac");
                player.setPlaylist(p);
                Add(box); 
            }

            private void ButtonPlay(object obj, EventArgs args)
            {
                player.Play();
            }

            private void ButtonStop(object obj, EventArgs args)
            {
                player.Stop();
            }

            private void ButtonPause(object obj, EventArgs arg)
            {
                player.Pause();
            }

            private void ButtonNext(object obj, EventArgs arg)
            {
                player.Next();
            }

            private void ButtonPrevious(object obj, EventArgs arg)
            {
                player.Previous();
            }


            private void quit(object obj, EventArgs arg)
            {
                Application.Quit();
                player.release();
            }

            [GLib.ConnectBefore]
            private void OpenConsole(object obj, KeyPressEventArgs args)
            {
                if (args.Event.Key == Gdk.Key.quoteleft)
                {
                    consoleHide = !consoleHide;
                    args.RetVal = true;
                }

                if (consoleHide)
                    console.Hide();
                else
                    console.Show();
            }



        }
    }
}
