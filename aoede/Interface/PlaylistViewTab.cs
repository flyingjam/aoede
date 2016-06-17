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
        Button close;
        PlaylistViewContainer parent;
        PlaylistViewWindow view;

        public PlaylistViewTab(string label, PlaylistViewWindow v, PlaylistViewContainer p)
        {
            parent = p;
            name = new Label(label);
            Image image = new Image(Gtk.Stock.Close);
            image.IconSize = 16;
            close = new Button("x");
            close.Clicked += CloseTabHandler;
            close.Relief = ReliefStyle.None;

            view = v;

            this.PackStart(name, false, false, 0);
            this.PackStart(close, false, false, 0);
            ShowAll();
        }

        private void CloseTabHandler(object o, EventArgs args)
        {
            parent.Remove(view);
        }
        
    }
}
