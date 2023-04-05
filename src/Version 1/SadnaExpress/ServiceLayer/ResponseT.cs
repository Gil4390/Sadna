using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer
{
    internal class ResponseT<T> : Response
    {
        public readonly T Value;

        internal ResponseT() : base(){
            Value = default;
        }
        internal ResponseT(T value) : base()
        {
            Value = value;
        }
        internal ResponseT(string message) : base(message)
        {
            Value = default;
        }

    }
}
