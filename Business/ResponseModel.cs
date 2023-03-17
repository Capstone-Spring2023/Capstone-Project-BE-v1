using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class ResponseModel
    {

        public int StatusCode { get; set; }
        public object Data { get; set; }

        public ResponseModel(int statusCode, object data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        public ResponseModel(int statusCode)
        {
            StatusCode = statusCode;
        }
 
        public ResponseModel() { }

    }
    
}
