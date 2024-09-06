using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Compiler
{
    public class CardCreator
    {
        public Dictionary<string, string> StringValue = new();
        public Dictionary<string, string> NumberValue = new();
        public Dictionary<string, string> BooleanValue = new();
        public List<LogError> error = new();
        readonly Options options = new();

        //Función madre, revisa la sintaxis en general
        public void SintaxCheck(string Code)
        {
            string[] Lines = Code.Split('\n');
            int NLine = 1;
            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = RemoveSpace(Lines[i]);
            }
            foreach (string Line in Lines)
            {
                if (Regex.IsMatch(Line, @"^string|<#texto\s+[a-z](1,15)(\s+:\s+[a-z](1,15)')*;$"))
                {
                    StringMaker(Line, NLine);
                }
                else if (Regex.IsMatch(Line, @"^number|<#real\s+[a-z](1,15)(\s+:\s+\d(0,32000))*;$"))
                {
                    NumberMaker(Line, NLine);
                }
                if (Regex.IsMatch(Line, @"^bool|<#boolean\s+[a-z](1,15)(\s+:\s+(true|false))*;$"))
                {
                    BoolMaker(Line, NLine);
                }
                if (Regex.IsMatch(Line, @"^card|<#definition\s+['{']*;$"))
                {
                    CardCreate(Lines, NLine);
                }
                if (Regex.IsMatch(Line, @"^effect|<#definition\s+['{']*;$"))
                {
                    EffectCreate(Lines, NLine);
                }
                if (error.Count != 0)
                {
                    break;
                }
                NLine++;
            }
        }

        //Método para limpiar el Compiler
        public void CleanCompiler()
        {
            error.Clear();
            StringValue.Clear();
            NumberValue.Clear();
            BooleanValue.Clear();
        }

        //Método que crea variables tipo number
        public void NumberMaker(string Line, int Nline)
        {
            if (EndVerification(Line, ';'))
            {
                if (Exist(Line, "="))
                {
                    string[] separates = Line.Split('=');
                    string[] check = separates[0].Split(' ');
                    string[] end = separates[1].Split(';');
                    if (check.Length >= 2 && options.LexicalVerification(check[1]) == VariableClass.Var && check[0] == "number")
                    {
                        string num = ArithmeticOperations(end[0], Nline);
                        if (num != " ")
                        {
                            NumberValue[check[1]] = num;
                        }

                    }

                    else
                    {
                        error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
                    }
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORassign));
                }
            }
            else
            {
                error.Add(new LogError(Nline, ErrorClass.ERRORclouse));
            }
        }

        //Mtodo que crea variables de tipo string
        public bool StringMaker(string Code, int Nline)
        {
            if (EndVerification(Code, ';'))
            {
                if (Exist(Code, "="))
                {
                    string[] Sentencias = Code.Split('=');
                    string[] var = Sentencias[0].Split(' ');
                    if (options.LexicalVerification(var[1]) != VariableClass.Var || var[0] != "string") 
                    {
                        error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
                        return false;
                    }
                    string[] declarate = Sentencias[1].Split(';');
                    declarate[0] = RemoveSpace(declarate[0]);
                    if (options.LexicalVerification(declarate[0]) == VariableClass.Var)
                    {
                        declarate[0] = VariableValue(StringValue, declarate[0]);
                    }
                    string notNull = CreateString(declarate[0], Nline);
                    if (notNull != " ")
                    {
                        StringValue[var[1]] = notNull;
                        return true;
                    }
                    else
                    {
                        error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                        return false;
                    }
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORassign));
                    return false;
                }
            }
            else
            {
                error.Add(new LogError(Nline, ErrorClass.ERRORclouse));
                return false;
            }
        }

        //Mtodo que crea variables de tipo bool
        public void BoolMaker(string Line, int Nline)
        {
            if (EndVerification (Line, ';'))
            {
                if (Exist(Line, "="))
                {
                    string[] separavar = Line.Split('=');
                    string[] Verify = separavar[0].Split(' ');
                    string[] ent = separavar[1].Split(';');
                    if (Verify.Length >= 2 && options.LexicalVerification(Verify[1]) == VariableClass.Var && Verify[0] == "bool")
                    {
                        try
                        {
                            bool var;
                            var = bool.Parse(ent[0]);
                            BooleanValue[Verify[1]] = var.ToString();
                        }
                        catch (FormatException)
                        {
                            error.Add(new LogError(Nline, ErrorClass.ERRORvalue));

                        }
                    }
                    else
                    {
                        error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
                    }
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORassign));
                }
            }
            else
            {
                error.Add(new LogError(Nline, ErrorClass.ERRORclouse));
            }
        }

        //Método para crear strings 
        public string CreateString(string Code, int Nline)
        {
            Code = RemoveSpace(Code);
            if (Exist(Code, "@"))
            {
                string[] var = Code.Split('@');
                for (int i = 0; i < var.Length; i++)
                {
                    var[i] = RemoveSpace(var[i]);
                    if (options.LexicalVerification(var[i]) == VariableClass.Var)
                    {
                        var[i] = VariableValue(StringValue, var[i]);
                    }
                }
                if (Exist(Code, "@@"))
                {
                    int space = 0;
                    for (int i = 0; i < var.Length; i++)
                    {
                        if (options.LexicalVerification(var[i]) == VariableClass.String)
                        {
                            space = 0;
                        }
                        if (i < var.Length - 1 && space < 1 && var[i] == "")
                        {
                            var[i] = "' '";
                            space++;
                        }
                    }
                }
                foreach (string s in var)
                {
                    if (options.LexicalVerification(s) != VariableClass.String)
                    {
                        error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                        return " ";
                    }
                }
                string line = "";
                foreach (string s in var)
                {
                    foreach (char c in s)
                    {
                        if (c != '\'')
                        {
                            line += c;
                        }
                    }
                }
                return line;
            }
            else if (options.LexicalVerification(Code) == VariableClass.String)
            {
                string line = "";
                foreach (char c in Code)
                {
                    if (c != '\'')
                    {
                        line += c;
                    }
                }
                return line;
            }
            else
            {
                return " ";
            }
        }

        //Método para comprobar el cierre de la línea
        public bool EndVerification(string Code, char end)
        {
            if (Code.Length >= 1 && Code[^1] == end)
            {
                return true;
            }
            return false;
        }

        //Método para comprobar la coincidencia en una línea
        public bool Exist(string Code, string sentence)
        {
            if (Code.Contains(sentence))
            {
                return true;
            }
            return false;
        }

        //Método para obviar los posibles espacios en blanco delante o detrás de las líneas
        public string RemoveSpace(string Code)
        {
            bool start = false;
            string code = "";
            int a = 0;
            int b = 0;
            for (int i = Code.Length - 1; i >= 0; i--)
            {
                if (Code[i] != ' ')
                {
                    a = i;
                    break;
                }
            }
            foreach (char c in Code)
            {
                if (!start && c != ' ')
                {
                    start = true;
                }
                if (start && b <= a)
                {
                    code += c;
                    b++;
                }
            }
            return code;
        }

        //Método que devuelve el valor de la variable y comprueba su existencia 
        public string VariableValue(Dictionary<string, string> Variables, string Var)
        {
            foreach (var word in Variables.Keys)
            {
                if (Var == word)
                {
                    return Variables[Var];
                }
            }
            return " ";
        }

        //Metodo para los operadores dobles ++ -- 
        public string AddorSubtract(string code)

        {
            string[] num;
            if (Exist(code, "++"))
            {
                num = code.Split("++");
                if (NumberValue.ContainsKey(RemoveSpace(num[0])))
                {
                    NumberValue[RemoveSpace(num[0])] = (int.Parse(NumberValue[RemoveSpace(num[0])]) + 1).ToString();
                    return RemoveSpace(num[0]);
                }
            }
            else if (Exist(code, "--"))
            {
                num = code.Split("++");
                if (NumberValue.ContainsKey(RemoveSpace(num[0])))
                {
                    NumberValue[RemoveSpace(num[0])] = (int.Parse(NumberValue[RemoveSpace(num[0])]) - 1).ToString();
                    return RemoveSpace(num[0]);
                }
            }
            else if (Exist(code, "+="))
            {
                num = code.Split("+=");
                if (NumberValue.ContainsKey(RemoveSpace(num[0])))
                {
                    if (options.LexicalVerification(RemoveSpace(num[1])) == VariableClass.Var) num[1] = VariableValue(NumberValue, num[1]);
                    if (options.LexicalVerification(RemoveSpace(num[1])) == VariableClass.Number)
                    {
                        NumberValue[RemoveSpace(num[0])] = (int.Parse(VariableValue(NumberValue, num[0])) + int.Parse(num[1])).ToString();
                        return RemoveSpace(num[0]);
                    }
                }
            }
            else if (Exist(code, "-="))
            {
                num = code.Split("-=");
                if (NumberValue.ContainsKey(RemoveSpace(num[0])))
                {
                    if (options.LexicalVerification(RemoveSpace(num[1])) == VariableClass.Var) num[1] = VariableValue(NumberValue, num[1]);
                    if (options.LexicalVerification(RemoveSpace(num[1])) == VariableClass.Number)
                    {
                        NumberValue[RemoveSpace(num[0])] = (int.Parse(VariableValue(NumberValue, num[0])) - int.Parse(num[1])).ToString();
                        return RemoveSpace(num[0]);
                    }
                }
            }
            return " ";
        }

        //Método para el desarrollo de operaciones aritmética
        public string ArithmeticOperations(string Code, int Nline)
        {
            string[] sum, rest, mul, div, pot;
            Code = RemoveSpace(Code);
            bool number;
            string incremento = AddorSubtract(Code);
            //Verifica si es un número
            if (options.LexicalVerification(Code) == VariableClass.Number)
            {
                return Code;
            }

            //Verificar operador doble
            else if (AddorSubtract(Code) != " ") return VariableValue(NumberValue, incremento);

            //Verifica si es una variable
            else if (options.LexicalVerification(Code) == VariableClass.Var)
            {
                return VariableValue(NumberValue, Code);
            }

            //Verifica si hay una suma
            else if (Exist(Code, "+"))
            {
                number = true;
                sum = Code.Split('+');
                for(int i = 0; i < sum.Length; i++)
                {
                    sum[i] = RemoveSpace(sum[i]);
                    if (options.LexicalVerification(sum[i]) == VariableClass.Var)
                    {
                        sum[i] = VariableValue(NumberValue, sum[i]);
                    }
                    if (options.LexicalVerification(sum[i]) != VariableClass.Number)
                    {
                        sum[i] = ArithmeticOperations(sum[i], Nline);
                    }
                }
                foreach (string s in sum)
                {
                    if (options.LexicalVerification(s) != VariableClass.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(sum[0]);
                    for (int i = 1; i < sum.Length; i++)
                    {
                        var += float.Parse(sum[i]);
                    }
                    return var.ToString();
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                    return " ";
                }
            }

            //Verifica si hay una resta 
            else if (Exist(Code, "-"))
            {
                number = true;
                rest = Code.Split('-');
                for (int i = 0; i < rest.Length; i++)
                {
                    rest[i] = RemoveSpace(rest[i]);
                    if (options.LexicalVerification(rest[i]) == VariableClass.Var)
                    {
                        rest[i] = VariableValue(NumberValue, rest[i]);
                    }
                    if (options.LexicalVerification(rest[i]) != VariableClass.Number)
                    {
                        rest[i] = ArithmeticOperations(rest[i], Nline);
                    }
                }
                foreach (string s in rest)
                {
                    if (options.LexicalVerification(s) != VariableClass.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(rest[0]);
                    for (int i = 1; i < rest.Length; i++)
                    {
                        var -= float.Parse(rest[i]);
                    }
                    return var.ToString();
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                    return " ";
                }
            }

            //Verifica si hay una multiplicación
            else if (Exist(Code, "*"))
            {
                number = true;
                mul = Code.Split('*');
                for (int i = 0; i < mul.Length; i++)
                {
                    mul[i] = RemoveSpace(mul[i]);
                    if (options.LexicalVerification(mul[i]) == VariableClass.Var)
                    {
                        mul[i] = VariableValue(NumberValue, mul[i]);
                    }
                    if (options.LexicalVerification(mul[i]) != VariableClass.Number)
                    {
                        mul[i] = ArithmeticOperations(mul[i], Nline);
                    }
                }
                foreach (string s in mul)
                {
                    if (options.LexicalVerification(s) != VariableClass.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(mul[0]);
                    for (int i = 1; i < mul.Length; i++)
                    {
                        var *= float.Parse(mul[i]);
                    }
                    return var.ToString();
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                    return " ";
                }
            }

            //Verifica si hay división
            else if (Exist(Code, "/"))
            {
                number = true;
                div = Code.Split('/');
                for (int i = 0; i < div.Length; i++)
                {
                    div[i] = RemoveSpace(div[i]);
                    if (options.LexicalVerification(div[i]) == VariableClass.Var)
                    {
                        div[i] = VariableValue(NumberValue, div[i]);
                    }
                    if (options.LexicalVerification(div[i]) != VariableClass.Number)
                    {
                        div[i] = ArithmeticOperations(div[i], Nline);
                    }
                }
                foreach (string s in div)
                {
                    if (options.LexicalVerification(s) != VariableClass.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(div[0]);
                    for (int i = 1; i < div.Length; i++)
                    {
                        if (float.Parse(div[i]) != 0)
                        {
                            var /= float.Parse(div[i]);
                        }
                        else
                        {
                            error.Add(new LogError(Nline, ErrorClass.ERRORdifferenceZero));
                            return " ";
                        }
                    }
                    return var.ToString();
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                    return " ";
                }
            }

            //Verifica si hay potencia
            else if (Exist(Code, "^"))
            {
                number = true;
                pot = Code.Split('^');
                for (int i = 0; i < pot.Length; i++)
                {
                    pot[i] = RemoveSpace(pot[i]);
                    if (options.LexicalVerification(pot[i]) == VariableClass.Var)
                    {
                        pot[i] = VariableValue(NumberValue, pot[i]);
                    }
                }
                foreach (string s in pot)
                {
                    if (options.LexicalVerification(s) != VariableClass.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    for (int i = pot.Length - 2; i >= 0; i--)
                    {
                        pot[i] = Math.Pow(double.Parse(pot[i]), double.Parse(pot[i + 1])).ToString();
                    }

                    return pot[0];
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                    return " ";
                }
            }

            //Verifica si es un error de entrada
            else
            {
                error.Add(new LogError(Nline, ErrorClass.ERRORvalue));
                return " ";
            }
        }

        //Método que realiza las comparaciones
        public bool Compare(string Code, int Nline)
        {
            string[] numbers;
            if (Exist(Code, "<="))
            {

                numbers = Code.Split("<=");
                numbers[0] = ArithmeticOperations(numbers[0], Nline);
                numbers[1] = ArithmeticOperations(numbers[1], Nline);
                if (numbers[0] != " " && numbers[1] != " ")
                {
                    if (float.Parse(numbers[0]) <= float.Parse(numbers[1])) return true;
                    else return false;
                }
            }
            else if (Exist(Code, ">="))
            {
                numbers = Code.Split(">=");
                numbers[0] = ArithmeticOperations(numbers[0], Nline);
                numbers[1] = ArithmeticOperations(numbers[1], Nline);
                if (numbers[0] != " " && numbers[2] != " ")
                {
                    if (float.Parse(numbers[0]) >= float.Parse(numbers[2])) return true;
                    else return false;
                }
            }
            else if (Exist(Code, ">"))
            {
                numbers = Code.Split('>');
                numbers[0] = ArithmeticOperations(numbers[0], Nline);
                numbers[1] = ArithmeticOperations(numbers[1], Nline);
                if (numbers[0] != " " && numbers[1] != " ")
                {
                    if (float.Parse(numbers[0]) > float.Parse(numbers[1])) return true;
                    else return false;
                }
            }
            else if (Exist(Code, "<"))
            {
                numbers = Code.Split('<');
                numbers[0] = ArithmeticOperations(numbers[0], Nline);
                numbers[1] = ArithmeticOperations(numbers[1], Nline);
                if (numbers[0] != " " && numbers[1] != " ")
                {
                    if (float.Parse(numbers[0]) < float.Parse(numbers[1])) return true;
                    else return false;
                }
            }
            else if (Exist(Code, "=="))
            {
                numbers = Code.Split("==");
                numbers[0] = ArithmeticOperations(numbers[0], Nline);
                numbers[1] = ArithmeticOperations(numbers[1], Nline);
                if (numbers[0] != " " && numbers[1] != " ")
                {
                    if (float.Parse(numbers[0]) == float.Parse(numbers[1])) return true;
                    else return false;
                }
            }
            return false;
        }

        //Método que define las cartas
        public void CardCreate(string[] Code, int Nline)
        {
            string Type = " ";
            string Name = " ";
            string Faction = " ";
            string Power = " ";
            string Range = " ";
            string onActivation = "*";
            if (EndVerification(Code[Nline - 1], '{'))
            {
                while (Code[Nline - 1] != "}")
                {
                    if (Nline + 1 < Code.Length)
                    {
                        Nline++;
                    }
                    else
                    {
                        break;
                    }
                    Code[Nline - 1] = RemoveSpace(Code[Nline - 1]);
                    if (Code[Nline - 1] != "")
                    {
                        string Definition = VerificateCard(Code[Nline - 1], Nline);
                        if (Definition != " ")
                        {
                            string[] var = Definition.Split('-');
                            //Verifica variable Name
                            if (var[0] == "Name" && Name == " ")
                            {
                                Name = var[1];
                            }
                            //Verifica variable Faction
                            else if (var[0] == "Faction" && Faction == " ")
                            {
                                Faction = var[1];
                            }
                            //Verifica variable Type
                            else if (var[0] == "Type" && Type == " ")
                            {
                                Type = var[1];
                            }
                            //Verificar variable Power
                            else if (var[0] == "Power" && Power == " ")
                            {
                                Power = var[1];
                            }
                            //Verifica variable Range
                            else if (var[0] == "Range" && Range == " ")
                            {
                                for (int i = 1; i < var.Length; i++)
                                {
                                    Range += var[i] + "-";
                                }
                            }
                            //Verificar definicion OnActivation
                            else if (Definition == "OnActivation")
                            {
                                if (RemoveSpace(Code[Nline]) == "{")
                                {
                                    Nline++;
                                    onActivation = VerificateOnActivation(Code, Nline);
                                    if (onActivation != " ")
                                    {
                                        string[] on = onActivation.Split('*');
                                        int linesAdd = on[1].Split('.').Length + 9;
                                        Nline += linesAdd;
                                    }
                                }
                                else
                                {
                                    error.Add(new LogError(Nline, ErrorClass.ERRORopenBrace));
                                }
                            }

                            //Comprueba error
                            else
                            {
                                error.Add(new LogError(Nline, ErrorClass.ERRORincorrectDeclaration));
                            }
                        }
                        else
                        {
                            error.Add(new LogError(Nline, ErrorClass.ERRORincorrectDeclaration));
                            return;
                        }
                    }
                }
                if (Code[Nline] != "}")
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORcloseBrace));
                }

                //Crea una carta
                if (Type != " " && Name != " " && Faction != " " && Power != " " && Range != " " && error.Count == 0)
                {
                    Range = RemoveSpace(Range);
                    string card = (Name + "|" + Type + "|" + Faction + "|" + Power + "|" + Range + "|" + onActivation);
                    string d;
                    string decks = File.ReadAllText(Application.dataPath + "/Resources/Setting/PlayDecks.txt");
                    if (File.Exists(Application.dataPath + "/Resources/Decks/" + Faction + ".txt"))
                    {
                        d = File.ReadAllText(Application.dataPath + "/Resources/Decks/" + Faction + ".txt");
                        d += "\n" + card;
                        SaveCard(Faction, d, decks);
                    }
                    else
                    {
                        SaveCard(Faction, card, decks);
                    }
                }
                else
                {
                    error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
                }
            }
            else
            {
                error.Add(new LogError(Nline, ErrorClass.ERRORopenBrace));
            }
        }

        //Metodo para verificar parametros de carta
        public string VerificateCard(string Code, int Nline)
        {
            string[] sintax;
            if (EndVerification(Code, ','))
            {
                if (Exist(Code, ":"))
                {
                    //Verifica si existe una declaracion de array Range
                    if (Regex.IsMatch(Code, @"^Range|<#definition\s+['{']*;$"))
                    {
                        if (Exist(Code, "[") && Exist(Code, "]"))
                        {
                            string arrayAttack = "Range";
                            sintax = Code.Split('[', ']');
                            if (Exist(sintax[1], ","))
                            {
                                sintax = sintax[1].Split(',');
                                if (sintax.Length > 3 || sintax.Length < 1)
                                {
                                    error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                                    return " ";
                                }
                                else
                                {
                                    foreach (string s in sintax)
                                    {
                                         string d;
                                        if (options.LexicalVerification(s) == VariableClass.Var)
                                        {
                                            d = VariableValue(StringValue, s);
                                        }
                                        else if (options.LexicalVerification(s) == VariableClass.String)
                                        {
                                            d = CreateString(s, Nline);
                                        }
                                        else
                                        {
                                            error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                                            return " ";
                                        }
                                        if (options.CheckAttackClass(d) != AttackClass.None && !Exist(arrayAttack, d) && d != " ")
                                        {

                                            arrayAttack += "-" + d;
                                        }
                                        else
                                        {
                                            error.Add(new LogError(Nline, ErrorClass.ERRORincorrectDeclaration));
                                            return " ";
                                        }
                                    }
                                    return arrayAttack;
                                }
                            }
                            else
                            {
                                if (options.LexicalVerification(sintax[1]) == VariableClass.Var)
                                {
                                    sintax[1] = VariableValue(StringValue, sintax[1]);
                                }
                                else if (options.LexicalVerification(sintax[1]) == VariableClass.String)
                                {
                                    sintax[1] = CreateString(sintax[1], Nline);
                                }
                                else
                                {
                                    error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                                    return " ";
                                }
                                if (options.CheckAttackClass(sintax[1]) != AttackClass.None && !Exist(arrayAttack, sintax[1]))
                                {

                                    arrayAttack += "-" + sintax[1];
                                }
                                else
                                {
                                    error.Add(new LogError(Nline, ErrorClass.ERRORincorrectDeclaration));
                                    return " ";
                                }
                            }
                            return arrayAttack;
                        }
                    }
                    //Hace la verificación con variables simples
                    sintax = Code.Split(':', ',');
                    sintax[1] = RemoveSpace(sintax[1]);                   

                    //Verifica la declaración Name
                    if (Regex.IsMatch(Code, @"^Name|<#definition\s+['{']*;$") || Regex.IsMatch(Code, @"^Faction|<#definition\s+['{']*;$"))
                    {
                        if (options.LexicalVerification(sintax[1]) == VariableClass.Var)
                        {
                            sintax[1] = VariableValue(StringValue, sintax[1]);
                        }

                        else if (options.LexicalVerification(sintax[1]) == VariableClass.String)
                        {
                            sintax[1] = CreateString(sintax[1], Nline);
                        }

                        if (sintax[1] != " " && Exist(Code, "Name"))
                        {
                            return "Name-" + sintax[1];
                        }
                        if (sintax[1] != " " && Exist(Code, "Faction"))
                        {
                            return "Faction-" + sintax[1];
                        }
                        else
                        {
                            error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                        }
                    }

                    //Verificacion declaracion Type
                    if (Regex.IsMatch(Code, @"^Type|<#definition\s+['{']*;$"))
                    {
                        if (options.LexicalVerification(sintax[1]) == VariableClass.Var)
                        {
                            sintax[1] = VariableValue(StringValue, sintax[1]);
                        }
                        if (options.LexicalVerification(sintax[1]) == VariableClass.String)
                        {
                            sintax[1] = CreateString(sintax[1], Nline);
                        }
                        if (options.CheckCardClass(sintax[1]) != CardClass.None)
                        {
                            return "Type-" + sintax[1];
                        }
                        else
                        {
                            error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                            return " ";
                        }
                    }

                    //Verificacion declaracion Power
                    if (Regex.IsMatch(Code, @"^Power|<#definition\s+['{']*;$"))
                    {
                        if (options.LexicalVerification(sintax[1]) == VariableClass.Var)
                        {
                            sintax[1] = VariableValue(NumberValue, sintax[1]);
                        }
                        if (options.LexicalVerification(sintax[1]) == VariableClass.String)
                        {
                            sintax[1] = ArithmeticOperations(sintax[1], Nline);
                        }
                        if (Exist(Code, "Power") && sintax[1] != " ")
                        {
                            return "Power-" + sintax[1];
                        }
                        else
                        {
                            error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                            return " ";
                        }
                    }
                }
            }
            //Verificacion onActivation
            else if (EndVerification(Code, '['))
            {
                //Verificacion declaracion Power
                if (Regex.IsMatch(Code, @"^OnActivation|<#definition\s+['{']*;$"))
                {
                    if (Exist(Code, ":"))
                    {
                        if (Code.Split(':').Length == 2)
                        {
                            return "OnActivation";
                        }
                        else
                        {
                            error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                            return " ";
                        }
                    }
                    else
                    {
                        error.Add(new LogError(Nline, ErrorClass.ERRORunspecified));
                        return " ";
                    }
                }
            }
            else
            {
                error.Add(new LogError(Nline, ErrorClass.ERRORassingDeclaration));
            }
            return " ";
        }

        //Método que verifica OnActivation
        public string VerificateOnActivation(string[] Code, int Nline)
        {
            bool cierre = false;
            string[] OnActive = new string[3];

            //Nombre del efecto
            if (Regex.IsMatch(Code[Nline], @"^Effect|<#definition\s+['{']*;$") && Exist(Code[Nline], ":") && EndVerification(Code[Nline], '{'))
            {
                while (RemoveSpace(Code[Nline]) != "}" && !Regex.IsMatch(Code[Nline], @"^Selector|<#definition\s+['{']*;$"))
                {
                    Nline++;
                    //Encontrando nombres
                    if (Regex.IsMatch(Code[Nline], @"^Name|<#definition\s+['{']*;$"))
                    {
                        if (Exist(Code[Nline], ":") && EndVerification(Code[Nline], ','))
                        {
                            string[] sintax = Code[Nline].Split(':', ',');
                            sintax[1] = RemoveSpace(sintax[1]);
                            if (options.LexicalVerification(sintax[1]) == VariableClass.Var)
                            {
                                sintax[1] = VariableValue(StringValue, sintax[1]);
                            }

                            if (options.LexicalVerification(sintax[1]) == VariableClass.String)
                            {
                                sintax[1] = CreateString(sintax[1], Nline);
                            }

                            if (sintax[1] != " ")
                            {
                                if (File.Exists(Application.dataPath + "/Resources/Effects/" + sintax[1] + ".txt"))
                                {
                                    OnActive[0] = sintax[1];

                                }
                            }
                        }
                    }

                    //Encontrando Parametros
                    if (Exist(Code[Nline], ":") && Exist(Code[Nline], ",") && !Regex.IsMatch(Code[Nline], @"^Name|<#definition\s+['{']*;$"))
                    {
                        string[] param = Code[Nline].Split(':', ',');
                        param[1] = RemoveSpace(param[1]);
                        param[0] = RemoveSpace(param[0]);
                        if (options.LexicalVerification(param[0]) == VariableClass.Var)
                        {
                            OnActive[1] += param[0] + ":" + param[1] + ".";
                        }
                    }
                }
                if (RemoveSpace(Code[Nline]) == "}") cierre = true;
            }
            Nline++;

            //Selector
            if (cierre)
            {
                cierre = false;
                if (Regex.IsMatch(Code[Nline], @"^Selector|<#definition\s+['{']*;$") && Exist(Code[Nline], ":") && EndVerification(Code[Nline], '{'))
                {
                    string source = "";
                    string single = "false";
                    string predicate = "";
                    int i = 0;
                    Nline++;
                    while (i < 3)
                    {
                        if (Regex.IsMatch(Code[Nline], @"^Source|<#definition\s+['{']*;$"))
                        {
                            if (Exist(Code[Nline], ":") && EndVerification(Code[Nline], ','))
                            {
                                string[] sintax = Code[Nline].Split(':', ',');
                                sintax[1] = RemoveSpace(sintax[1]);
                                if (options.LexicalVerification(sintax[1]) == VariableClass.Var)
                                {
                                    sintax[1] = VariableValue(StringValue, sintax[1]);
                                }

                                if (options.LexicalVerification(sintax[1]) == VariableClass.String)
                                {
                                    sintax[1] = CreateString(sintax[1], Nline);
                                }

                                sintax[1] = options.CheckSourceClass(sintax[1]);
                                if (sintax[1] != " ")
                                {
                                    source = sintax[1];
                                }
                                else
                                {
                                    error.Add(new LogError(Nline, ErrorClass.ERRORincorrectDeclaration));
                                }
                            }
                        }
                        if (Regex.IsMatch(Code[Nline], @"^Single|<#definition\s+['{']*;$"))
                        {
                            if (Exist(Code[Nline], ":") && EndVerification(Code[Nline], ','))
                            {
                                string[] sintax = Code[Nline].Split(':', ',');
                                sintax[1] = RemoveSpace(sintax[1]);
                                if (options.LexicalVerification(sintax[1]) == VariableClass.Var)
                                {
                                    sintax[1] = VariableValue(StringValue, sintax[1]);
                                }
                                sintax[1] = RemoveSpace(sintax[1]);
                                if (sintax[1] == "false" || sintax[1] == "true")
                                {
                                    single = sintax[1];
                                }
                            }
                        }
                        if (Regex.IsMatch(Code[Nline], @"^Predicate|<#definition\s+['{']*;$"))
                        {
                            if (Exist(Code[Nline], ":") && EndVerification(Code[Nline], ','))
                            {
                                string[] sintax = Code[Nline].Split(':', ',');
                                sintax[1] = RemoveSpace(sintax[1]);
                                if (Exist(sintax[1], "(") && Exist(sintax[1], ")"))
                                {
                                    string[] unit = sintax[1].Split('(', ')');
                                    if (RemoveSpace(unit[1]) == "unit" && Exist(sintax[1], "=>") && Exist(sintax[1], "unit."))
                                    {
                                        predicate = sintax[1].Split('.')[1];
                                    }
                                }
                            }
                        }

                        i++;
                        Nline++;
                    }
                    if (source != "" && predicate != "")
                    {
                        OnActive[2] = source + "." + single + "." + predicate + ".";
                    }
                }

            }
            else
            {
                error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
            }

            if (RemoveSpace(Code[Nline]) == "}" && RemoveSpace(Code[Nline + 1]) == "}" && RemoveSpace(Code[Nline + 2]) == "]")
            {
                //Creando respuesta

                if (OnActive[0] != null && OnActive[2] != null)
                {
                    string code = OnActive[0] + "*" + OnActive[1] + "*" + OnActive[2];
                    return code;
                }

            }
            else
            {
                error.Add(new LogError(Nline + 1, ErrorClass.ERRORcorrectDeclaration));
            }
            return " ";
        }

        //Método que guarda las carta
        public void SaveCard(string Faction, string decksito, string decks)
        {
            StreamWriter sw = new(Application.dataPath + "/Resources/Decks/" + Faction + ".txt");
            if (!Exist(decks, Faction))
            {
                StreamWriter sw2 = new(Application.dataPath + "/Resources/Setting/PlayDecks.txt");
                decks += "-" + Faction;
                sw2.Write(decks);
                sw2.Close();
            }
            sw.Write(decksito);
            sw.Close();
        }

        //Método que define los efectos
        public void EffectCreate(string[] Code, int Nline)
        {
            string Name = " ";
            Dictionary<string, string> Params = new();
            int Line = Nline - 1;

            while (Line < Code.Length)
            {
                if (Regex.IsMatch(Code[Line], @"^Name|<#definition\s+['{']*;$") && Exist(Code[Line], ":") && EndVerification(Code[Line], ','))
                {
                    string[] sintax = Code[Line].Split(':', ',');
                    if (options.LexicalVerification(sintax[1]) == VariableClass.Var) sintax[1] = VariableValue(StringValue, RemoveSpace(sintax[1]));

                    if (options.LexicalVerification(sintax[1]) == VariableClass.String) sintax[1] = CreateString(RemoveSpace(sintax[1]), Nline);

                    if (sintax[1] != " ") Name = sintax[1];
                }

                if (Regex.IsMatch(Code[Line], @"^Params|<#definition\s+['{']*;$") && Exist(Code[Line], ":") && EndVerification(Code[Line], '{'))
                {
                    Line++;
                    Nline++;
                    while (!Regex.IsMatch(Code[Line], @"^}|<#End\s+['{']*;$"))
                    {
                        string[] sintax = Code[Line].Split(':', ',');
                        Params[sintax[0]] = RemoveSpace(sintax[1]);
                        Line++;
                        Nline++;
                    }
                }

                if (Regex.IsMatch(Code[Line], @"^Action|<#definition\s+['{']*;$") && Exist(Code[Line], ":") && EndVerification(Code[Line], '{'))
                {
                    if (Exist(Code[Line], "=>"))
                    {
                        string[] sintax = Code[Line].Split(':', '=');
                        sintax = sintax[1].Split('(', ',', ')');
                        if (RemoveSpace(sintax[1]) == "targets" && RemoveSpace(sintax[2]) == "context")
                        {
                            Nline++;
                            Line++;
                            if (Name != " ") VerificateEffect(Code, Params, Name, Line);
                            else error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
                        }
                        else error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
                    }
                    else error.Add(new LogError(Nline, ErrorClass.ERROReffectAssign));
                }
                Line++;
                Nline++;
            }

        }

        //Método que verifica los parámetros de los effectos
        public void VerificateEffect(string[] Code, Dictionary<string, string> Vars, string Name, int Nline)
        {
            string[] Action = new string[Code.Length - Nline + 1];
            Array.Copy(Code, Nline - 1, Action, 0, Code.Length - Nline + 1);
            string Ordenes = "";
            string function = " ";
            List<string> Local_Param_Cards = new();
            Dictionary<string,string> Local_Param_List = new();
            Dictionary<string,string> Local_Param_Property = new();
            bool ActionCierre = false;
            bool EffectCierre = false;
            bool ForCierre = true;
            Local_Param_Cards.Add("");

            //Eliminar espacios en blanco
            for (int i = 0; i < Action.Length; i++)
            {
                Action[i] = RemoveSpace(Action[i]);
            }

            //Revision sintactica en la declaracion de efectos
            foreach (string code in Action)
            {
                //Revision de instruccion for
                if (Regex.IsMatch(code, @"^for|<#definition\s+['{']*;$"))
                {
                    string[] forIs = code.Split(' ');
                    if (RemoveSpace(forIs[0]) == "for" && RemoveSpace(forIs[1]) == "target" && RemoveSpace(forIs[2]) == "in" && RemoveSpace(forIs[3]) == "targets" && RemoveSpace(forIs[4]) == "{")
                    {
                        Ordenes += "for|Source-";
                        ForCierre = false;
                    }
                    else error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
                }

                //Revision de instruccion while
                else if (Regex.IsMatch(code, @"^while|<#definition\s+['{']*;$") && Exist(code, "(") && Exist(code, ")"))
                {
                    string[] par = code.Split('(', ')');
                    Ordenes += "while|" + par[1] + "-";
                }

                //Verificacion de declaracion de un tipo String
                else if (Regex.IsMatch(code, @"^string|<#texto\s+[a-z](1,15)(\s+:\s+[a-z](1,15)')*;$")) Ordenes += "StrVar|Source|" + code + "-";

                //Verificacion de declaracion de un tipo Number
                else if (Regex.IsMatch(code, @"^number|<#real\s+[a-z](1,15)(\s+:\s+\d(0,32000))*;$")) Ordenes += "NumVar|Source|" + code + "-";

                //Verificacion de declaracion bool 
                else if (Regex.IsMatch(code, @"^bool|<#boolean\s+[a-z](1,15)(\s+:\s+(true|false))*;$")) Ordenes += "BoolVar|Source|" + code + "-";

                //Verificar si se utiliza  o asigna una propiedad de target o context
                else if (Exist(code, "."))
                {
                    //Verificar una asignacion
                    if (Exist(code, "=") && EndVerification(code, ';'))
                    {
                        string[] assing = code.Split('=');
                        string var = RemoveSpace(assing[0]);
                        if (options.LexicalVerification(var) == VariableClass.Var)
                        {
                            assing = assing[1].Split('.', ';');

                            //Asignacion Pop
                            if (assing.Length == 4 && RemoveSpace(assing[0]) == "context")
                            {
                                if (RemoveSpace(assing[2]) == "Pop()" && VerificateContextList(assing[1]) != " ")
                                {
                                    Local_Param_Cards.Add(var);
                                    function = "Pop|" + assing[1] + "|" + var + "|";
                                }
                            }
                            //Asignacion Find
                            if (Exist(code, "Find") && Exist(code, "(") && Exist(code, ")") && Exist(code, "unit.") && Exist(code, "==") && VerificateContextList(assing[1]) != " ")
                            {
                                string[] find = code.Split('(', ')');
                                Local_Param_Cards.Add(var);
                                function = "Find|" + assing[1] + "|" + var + "|" + find[1].Split("unit.")[1];
                            }
                            //Asignacion Pop desde una lista asignada
                            else if (assing.Length == 3 && Local_Param_List.ContainsKey(assing[0]))
                            {
                                if (RemoveSpace(assing[1]) == "Pop()" && Local_Param_List.ContainsKey(assing[0]))
                                {
                                    Local_Param_Cards.Add(var);
                                    function = "Pop|" + Local_Param_List[assing[1]] + "|" + var + "|";
                                }
                                if (Exist(code, "Find") && Exist(code, "(") && Exist(code, ")") && Exist(code, "unit.") && Exist(code, "==") && Local_Param_List.ContainsKey(RemoveSpace(assing[0])))
                                {
                                    string[] find = code.Split('(', ')');
                                    Local_Param_Cards.Add(var);
                                    function = "Find|" + assing[1] + "|" + var + "|" + find[1].Split("unit.")[1];
                                }
                            }
                            //Asignacion de una lista
                            else if (assing.Length > 3 && (VerificateContextList(assing[1] + "." + assing[2]) != " " || (Local_Param_Property.ContainsKey(assing[1] + "." + assing[2]) && VerificateContextList(Local_Param_Property[assing[1] + "." + assing[2]]) != " ")))
                            {
                                //Definir Assignacion
                                if (Local_Param_Property.ContainsKey(assing[1] + "." + assing[2])) Local_Param_List[var] = VerificateContextList(Local_Param_Property[assing[1] + "." + assing[2]]);
                                else Local_Param_List[var] = VerificateContextList(assing[1] + "." + assing[2]);
                                function = "ListAdd|Source";

                            }
                            //Asignacion de una lista sin context.TriggerPlayer
                            else if (assing.Length == 3 && (VerificateContextList(assing[1]) != " " || (Local_Param_Property.ContainsKey(assing[1] + assing[2]) && VerificateContextList(Local_Param_Property[assing[1]]) != " ")))
                            {
                                //Definir Assignacion
                                if (Local_Param_Property.ContainsKey(assing[1])) Local_Param_List[var] = VerificateContextList(Local_Param_Property[assing[1]]);
                                else Local_Param_List[var] = VerificateContextList(assing[1]);
                                function = "ListAdd|Source";
                            }
                            //Asignacion de alguna propiedad target
                            else if (assing.Length == 3 && RemoveSpace(assing[1]) == "Owner" && RemoveSpace(assing[0]) == "target" && Ordenes.Contains("for"))
                            {
                                //Definir Asignacion
                                Local_Param_Property[var] = "context.TriggerPlayer";
                            }
                        }
                    }

                    //Verificar llamada a una funcion de una funcion
                    if ((Exist(code, "(") && Exist(code, ")") || Exist(code, "++") || Exist(code, "--") || Exist(code, "-=") || Exist(code, "+=") || Exist(code, "=")) && Exist(code, ".") && EndVerification(code, ';'))
                    {
                        string[] assign = code.Split('.', ';');
                        string parametro = "";
                        if (Exist(code, "context")) if (assign[2].Split('(', ')').Length >= 2) parametro = (RemoveSpace(code.Split('(', ')')[1]));
                        if (assign[1].Split('(', ')').Length >= 2) parametro = (RemoveSpace(code.Split('(', ')')[1]));
                        //Funciones directas del context
                        if (Local_Param_Cards.Contains(parametro) && assign.Length == 4 && RemoveSpace(assign[0]) == "context" && (VerificateContextList(assign[1]) != " " || VerificateContextList(Local_Param_Property[assign[1]]) != " ")) function = ContextFunction(assign, parametro, VerificateContextList(RemoveSpace(assign[1])));
                        else if (assign.Length == 4 && RemoveSpace(assign[0]) == "context") function = ContextFunction(assign);
                        //Funciones de una variable que contienen una lista del context
                        else if (Local_Param_Cards.Contains(parametro) && assign.Length == 3 && Local_Param_List.ContainsKey(RemoveSpace(assign[0]))) function = ContextFunction(assign, parametro, Local_Param_List[assign[0]], 1);
                        else if (assign.Length == 3 && Local_Param_List.ContainsKey(RemoveSpace(assign[0]))) function = ContextFunction(assign, value: 1, list: Local_Param_List[assign[0]]);
                        //Funciones de operador doble con un target.Power
                        if (Ordenes.Contains("for") && assign.Length == 3 && RemoveSpace(assign[0]) == "target" && RemoveSpace(assign[1]) == "Power++") function = "TargetPowerSum|Source|";
                        else if (Ordenes.Contains("for") && assign.Length == 3 && RemoveSpace(assign[0]) == "target" && RemoveSpace(assign[1]) == "Power--") function = "TargetPowerRest|Source|";
                        else if (Ordenes.Contains("for") && assign.Length == 3 && RemoveSpace(assign[0]) == "target" && assign[1].Contains("Power") && assign[1].Contains("-=")) function = "TargetPowerRestTo|Source|" + assign[1].Split("-=", ';')[1];
                        else if (Ordenes.Contains("for") && assign.Length == 3 && RemoveSpace(assign[0]) == "target" && assign[1].Contains("Power") && assign[1].Contains("+=")) function = "TargetPowerSumTo|Source|" + assign[1].Split("+=", ';')[1];
                        else if (Ordenes.Contains("for") && assign.Length == 3 && RemoveSpace(assign[0]) == "target" && assign[1].Contains("Power") && assign[1].Contains("=")) function = "TargetAssingTo|Source|" + assign[1].Split("=", ';')[1];
                    }

                    if (function != " ")
                    {
                        Ordenes += function + "-";
                        function = " ";
                    }
                    else if (function == " ") error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));

                }
                //Leer instrucciones
                else if (EndVerification(code, '}') && Ordenes.Contains("for") && !ActionCierre && !EffectCierre && !ForCierre)
                {
                    ForCierre = true;
                }
                else if (EndVerification(code, '}') && !ActionCierre && ForCierre)
                {
                    ActionCierre = true;
                }
                else if (EndVerification(code, '}') && ActionCierre && ForCierre && !EffectCierre)
                {
                    EffectCierre = true;
                }
                Nline++;
            }
            string vars = "";
            foreach (string s in Vars.Keys)
            {
                vars += s + "|" + Vars[s] + "^";
            }
            if (ActionCierre && EffectCierre && ForCierre)
            {
                string save = CreateString(Name, Nline) + "&" + vars + "&" + Ordenes;
                StreamWriter sw = new(Application.dataPath + "/Resources/Effects/" + CreateString(Name, Nline) + ".txt");
                sw.Write(save);
                sw.Close();
            }
            else error.Add(new LogError(Nline, ErrorClass.ERRORcorrectDeclaration));
        }

        //Método para verificar listar existentes del context en una linea
        public string VerificateContextList(string Code)
        {
            Code = RemoveSpace(Code);
            if (Code == "TriggerPlayer") return Code;
            if (Code == "Hand") return Code;
            if (Code == "Deck") return Code;
            if (Code == "Graveyard") return Code;
            if (Code == "Field") return Code;
            if (Code == "Board") return Code;
            if (Exist(Code, "(") && Exist(Code, ")"))
            {
                string[] ofPlayer = Code.Split('(', ')');
                if (Exist(RemoveSpace(ofPlayer[0]), "DeckOfPlayer"))
                {
                    if (RemoveSpace(ofPlayer[1]) == "context.TriggerPlayer") return "Deck";
                    else if (options.LexicalVerification(RemoveSpace(ofPlayer[1])) == VariableClass.Var) return "Source";
                }
                if (Exist(RemoveSpace(ofPlayer[0]), "GraveyardOfPlayer"))
                {
                    if (RemoveSpace(ofPlayer[1]) == "context.TriggerPlayer") return "Graveyard";
                    else if (options.LexicalVerification(RemoveSpace(ofPlayer[1])) == VariableClass.Var) return "Source";
                }
                if (Exist(RemoveSpace(ofPlayer[0]), "HandOfPlayer"))
                {
                    if (RemoveSpace(ofPlayer[1]) == "context.TriggerPlayer") return "Hand";
                    else if (options.LexicalVerification(RemoveSpace(ofPlayer[1])) == VariableClass.Var) return "Source";
                }
                if (Exist(RemoveSpace(ofPlayer[0]), "FieldOfPlayer"))
                {
                    if (RemoveSpace(ofPlayer[1]) == "context.TriggerPlayer") return "Field";
                    else if (options.LexicalVerification(RemoveSpace(ofPlayer[1])) == VariableClass.Var) return "Source";
                }
            }
            return " ";
        }

        //Determina la función del context
        public string ContextFunction(string[] var, string param = " ", string list = "", int value = 2)
        {
            if (RemoveSpace(var[value]) == "Pop()") return "Pop|" + list;
            if (RemoveSpace(var[value]) == "Remove(" + param + ")") return "Remove|" + list + "|" + param;
            if (RemoveSpace(var[value]) == "Push(" + param + ")") return "Push|" + list + "|" + param + "|"; ;
            if (RemoveSpace(var[value]) == "Add(" + param + ")") return "Add|" + list + "|" + param + "|";
            if (RemoveSpace(var[value]) == "SendBottom(" + param + ")") return "SendBottom|" + list + "|" + param + "|";
            if (RemoveSpace(var[value]) == "Shufle()") return "Shuffle|" + list;

            return " ";
        }
    }
}