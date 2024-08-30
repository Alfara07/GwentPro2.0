using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Compiler
{
    public class Options
    {
        List<CardClass> cardclass = new(); 
        List<AttackClass> attackclass = new();
        List<SourceClass> Source = new();

        public Options() 
        {
            //Variables definidas por defecto del programa
            cardclass.Add(CardClass.Lider);
            cardclass.Add(CardClass.Oro);
            cardclass.Add(CardClass.Plata);
            cardclass.Add(CardClass.Aumento);
            cardclass.Add(CardClass.Clima);
            cardclass.Add(CardClass.Despeje);
            cardclass.Add(CardClass.Señuelo);

            attackclass.Add(AttackClass.Melee);
            attackclass.Add(AttackClass.Ranged);
            attackclass.Add(AttackClass.Siege);

            Source.Add(SourceClass.hand);
            Source.Add(SourceClass.otherhand);
            Source.Add(SourceClass.field);
            Source.Add(SourceClass.otherfield);
            Source.Add(SourceClass.deck);
            Source.Add(SourceClass.otherdeck);
            Source.Add(SourceClass.board);
        }

        //Método para verificar la validez de las variables
        public VariableClass LexicalVerification(string Text)
        {
            if (Regex.IsMatch(Text, @"^[\'][a-zA-Z' '0-9]*'"))
            {
                return VariableClass.String;
            }
            else if (Regex.IsMatch(Text, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
            {
                return VariableClass.Var;
            }
            else if (Regex.IsMatch(Text, @"^-?\d+$"))
            {
                return VariableClass.Number;
            }
            else
            {
                return VariableClass.Error;
            }
        }

        //Método para verificar que exite la clase de carta dada
        public CardClass CheckCardClass(string type)
        {
            foreach(CardClass c in cardclass)
            {
                if(c.ToString() == type)
                { return c;}
            }
            return CardClass.None;
        }

        //Método para verificar que exite la clase de ataque dada
        public AttackClass CheckAttackClass(string type)
        {
            foreach(AttackClass c in attackclass)
            {
                if(c.ToString() == type)
                { return c;}
            }
            return AttackClass.None;
        }

        //Método para verificar que exite la clase de posición dada
        public string CheckSourceClass(string type)
        {
            foreach(SourceClass c in Source)
            {
                if (c.ToString() == type)
                { return c.ToString(); }
            }
            return " ";
        }
    }

    //Variables por definir 
    public enum VariableClass
    {
        String,
        Number,
        Var,
        Error
    }
    public enum AttackClass
    {
        Melee,
        Ranged,
        Siege,
        None
    }
    public enum CardClass
    {
        Lider,
        Oro,
        Plata,
        Aumento,
        Clima,
        Despeje,
        Señuelo,
        None
    }
    public enum SourceClass
    {
        hand,
        otherhand,
        field,
        otherfield,
        deck,
        otherdeck,
        board
    }
}