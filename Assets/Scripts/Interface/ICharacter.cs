using System.Threading.Tasks;
using GameData.Item;

namespace Interface
{
    
    public interface ICharacter
    {

        void Damage(int damage,ElementType eType, bool criticalFlag);
        bool UseItem(ItemData item);
    }
}
