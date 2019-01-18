using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchardCoreTranslateTool
{
    [Serializable]
    public class PoModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Msgctxt { get; set; }

        public string MsgId { get; set; }

        public string MsgStr { get; set; }

        public override string ToString()
        {

            string temp = $"msgctxt \"{this.Msgctxt}\"" + System.Environment.NewLine
                        + $"msgid \"{this.MsgId}\""     + System.Environment.NewLine
                        + $"msgstr \"{this.MsgStr}\""   + System.Environment.NewLine;
            return temp;
        }
    }
}
