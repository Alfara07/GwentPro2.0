using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Compiler
{
    //Clase para gestionar los errores
    public class LogError
    {
        public string errorclass;
        public int errorline;

        public LogError(string errorclass, int errorline)
        {
            this.errorclass = errorclass;
            this.errorline = errorline;
        }
    }

    //Variables para definir los posibles errores en la compilación
    public class ErrorClass
    {
        public const string ERRORclouse               = "Error001.Warning: ; not found";
        public const string ERRORcloseDeclaration     = "Error002.Warning: , not found";
        public const string ERRORassign               = "Error003.Warning: = not found";
        public const string ERRORassingDeclaration    = "Error004.Warning: : not found";
        public const string ERRORopenBrace            = "Error005.Warning: { not found";
        public const string ERRORcloseBrace           = "Error006.Warning: } not found";
        public const string ERRORopenBracket          = "Error007.Warning: [ not found";
        public const string ERRORcloseBracket         = "Error008.Warning: ] not found";
        public const string ERRORvalue                = "Error009.Warning: The value is not valid";
        public const string ERRORdifferenceZero       = "Error010.Warning: Division by 0 is not valid";
        public const string ERRORcorrectDeclaration   = "Error011.Warning: The declaration is not corret";
        public const string ERRORunspecified          = "Error012.Warning: The variable is not corret";
        public const string ERRORincorrectDeclaration = "Error013.Warning: That property was used / thats not corret";
    }
}