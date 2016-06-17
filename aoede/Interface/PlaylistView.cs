using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Gtk;
using Gdk;
using SFML;

namespace aoede.Interface
{
    class PlaylistViewWindow : MultiDragScrolled
    {
        PlaylistView tree;
        public Audio.Playlist playlist { get; private set; }
        public PlaylistViewWindow(Audio.Playlist play, Audio.Walkman walk)
        {
            tree = new PlaylistView(play, walk, this);
            Add(tree);
            playlist = tree.internalPlaylist;
        }

          
    }

    class PlaylistView : MultiDragTreeView
    {
        TargetEntry[] t =
        {
            new TargetEntry("text/uri-list", 0, 2)
        };

        TreeViewColumn fileName = new TreeViewColumn();
        TreeViewColumn artist = new TreeViewColumn();

        ListStore musicList = new ListStoreMoveable(typeof(string), typeof(string), typeof(Guid), typeof(int));
        CellRendererText fileNameCell = new CellRendererText();
        CellRendererText artistCell = new CellRendererText();
        Audio.Walkman player;

        public Audio.Playlist internalPlaylist { get; private set; }
        public PlaylistView(Audio.Playlist play, Audio.Walkman walk, Scrollable parent) : base(parent)
        {
            
            //Create target list
            var combined = this.targets.Concat(t).ToArray();
            Gtk.Drag.DestSet(this, DestDefaults.All, combined, DragAction.Move | DragAction.Copy);
            
            internalPlaylist = play;
            player = walk;

            fileName.Title = "File Name";
            AppendColumn(fileName);

            artist.Title = "Artist";
            AppendColumn(artist);

            updateView();

            fileName.PackStart(fileNameCell, false);

            artist.PackStart(artistCell, false);

            fileName.AddAttribute(fileNameCell, "text", 0);
            fileName.Resizable = true;
            artist.AddAttribute(artistCell, "text", 1);
            artist.Resizable = true;

            this.ResizeMode = ResizeMode.Immediate;
            this.HeadersClickable = true;

            Model = musicList;

            Selection.Mode = SelectionMode.Multiple;
            RubberBanding = true;

            ConnectSignals();
        }

        private void ConnectSignals()
        {
            DragDrop += DragDataHandler;
            DragDataGet += DragDataGetHandler;
            DragDataReceived += DragDataReceivedHandler;
            RowActivated += ActivatedHandler;
            DragMotion += PlaylistView_DragMotion;
        }

        private void PlaylistView_DragMotion(object o, DragMotionArgs args)
        {
        }

        public override void moveCallback(List<TreeIter> items, TreeModel model, TreeIter target, bool after)
        {
            makePlaylistSameAsView();
        }

        private void makePlaylistSameAsView()
        {
            var list = new List<Audio.Music>(internalPlaylist.music.Count);
            Model.Foreach(new TreeModelForeachFunc(delegate (TreeModel m, TreePath p, TreeIter i)
            {
                var PID = (int)m.GetValue(i, 3);
                list.Add(internalPlaylist.GetAt(PID));
                return false;
            }));

            internalPlaylist.music = list;

            foreach(var m in internalPlaylist.music)
            {
                Console.WriteLine(m.Filepath);
            }
        }

        public void updateView()
        {
            foreach (Audio.Music m in internalPlaylist.music)
            {
                if (!listStoreContains(m.PlaylistID))
                {
                    var artist = player.getTag(m, "ARTIST");
                    musicList.AppendValues(m.Filename, artist.Value, m.UUID, m.PlaylistID);
                }
            }

        }

        private bool listStoreContains(int PID)
        {
            foreach (object[] row in musicList)
            {
                var pid = (int)row[3];
                if (PID == pid)
                {
                    return true;
                }
            }

            return false;
        }

        public void add(Audio.Music m)
        {
            internalPlaylist.add(m);
            updateView();
        }

        public void add(List<string> fileNames)
        {
            foreach (string file in fileNames)
            {
                if (!Misc.isUnicodeNull(file))
                {
                    var music = player.createMusic(file);
                    add(music);
                }
            }
        }

        public void DragDataHandler(object o, DragDropArgs args)
        {
            if (args.Context.Targets[0].Name == "text/uri-list")
            {
                Gtk.Drag.GetData((Widget)o, args.Context, args.Context.Targets[0], args.Time);
            }
        }

        public void DragDataReceivedHandler(object o, DragDataReceivedArgs args)
        {
            if (args.Context.Targets[0].Name == "text/uri-list")
            {
                var context = args.Context;
                var thing = args.SelectionData.Text;
                var data = args.SelectionData.Data;
                string encoded = System.Text.Encoding.UTF8.GetString(data);
                var paths = encoded.Split('\r').Select(x => Uri.UnescapeDataString(x.Replace("file:///", "")).Trim()).ToList<string>();
                foreach(var path in paths)
                {
                    Console.WriteLine(path);
                }
                add(paths);
            }
        }

        public void DragDataGetHandler(object o, DragDataGetArgs args)
        {
            Console.WriteLine(args.Context.Targets[0].Name);
        }

        private void CursorChangedHandler(object o, EventArgs args)
        {
            var selection = ((TreeView)o).Selection;
            TreeIter iter;
            TreeModel model;
            selection.SelectedForeach(new TreeSelectionForeachFunc(delegate (TreeModel m, TreePath path, TreeIter i)
            {

                var value = (string)m.GetValue(i, 0);
                foreach (char c in value) ;
                //Console.WriteLine((value as string));
            }));

            if (selection.GetSelected(out model, out iter))
            {
                Console.WriteLine(model.GetPath(iter));
            }
            if (model != null)
                Console.WriteLine("why");
        }

        private void ActivatedHandler(object o, RowActivatedArgs args)
        {
            TreeIter iter;
            musicList.GetIter(out iter, args.Path);
            var data = (int)musicList.GetValue(iter, 3);
            var name = (string)musicList.GetValue(iter, 0);
            Console.WriteLine(name);
            Console.WriteLine(data);
            if (!player.playlistSeekPID(data))
                Console.WriteLine("FUCK");
            player.Play();
        }


    }
}
