using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_API.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data)
        {
            Succeed = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }
        public T Data { get; set; }
        public bool Succeed { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
    }
}