using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Compiler;
public class CompilerLector: MonoBehaviour
{
    public TMP_InputField In, Out;

    public void Build()
    {
        CardCreator options = new();

        options.CleanCompiler();
        options.SintaxCheck(In.text);
        Out.text = "";
        if (options.error.Count <= 0)
        {
            Out.text += "Build Succed";

        }

        else
        {
            foreach (LogError lines in options.error)
            {
                Out.text += "Line: " + lines.errorline + ": *" + lines.errorclass + "*" + "\n";
            }
        }

    }
}
