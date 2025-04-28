using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IA : MonoBehaviour
{
    System.Diagnostics.Process process = null;
    public static int level = 0;
    string lastFEN;
    public static Dictionary<int, int> IA_Level = new Dictionary<int, int>()
    {
        {0, 1},
        {1, 2}
    };

    public static Dictionary<int, int> IA_Game_Level = new Dictionary<int, int>()
    {
        {1, 1},
        {2, 2}
    };

    public void Setup()
    {
        process = new System.Diagnostics.Process();
        
        if(level == 2) 
        {
            process.StartInfo.FileName = Directory.GetParent(Application.dataPath) + "/main.exe";
            //process.StartInfo.FileName = (Application.dataPath) + "/AI/main.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
        }
        else if(level == 1)
        {
            process.StartInfo.FileName = Directory.GetParent(Application.dataPath) + "/Makoto_1.exe";
            //process.StartInfo.FileName = (Application.dataPath) + "/AI/Makoto_1.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
        }
        Debug.Log(level);
        Debug.Log(process.StartInfo.FileName);
        lastFEN = GetFEN();
    }

    public void Close()
    {
        if(level == 1 || level == 2) 
        {
            process.StandardInput.WriteLine("exit");
        }
        process.Close();
    }

    public string GetBestMove()
    {
        if (level == 1 || level == 2) 
        {
            string setupString = lastFEN;
            process.StandardInput.WriteLine(setupString);

            string bestMoveInAlgebraicNotation = "";
            do
            {
                bestMoveInAlgebraicNotation = process.StandardOutput.ReadLine();
            } while (!bestMoveInAlgebraicNotation.Contains("bestmove"));

            bestMoveInAlgebraicNotation = bestMoveInAlgebraicNotation.Substring(9, 4);
            return bestMoveInAlgebraicNotation;
        }
        else 
        {
            string setupString = "position fen " + lastFEN;
            process.StandardInput.WriteLine(setupString);

            // Process for 5 seconds
            string processString = "go movetime 1";

            // Process deep
            //string processString = "go depth 1";

            process.StandardInput.WriteLine(processString);

            string bestMoveInAlgebraicNotation = "";
            do
            {
                bestMoveInAlgebraicNotation = process.StandardOutput.ReadLine();
            } while (!bestMoveInAlgebraicNotation.Contains("bestmove"));

            bestMoveInAlgebraicNotation = bestMoveInAlgebraicNotation.Substring(9, 4);

            return bestMoveInAlgebraicNotation;
        }
    }

    public string GetFEN()
    {
        process.StandardInput.WriteLine("d");
        string output = "";
        do
        {
            output = process.StandardOutput.ReadLine();
        }
        while (!output.Contains("Fen"));
        output = output.Substring(5);
        return output;
    }

    public void setIAmove(string move)
    {   
        
            string setupString = "position fen " + lastFEN + " moves " + move;
            process.StandardInput.WriteLine(setupString);
            lastFEN = GetFEN();

    }
}
