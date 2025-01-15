using GameData.Item;

namespace Interface
{
    
    public interface ICharacter
    {

        void Damage(int damage,bool criticalFlag);
        bool UseItem(ItemParam item);
    }
}
