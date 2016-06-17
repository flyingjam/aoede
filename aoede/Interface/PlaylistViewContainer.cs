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
        public PlaylistViewContainer(Audio.Walkman p)
        {
            player = p;
            player.PlaylistAdded += PlaylistAddedHandler;
            label = new Label("test");
            this.SwitchPage += SwitchPageHandler;
        }

        public void add(Audio.Playlist play)
        {
            var view = new PlaylistViewWindow(play, player);
            var tab = new PlaylistViewTab(play.Name, view, this);
            AppendPage(view, tab);
            ShowAll();
        }

        public void PlaylistAddedHandler(object obj, Audio.PlaylistAddedArgs args)
        {
            Console.WriteLine("woah");
            add(args.Addition);
        }

        private void SwitchPageHandler(object o, SwitchPageArgs args)
        {
            var playlistView = (PlaylistViewWindow)CurrentPageWidget;
            player.setPlaylist(playlistView.playlist);
        }

    }
}
