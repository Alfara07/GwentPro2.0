using System;
using System.Collections.Generic;

namespace Compiler
{
    public class CardCreator
    {
        public Dictionary<string, string> StringValue = new();
        public Dictionary<string, string> NumberValue = new();
        public Dictionary<string, string> BooleanValue = new();
        public List<LogError> error = new();
        readonly Options options = new();

        //Método para limpiar el Compiler
        public void CleanCompiler()
        {
            error.Clear();
            StringValue.Clear();
            NumberValue.Clear();
            BooleanValue.Clear();
        }
        
        //Método para comprobar el cierre de la línea
        public bool EndVerification(string Code, char end)
        {
            if (Code[^1] == end)
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
                if (Code[a] != ' ')
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
        private string VariableValue(Dictionary<string, string> Variables, string Var)
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

        //Método para el desarrollo de operaciones aritmética
        public string ArithmeticOperations(string Code, int Nline)
        {
            string[] sum, rest, mul, div, pot;
            Code = RemoveSpace(Code);
            bool number;

            //Verifica si es un número
            if (options.LexicalVerification(Code) == VariableClass.Number)
            {
                return Code;
            }

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
    }
}
