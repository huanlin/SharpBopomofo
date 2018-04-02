using System.Collections.Generic;
using ProtoBuf;

namespace SharpBopomofo
{
    [ProtoContract]
    public class PhraseData
    {      
        [ProtoMember(1)]
        public int Frequency { get; set; } = 0;

        [ProtoMember(2)]
        public List<string> BopomofoList { get; private set; }

        public PhraseData()
        {
            Frequency = 0;
            BopomofoList = new List<string>();
        }

        public PhraseData(int frequency, List<string> bopomofoList)
        {
            Frequency = frequency;
            BopomofoList = bopomofoList;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as PhraseData;
            if (target == null)
                return false;

            if (Frequency != target.Frequency || BopomofoList.Count != target.BopomofoList.Count)
                return false;

            for (int i = 0; i < BopomofoList.Count; i++)
            {
                if (BopomofoList[i].Equals(target.BopomofoList[i]) == false)
                    return false;
            }
            return true;
        }
    }

}
