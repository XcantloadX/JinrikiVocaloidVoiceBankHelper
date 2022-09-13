using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinrikiVocaloidVBHelper.Core
{
    /// <summary>
    /// 素材库里的单个音频
    /// </summary>
    public class LibraryAudio
    {
        public List<string> Tags { get; private set; }
        public string Path { get; set; }

    }
}
