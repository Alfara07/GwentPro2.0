using Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Compiler
{
    public class CardManager
    {
        //Valores que recibirá el compilador 
        public class CardAdministrator
        {
            public string Name;
            public CardClass Type;
            public string Faction;
            public int Power;
            public List<AttackClass> AttackClasses;
            public string OnActivation;

            public CardAdministrator(string name, CardClass type, string faction, int power, List<AttackClass> attackClasses, string onactivation)
            {
                Name = name;
                Type = type;
                Faction = faction;
                Power = power;
                AttackClasses = attackClasses;
                OnActivation = onactivation;
            }
        }
    }
}