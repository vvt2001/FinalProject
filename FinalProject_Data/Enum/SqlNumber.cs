using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject_Data.Enum
{
    public enum SqlNumber
    {
        [Description("Không thể xóa {0} đang được sử dụng")]
        ConflictFK = 547,
        [Description("{0} đã tồn tại trên hệ thống")]
        DuplicateKey = 2601
    }
}
