﻿using System.Collections.Generic;

namespace SharpBopomofo
{
    public class PhraseBopomofoDictionary : Dictionary<string, PhraseData>
    {
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var target = obj as PhraseBopomofoDictionary;
            if (target == null)
                return false;
            if (ReferenceEquals(this, target))
                return true;
            if (Count != target.Count)
                return false;
            foreach (var key in Keys)
            {
                if (!target.ContainsKey(key))
                    return false;
                if (!this[key].Equals(target[key]))
                    return false;
            }
            return true;
        }
    }
}
