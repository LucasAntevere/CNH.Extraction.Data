using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Computer.Vision.Sample
{
    public class OCRResult
    {
        public string language { get; set; }
        public decimal textAngle { get; set; }
        public string orientation { get; set; }
        public List<Region> regions { get; set; }
    }

    public class Region : BoundingBox
    {
        public List<Line> lines { get; set; }
    }

    public class Line : BoundingBox
    {
        public List<Word> words { get; set; }
        [JsonIgnore]
        public string line
        {
            get
            {
                if (words == null || words.Count == 0)
                    return string.Empty;

                return words.Aggregate(new StringBuilder(), (s, w) => s.Append(w.text + " ")).ToString();
            }
        }

        [JsonIgnore]
        public string lineWithoutSpaces
        {
            get
            {
                return line.Replace(" ", string.Empty);
            }
        }

        public List<Line> Match(Regex reg)
        {
            var result = new List<Line>();

            foreach (var match in reg.Matches(line))
            {
                var l = new Line();
                l.boundingBox = boundingBox;

                var s = match.ToString();

                var index = line.IndexOf(s);
                var spaces = s.Count(x => x == ' ');
                spaces++;

                var spacesBefore = line.Substring(0, index).Count(x => x == ' ');
                l.words = words.Skip(spacesBefore).Take(spaces).ToList();

                result.Add(l);
            }

            return result;
        }
    }

    public class Word : BoundingBox
    {
        public string text { get; set; }
    }

    public class BoundingBox
    {
        public string boundingBox { get; set; }
        [JsonIgnore]
        public List<int> Bounds
        {
            get
            {
                return boundingBox.Split(',').Select(x => int.Parse(x.ToString())).ToList();
            }
        }
        [JsonIgnore]
        public int TopLeftX { get { return Bounds[0]; } }
        [JsonIgnore]
        public int TopLeftY { get { return Bounds[1]; } }
        [JsonIgnore]
        public int BottomRightX { get { return TopLeftX + Bounds[2]; } }
        [JsonIgnore]
        public int BottomRightY { get { return TopLeftY + Bounds[3]; } }
    }
}
