using Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Compiler
{
    //Valores que recibirá el compilador 
    public class CardAdministrator
    {
        public string Name;
        public CardClass Type;
        public string Faction;
        public int Power;
        public List<AttackClass> AttackClasses;
        public effect effect;
        public int TriggerPlayer;
        public CardAdministrator(string name, CardClass type, string faction, int power, List<AttackClass> attackClasses, int triggerPlayer)
        {
            Name = name;
            Type = type;
            Faction = faction;
            Power = power;
            AttackClasses = attackClasses;
            TriggerPlayer = triggerPlayer;  
        }
    }
}