using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Game.Integration
{
    public interface IDamageable
    {
        void TakeDamage(int damage);
    }
}
