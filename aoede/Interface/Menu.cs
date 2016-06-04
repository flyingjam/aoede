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
        public Menu()
        {
            toolbar = new MenuToolbar();
            temp = new Label("WHER THE LINE THING IS");
            Add(toolbar);
            Add(temp);
        }
    }

    class MenuToolbar : Toolbar
    {
        ToolButton start, pause, stop, next, previous;

        public MenuToolbar()
        {
            this.ToolbarStyle = Gtk.ToolbarStyle.Icons;

            start = new ToolButton(Stock.MediaPlay);
            stop = new ToolButton(Stock.MediaStop);
            pause = new ToolButton(Stock.MediaPause);
            next = new ToolButton(Stock.MediaNext);
            previous = new ToolButton(Stock.MediaPrevious);
            this.Insert(start, 0);
            this.Insert(stop, 1);
            this.Insert(pause, 2);
            this.Insert(next, 3);
            this.Insert(previous, 4);
        } 
    }

}
