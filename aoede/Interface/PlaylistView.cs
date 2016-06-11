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
    class PlaylistView : TreeView
    {
        TreeViewColumn fileName;
        TreeViewColumn UUID;
        TreeViewColumn artist;

        ListStore musicList;
        CellRendererText fileNameCell;
        CellRendererText artistCell;
        Audio.Walkman player;
        bool isMouseDown = false;
        int previousSelectionNumber = 0;

        public Audio.Playlist internalPlaylist { get; }

        int MouseY = 0;

        public PlaylistView(Audio.Playlist play, Audio.Walkman walk) : base()
        {
            PlaySelect sele = new PlaySelect();
            TargetEntry[] entries = new TargetEntry[1];
            var entry = new TargetEntry("PlaylistView", TargetFlags.Widget, 0);
            entry.Target = "STRING";
            entry.Target = "text/plain";
            entries[0] = entry;
            //Reorderable = true;
            this.DragDrop += DragDataHandler;
            this.DragDataGet += DragDataGetHandler;
            //this.EnableModelDragSource(ModifierType.Button1Mask, null, DragAction.Move);
            internalPlaylist = play;
            player = walk;

            fileName = new TreeViewColumn();
            fileName.Title = "File Name";
            AppendColumn(fileName);

            artist = new TreeViewColumn();
            artist.Title = "Artist";
            AppendColumn(artist);

            musicList = new ListStore(typeof(string), typeof(string), typeof(Guid));
            updateView();

            fileNameCell = new CellRendererText();
            fileName.PackStart(fileNameCell, true);

            artistCell = new CellRendererText();
            artist.PackStart(artistCell, true);

            fileName.AddAttribute(fileNameCell, "text", 0);
            artist.AddAttribute(artistCell, "text", 1);
            Model = musicList;
            //Reorderable = true;

            //this.ButtonReleaseEvent += MouseReleaseHandler;
            //musicList.RowsReordered += ReorderedHandler;

            var selection = this.Selection;
            previousSelectionNumber = selection.CountSelectedRows();
            selection.Mode = SelectionMode.Multiple;
            //Reorderable = true;

            TreeIter iter;
            musicList.IterNthChild(out iter, 3);
            selection.SelectIter(iter);

            this.SelectionClearEvent += delegate
            {
                Console.WriteLine("selected?");
            };

            //this.ButtonPressEvent += MousePressHandler;
            //this.SelectionNotifyEvent += test; 
            //this.SelectionReceived += test; 
            //this.SelectionRequestEvent += test;
            //this.SelectAll += test;
            //this.Selection.Changed += test;
            //Reorderable = true;
            RubberBanding = true;


        }

        private void PlaylistView_DragMotion(object o, DragMotionArgs args)
        {
            MouseY = args.Y;
            Console.WriteLine("MOVE");
        }
         
        private void DE(object obj, DragEndArgs args)
        {
            Console.WriteLine("END" + MouseY);
            var source = Gtk.Drag.GetSourceWidget(args.Context);
            var win = args.Context.DestWindow;
            if (args.Context.Targets.Count() > 0)
            {
            }

            TreeModel model;
            var selection = this.Selection.GetSelectedRows(out model);
            TreePath target;
            TreeIter targetIter;

            this.GetPathAtPos(0, MouseY, out target);
            Model.GetIter(out targetIter, target);
            foreach(var item in selection)
            {
                TreeIter itemIter;
                Model.GetIter(out itemIter, item);
                musicList.MoveAfter(itemIter, targetIter);

            }

            MouseY = 0;

        }


        public void updateView()
        {
            foreach (Audio.Music m in internalPlaylist.music)
            {
                if (!listStoreContains(m.UUID))
                {
                    var artist = player.getTag(m, "ARTIST");
                    musicList.AppendValues(m.filepath, artist.value, m.UUID);
                }
            }

        }

        private bool listStoreContains(Guid UUID)
        {
            foreach (object[] row in musicList)
            {
                var uuid = (Guid)row[2];
                if (uuid == UUID)
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

            Gtk.Drag.GetData((Widget)o, args.Context, args.Context.Targets[0], args.Time);
            Console.WriteLine("yeah");
        }

        public void DragDataReceivedHandler(object o, DragDataReceivedArgs args)
        {
    
            var context = args.Context;
            var thing = args.SelectionData.Text;
            var data = args.SelectionData.Data;
            string encoded = System.Text.Encoding.UTF8.GetString(data);
            var paths = encoded.Split('\r').Select(x => Uri.UnescapeDataString(x.Replace("file:///", "")).Trim()).ToList<string>();
            add(paths);
        }

        public void DragDataGetHandler(object o, DragDataGetArgs args)
        {
            Console.WriteLine("get");
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
            var data = (Guid)musicList.GetValue(iter, 2);
            player.playlistSeek(data);
            player.play();
        }

        [GLib.ConnectBefore]
        private void ReorderedHandler(object o, RowsReorderedArgs args)
        {
            Console.WriteLine("Reorder");
        }

        [GLib.ConnectBefore]
        private void MouseReleaseHandler(object o, ButtonReleaseEventArgs args)
        {
            var obj = (TreeView)o;
            var col = obj.Columns[0];

            Gdk.Rectangle rect = obj.VisibleRect;
            var newRect = new Gdk.Rectangle();
            int x_offset, y_offset, width, height;
            fileNameCell.GetSize(this, ref newRect, out x_offset, out y_offset, out width, out height);
            var pad = fileNameCell.Ypad;
            var asdlfa = fileNameCell.Size;

            var y = args.Event.Y;
            var index = y / (height + 2 * pad);
            //musicList.Insert((int)index);
            //Console.WriteLine((int)index);
        }

        private void getByIndex(int index)
        {
            TreeIter iter;
            //musicList.GetIterFirst(out iter);
            musicList.IterNthChild(out iter, index);
        }






    }

    class PlaySelect : TreeSelection
    {
        public PlaySelect()
        {

        }

    }
}
