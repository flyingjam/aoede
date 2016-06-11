using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace aoede.Interface
{
    class PlaylistViewTab : HBox
    {
        Label name;
        ToolButton close;
        PlaylistViewContainer parent;
        PlaylistView view;

        public PlaylistViewTab(string label, PlaylistView v, PlaylistViewContainer p)
        {
            name = new Label(label);
            Image image = new Image(Gtk.Stock.Close);
            image.IconSize = 16;
            close = new ToolButton(Gtk.Stock.Close);
            parent = p;
            close.Clicked += CloseTabHandler;

            view = v;

            Add(name);
            Add(close);
            ShowAll();
        }

        private void CloseTabHandler(object o, EventArgs args)
        {
            parent.Remove(view);
        }
        
    }
}
