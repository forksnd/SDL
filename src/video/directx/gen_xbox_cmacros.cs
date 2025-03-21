/*
  Simple DirectMedia Layer
  Copyright (C) 1997-2025 Sam Lantinga <slouken@libsdl.org>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

// Build and run this any time you update d3d12.h/d3d12sdklayers.h!

using System.IO;

class Program
{
    static void GenMacros(string[] input, StreamWriter output)
    {
        for (int i = 0; i < input.Length; i += 1)
        {
            if (input[i].StartsWith("#define I"))
            {
                // Strip out the bad ABI calls, use D3D_CALL_RET instead!
                if (input[i].Contains("_GetDesc(") ||
                    input[i].Contains("_GetDesc1(") ||
                    input[i].Contains("_GetCPUDescriptorHandleForHeapStart(") ||
                    input[i].Contains("_GetGPUDescriptorHandleForHeapStart(") ||
                    input[i].Contains("_GetResourceAllocationInfo(") ||
                    input[i].Contains("_GetResourceAllocationInfo1(") ||
                    input[i].Contains("_GetResourceAllocationInfo2(") ||
                    input[i].Contains("_GetResourceAllocationInfo3(") ||
                    input[i].Contains("_GetCustomHeapProperties(") ||
                    input[i].Contains("_GetAdapterLuid(") ||
                    input[i].Contains("_GetLUID(") ||
                    input[i].Contains("_GetProgramIdentifier(") ||
                    input[i].Contains("_GetNodeID(") ||
                    input[i].Contains("_GetEntrypointID("))
                {
                    // May as well skip the next line...
                    i += 1;
                    continue;
                }

                // The first line is fine as-is.
                output.WriteLine(input[i]);

                // The second line, however...
                i += 1;

                string notThis;
                if (input[i].LastIndexOf("This,") > -1)
                {
                    // Function with arguments
                    notThis = "This,";
                }
                else
                {
                    // Function with no arguments
                    notThis = "This";
                }

                int lastNotThis = input[i].LastIndexOf(notThis);
                string alias = input[i].Substring(0, lastNotThis).Replace("lpVtbl -> ", "");
                string definition = input[i].Substring(lastNotThis).Replace(notThis, "");
                output.WriteLine(alias + definition);
            }
        }
    }

    static void Main(string[] args)
    {
        using (FileStream SDL_d3d12_xbox_cmacros_h = File.OpenWrite("SDL_d3d12_xbox_cmacros.h"))
        using (StreamWriter output = new StreamWriter(SDL_d3d12_xbox_cmacros_h))
        {
            output.WriteLine("/* This file is autogenerated, DO NOT MODIFY */");
            GenMacros(File.ReadAllLines("d3d12.h"), output);
            GenMacros(File.ReadAllLines("d3d12sdklayers.h"), output);
        }
    }
}
