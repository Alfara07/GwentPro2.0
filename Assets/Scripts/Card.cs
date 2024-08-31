using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Card
    {
        public string Name;
        public CardClass Type;
        public string Faction;
        public int Power;
        public List<AttackClass> AttackType;
        public int TriggerPlayer;
        
        public Card(string name, CardClass type, string faction, int power,List<AttackClass> attackType,int trigerPlayer)
        {
            Name = name;                   
            Type = type;
            Faction = faction;
            Power = power;
            AttackType = attackType;
            TriggerPlayer = trigerPlayer;
        }
    }
    
}
