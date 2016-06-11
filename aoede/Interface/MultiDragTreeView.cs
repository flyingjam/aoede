using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using Gdk;

namespace aoede.Interface
{

    interface TreeStoreMovable
    {
        void After(TreeIter item, TreeIter target);
        void Before(TreeIter item, TreeIter target);
        void Next(ref TreeIter iter);
    }

    class ListStoreMovable : ListStore, TreeStoreMovable
    {
        public ListStoreMovable(params Type[] type) : base(type)
        {
            
        }

        public void After(TreeIter item, TreeIter target)
        {
            MoveAfter(item, target);
        }
        
        public void Before(TreeIter item, TreeIter target)
        {
            MoveBefore(item, target);
        }

        public void Next(ref TreeIter item)
        {
            IterNext(ref item);
        }
    }

    class MultiDragTreeView : TreeView
    {
        TreePath deferSelect = null;

        static TargetEntry[] targets =
        {
            new TargetEntry("text/internal-list", TargetFlags.Widget, 0),
            new TargetEntry("text/internal-list", TargetFlags.OtherWidget, 1)
        };
        
        public MultiDragTreeView()
        {
            SetDest();
            ConnectSignals();
        }
        
        private void SetDest()
        {
            Gtk.Drag.SourceSet(this, ModifierType.Button1Mask, targets, DragAction.Move);
            Gtk.Drag.DestSet(this, DestDefaults.All, targets, DragAction.Move | DragAction.Copy);
        }

        private void ConnectSignals()
        {
            this.ButtonPressEvent += onButtonPress;
            this.ButtonReleaseEvent += onButtonRelease;
            this.DragMotion += MultiDragTreeView_DragMotion;
            this.DragLeave += MultiDragTreeView_DragLeave;
            this.DragDataGet += MultiDragTreeView_DragDataGet;
            this.DragDataReceived += MultiDragTreeView_DragDataReceived;
            this.DragBegin += drag;
            Selection.Mode = SelectionMode.Extended;
        }

        private void MultiDragTreeView_DragDataReceived(object o, DragDataReceivedArgs args)
        {
            if (args.Context.Targets[0].Name == "text/internal-list")
            {
                Console.WriteLine("RECIEVED");
                TreeModel model;

                TreePath path;
                TreeViewDropPosition pos;
                if (this.GetDestRowAtPos(args.X, args.Y, out path, out pos))
                {
                    if (pos == TreeViewDropPosition.IntoOrBefore)
                        pos = TreeViewDropPosition.Before;
                    else if (pos == TreeViewDropPosition.IntoOrAfter)
                        pos = TreeViewDropPosition.After;
                }
                TreeIter targetIter;

                var selection = Selection.GetSelectedRows(out model);
                model.GetIter(out targetIter, path);

                List<TreeIter> thing = new List<TreeIter>();
                foreach (var item in selection)
                {
                    TreeIter i;
                    model.GetIter(out i, item);
                    thing.Add(i);
                }

                foreach (var item in thing)
                {
                    var i = item;
                    var m = (TreeStoreMovable)model;
                    if (pos == TreeViewDropPosition.After)
                    {
                        m.After(item, targetIter);
                        m.Next(ref targetIter);
                    }
                    else
                        m.Before(item, targetIter);
                }
            }
        }

        private void move(bool before)
        {
            var m = (ListStore)Model;
        }

        private void MultiDragTreeView_DragDataGet(object o, DragDataGetArgs args)
        {
            if (args.Context.Targets[0].Name == "text/internal-list")
            {
                args.SelectionData.Set(args.Context.Targets[0], 8, System.Text.Encoding.UTF8.GetBytes(""));
            }
        }

        private void MultiDragTreeView_DragLeave(object o, DragLeaveArgs args)
        {
            this.SetDragDestRow(null, 0);
        }

