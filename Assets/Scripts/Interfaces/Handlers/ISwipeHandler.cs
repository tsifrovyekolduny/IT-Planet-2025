using Assets.Scripts.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces.Handlers
{
    public interface ISwipeHandler
    {
        void OnSwipe(SwipeOrientation orientation);
    }
}
