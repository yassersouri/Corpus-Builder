using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentenceRecognizer
{
    public class VerbPlusValency : IComparable
    {
        public string VerbString { set; get; }
        public List<ValencyFrame> ValencyFrameList { set; get; }
        public int NumOfBaseStructures { set; get; }

        public VerbPlusValency(string vStr, List<ValencyFrame> valencyFrames)
        {
            NumOfBaseStructures = 0;
            VerbString = vStr;
            ValencyFrameList = valencyFrames;
        }
        public void AddNewFrame(ValencyFrame frame)
        {
            if (!ValencyFrameList.Contains(frame))
                ValencyFrameList.Add(frame);
        }

        public int CompareTo(object obj)
        {
            var newObj = (VerbPlusValency)obj;
            return VerbString.CompareTo(newObj.VerbString);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            return VerbString.Equals(obj);

        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return VerbString.GetHashCode();
        }
    }
}
