using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadlineVisualizer
{
    public class Milestone : NotifyBase
    {
        public Guid ID { get; set; }

        private string _title = "";
        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(); }
        }

        private DateTime _deadline;
        public DateTime Deadline
        {
            get { return _deadline; }
            set { _deadline = value; NotifyPropertyChanged(); }
        }

        private string _notes = "";
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged(); }
        }


        private Color _color = Colors.Lavender;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                ColorHex = value.ToHex();
                NotifyPropertyChanged();
            }
        }

        private string _colorHex;
        public string ColorHex
        {
            get { return _colorHex; }
            set
            {
                if (_colorHex != value)
                {
                    _colorHex = value;
                    Color = Color.FromHex(value);
                }
            }
        }

        public Milestone()
        {
            ID = Guid.NewGuid();
        }
    }
}
