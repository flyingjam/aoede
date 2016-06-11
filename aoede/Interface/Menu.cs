using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace aoede.Interface
{
    class Menu : HBox
    {
        MenuToolbar toolbar;
        Label temp;
        Audio.Walkman player;
        public Menu(Audio.Walkman p)
        {
            player = p;
            toolbar = new MenuToolbar(p);
            temp = new Label("WHER THE LINE THING IS");
            Add(toolbar);
            Add(temp);
        }
    }

    class MenuToolbar : Toolbar
    {
        ToolButton start, pause, stop, next, previous;
        Audio.Walkman player;

        public MenuToolbar(Audio.Walkman p)
        {

            player = p;

            this.ToolbarStyle = Gtk.ToolbarStyle.Icons;

            start = new ToolButton(Stock.MediaPlay);
            stop = new ToolButton(Stock.MediaStop);
            pause = new ToolButton(Stock.MediaPause);
            previous = new ToolButton(Stock.MediaPrevious);
            next = new ToolButton(Stock.MediaNext);

            this.Insert(start, 0);
            this.Insert(stop, 1);
            this.Insert(pause, 2);
            this.Insert(previous, 3);
            this.Insert(next, 4);


            connectSignals();
        } 

        private void connectSignals()
        {

            start.Clicked += StartHandler;
            stop.Clicked += StopHandler;
            pause.Clicked += PauseHandler;
            next.Clicked += NextHandler;
            previous.Clicked += PreviousHandler;
            
        }

        private void StartHandler(object o, EventArgs args)
        {
            player.play();            
        }

        private void StopHandler(object o, EventArgs args)
        {
            player.stop();
        }

        private void PauseHandler(object o, EventArgs args)
        {
            player.pause();
        }

        private void NextHandler(object o, EventArgs args)
        {
            player.next();
        }

        private void PreviousHandler(object o, EventArgs args)
        {
            player.previous();
        }

    }

}
