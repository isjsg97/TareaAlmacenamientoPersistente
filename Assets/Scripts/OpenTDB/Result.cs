using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTDB
{
    public class Result<T>
    {
        public T Data { get; set;  }
        public bool IsCompleted { get; set; }
    }
}
