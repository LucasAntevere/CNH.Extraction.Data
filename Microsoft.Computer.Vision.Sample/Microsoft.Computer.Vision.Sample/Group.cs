using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Computer.Vision.Sample
{
    public class Group
    {
        public Group()
        {
            Groups = new List<Group>();
        }

        public bool IsLine { get; set; }
        public string Text { get; set; }
        public string BoundingBox { get; set; }
        public List<Group> Groups { get; set; }
        public string Line
        {
            get
            {
                if (Groups.Count == 0 || !IsLine)
                    return Text;

                var s = new StringBuilder();

                for (int i = 0; i < Groups.Count; i++)
                {
                    var first = Groups[i];
                    
                    s.Append(first.Line);
                }

                return s.ToString();
            }
        }
        public List<int> Bounds
        {
            get
            {
                return BoundingBox.Split(',').Select(x => int.Parse(x.ToString())).ToList();
            }
        }
        public int TopLeftX { get { return Bounds[0]; } }
        public int TopLeftY { get { return Bounds[1]; } }
        public int BottomRightX { get { return TopLeftX + Bounds[2]; } }
        public int BottomRightY { get { return TopLeftY + Bounds[3]; } }
    }
}
