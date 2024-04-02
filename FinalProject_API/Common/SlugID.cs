using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject_API.Common
{
    public class SlugID
    {
        public static string New()
        {
            return Guid.NewGuid().ToByteArray().ToBase36String();
        }
    }
}