        private void MultiDragTreeView_DragMotion(object o, DragMotionArgs args)
        {
            Gdk.Drag.Status(args.Context, args.Context.SuggestedAction, args.Time);
            int x, y;
            this.ConvertWidgetToTreeCoords(args.X, args.Y, out x, out y);
            var p = belowOrAbove(args.X, args.Y);
            TreePath path;
            TreeViewDropPosition pos;
            if(this.GetDestRowAtPos(args.X, args.Y, out path, out pos))
            {
                if (pos == TreeViewDropPosition.IntoOrBefore)
                    pos = TreeViewDropPosition.Before;
                else if (pos == TreeViewDropPosition.IntoOrAfter)
                    pos = TreeViewDropPosition.After;
                this.SetDragDestRow(path, pos);
            }
        }

        private TreeViewDropPosition belowOrAbove(int x, int y)
        {
            TreePath path;
            TreeViewDropPosition pos;
            GetDestRowAtPos(x, y, out path, out pos);
            if (pos == TreeViewDropPosition.IntoOrBefore)
                return TreeViewDropPosition.Before;
            else if (pos == TreeViewDropPosition.IntoOrBefore)
                return TreeViewDropPosition.After;
            return pos;
        }

        [GLib.ConnectBefore]
        public void onButtonPress(object obj, ButtonPressEventArgs args)
        {
            TreePath target;
            this.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out target);
            if((target != null) 
                && args.Event.Type == EventType.ButtonPress
                && !(args.Event.State.HasFlag(Gdk.ModifierType.ControlMask))
                && this.Selection.PathIsSelected(target))
            {
                this.Selection.SelectFunction = (new TreeSelectionFunc(delegate { return false; }));
                deferSelect = target;
            }
            
        }

        public void onButtonRelease(object obj, ButtonReleaseEventArgs args)
        {
            this.Selection.SelectFunction = (new TreeSelectionFunc(delegate { return true; }));
            TreePath target;
            TreeViewColumn column;
            this.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out target, out column);
            if ((deferSelect != null)
                && deferSelect.Equals(target)
                && !(args.Event.X == 0 && args.Event.Y == 0)
                )
            {
                //Not dragging
                SetCursor(target, column, false);
                Selection.SelectPath(target);
            }
            else if (!(args.Event.State.HasFlag(Gdk.ModifierType.ControlMask))
                && !(args.Event.State.HasFlag(Gdk.ModifierType.ShiftMask)))
            {
                //get above or below
                int xOffset, yOffset, width, height;
                // moveList(this, target, args.Event.Y);
            }

            deferSelect = null;
        }

        public virtual void moveList(TreeView view, TreePath target, double y)
        {

        }


        
        [GLib.ConnectBefore]
        private void DragEndHandler(object o, DragEndArgs args)
        {
            Console.WriteLine("DragEndHandler");
        }

        [GLib.ConnectBefore]
        public void drag(object obj, DragBeginArgs args)
        {
            TreeModel model;
            var paths = this.Selection.GetSelectedRows(out model);
            if (paths.Length > 0)
            {
                var icons = paths.Select(x => this.CreateRowDragIcon(x));
                int height = (icons.Select(x => PixmapSizeGetHelper(x).Item2).Sum() - 2 * icons.Count()) + 2;
                int width = icons.Select(x => PixmapSizeGetHelper(x).Item1).Max();
                var final = new Pixmap(this.GdkWindow, width, height);
                var gc = new Gdk.GC(final);
                gc.Copy(this.Style.ForegroundGC(StateType.Normal));
                gc.Colormap = this.GdkWindow.Colormap;

                int count_y = 1;
                var size = PixmapSizeGetHelper(icons.First());
                foreach (Pixmap icon in icons)
                {
                    size = PixmapSizeGetHelper(icon);
                    final.DrawDrawable(gc, icon, 1, 1, 1, count_y, size.Item1 - 2, size.Item2 - 2);
                    count_y += (size.Item2 - 2);
                }

                Gtk.Drag.SetIconPixmap(args.Context, GdkWindow.Colormap, final, null, 0, 0);
            }
        }

        private Tuple<int, int> PixmapSizeGetHelper(Pixmap pix)
        {
            int width, height;
            pix.GetSize(out width, out height);
            return new Tuple<int, int>(width, height);
        }

    }
}
