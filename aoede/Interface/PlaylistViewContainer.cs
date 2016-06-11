using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace aoede.Interface
{
    class PlaylistViewContainer : Notebook
    {

        PlaylistView test;
        Label label;
        Audio.Walkman player;
        public PlaylistViewContainer(Audio.Playlist play, Audio.Walkman p)
        {
            player = p;
            var p2 = player.createPlaylist("r.mp3", "t.mp3");
            test = new PlaylistView(play, player);
            var test2 = new PlaylistView(p2, player);
            label = new Label("test");
            add(play);
            add(p2);
            this.SwitchPage += SwitchPageHandler;
            
        }

        public void add(Audio.Playlist play)
        {
            var view = new PlaylistView(play, player);
            var tab = new PlaylistViewTab("", view, this);
            AppendPage(view, tab);
        }

        private void SwitchPageHandler(object o, SwitchPageArgs args)
        {
            var playlistView = (PlaylistView)CurrentPageWidget;
            player.setPlaylist(playlistView.internalPlaylist);
            Console.WriteLine("SWITCH");
        }

    }
}
