﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces.Effects
{
    internal interface ICameraMover
    {
        public bool IsBlockingMoving { get; set; }
    }
}
