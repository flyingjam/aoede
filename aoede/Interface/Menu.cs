using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace aoede.Interface
{
    class Menu : Toolbar
    {
        Audio.Walkman player;

        ToolButton start = new ToolButton(Stock.MediaPlay);
        ToolButton stop = new ToolButton(Stock.MediaStop);
        ToolButton pause = new ToolButton(Stock.MediaPause);
        ToolButton previous = new ToolButton(Stock.MediaPrevious);
        ToolButton next = new ToolButton(Stock.MediaNext);


        ToolItem slider;
        ToolItem playtime;

        public Menu(Audio.Walkman p)
        {
            player = p;
            slider = WidgetToTool(new VolumeSlider(p));
            playtime = WidgetToTool(new PlaytimeSlider(p));
            playtime.Expand = true;

            ToolbarStyle = ToolbarStyle.Icons;
            Insert(start, 0);
            Insert(stop, 1);
            Insert(pause, 2);
            Insert(previous, 3);
            Insert(next, 4);
            Insert(slider, 5);
            Insert(playtime, 6);
            //AddWidget(slider, 5);
            //AddWidget(playtime, 6);
            ConnectSignals();
            this.HeightRequest = 40;
        }

        private Tuple<int, int> GetSize(Widget widget)
        {
            int width, height;
            widget.GetSizeRequest(out width, out height);
            return new Tuple<int, int>(width, height);
        }

        public void ConnectSignals()
        {
            start.Clicked += StartHandler;
            stop.Clicked += StopHandler;
            pause.Clicked += PauseHandler;
            next.Clicked += NextHandler;
            previous.Clicked += PreviousHandler;
        }

        public void AddWidget(MenuItem widget, int position)
        {
            var item = new ToolItem();
            item.Add((Widget)widget);
            item.WidthRequest = widget.GetWidth();
            Insert(item, position);
        }

        private ToolItem WidgetToTool(MenuItem widget)
        {
            var item = new ToolItem();
            item.Add((Widget)widget);
            item.WidthRequest = widget.GetWidth();
            return item;
        }

        private void StartHandler(object o, EventArgs args)
        {
            player.Play();
        }

        private void StopHandler(object o, EventArgs args)
        {
            player.Stop();
        }

        private void PauseHandler(object o, EventArgs args)
        {
            player.Pause();
        }

        private void NextHandler(object o, EventArgs args)
        {
            player.Next();
        }

        private void PreviousHandler(object o, EventArgs args)
        {
            player.Previous();
        }
    }

    interface MenuItem
    {
        int GetWidth();
    }

    class VolumeSlider : HScale, MenuItem
    {
        Audio.Walkman player;
        public VolumeSlider(Audio.Walkman p) : base(0, 150, 5){
            player = p;
            DrawValue = false;
            WidthRequest = 1;
            this.SetSizeRequest(10, 10);
            this.ValueChanged += VolumeSlider_ValueChanged;
            Value = 100;
            //Value = player.GetVolume();
       }

        private void VolumeSlider_ValueChanged(object sender, EventArgs e)
        {
            player.Volume(this.Value);
        }

        public void MoveSlider(object obj, MoveSliderArgs args)
        {

        }

        public int GetWidth()
        {
            return 80;
        }

    }

    class PlaytimeSlider : HBox, MenuItem
    {
        Audio.Walkman player;
        HScale slider = new HScale(0, 100, .5);
        Label timeLabel = new Label("0:00");

        bool userLock = false;

        uint timeoutID;

        public PlaytimeSlider(Audio.Walkman p) 
        {
            player = p;
            
            slider.DrawValue = false;
            PackStart(timeLabel, false, false, 5);
            PackStart(slider, true, true, 0);
            timeoutID = GLib.Timeout.Add(50, Update);
            slider.MoveSlider += Slider_MoveSlider;
            slider.ButtonPressEvent += PlaytimeSlider_ButtonPressEvent;
            slider.ButtonReleaseEvent += PlaytimeSlider_ButtonReleaseEvent;
            Destroyed += RemoveTimeout;
        }

        [GLib.ConnectBefore]
        private void PlaytimeSlider_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            var length = player.GetCurrentLength();
            var position = length.TotalMilliseconds * (slider.Value / 100);
            player.Seek((int)position);
            userLock = false;
        }

        [GLib.ConnectBefore]
        private void PlaytimeSlider_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            Console.WriteLine("LOCKED");
            userLock = true;
        }

        private void Slider_MoveSlider(object o, MoveSliderArgs args)
        {
            Console.WriteLine("hey{0}", args.Scroll);
        }

        public int GetWidth()
        {
            return 0;
        }
        
        private void RemoveTimeout(object obj, EventArgs args)
        {
            GLib.Source.Remove(timeoutID);
        }

        private bool Update()
        {
            if (!userLock)
            {
                var time = player.GetTime();
                var length = player.GetCurrentLength();
                double position = 0;
                if (length.TotalMilliseconds != 0)
                    position = (time.TotalMilliseconds / length.TotalMilliseconds) * 100;
                else
                    position = -2;
                slider.Value = position;
                timeLabel.Text = time.ToString(@"m\:ss");
            }
            return true;
        }
    }
}
