using System.Collections.Generic;

namespace sheep
{
    public abstract class Entity
    {
        // 唯一 id
        public long id;
        
        public abstract IEnumerable<Intent> tick(SheepWorld world);
    }
}