using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public delegate string AstExtractValue(Ast ast);
public static class RevokeObfuscationHelpers
{
    public static IDictionary AstValueGrouper(Ast ast, Type targetType, Dictionary<string, double> workingResult, string checkName, AstExtractValue extractValue)
    {
        // Initialize Dictionary to store final results for this check since we cannot modify a Dictionary over which we are iterating.
        Dictionary<string, double> finalResult = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        // Initialize counter to store Count results for percentage calculations.
        double totalItems = 0;

        // Compute the counts for each specified item.
        foreach (Ast targetAst in ast.FindAll(testAst => targetType.IsAssignableFrom(testAst.GetType()), true))
        {
            // Increment count for totalItems (to be used in next foreach loop for calculating percentages).
            totalItems++;

            // Value extraction filter defined as a delegate so it is specified in the calling function.
            String resultKey = extractValue(targetAst);
            // Add entry to UNKNOWN if new item and not in predefined indexes.
            if  (resultKey != null)
            {
                if  (!workingResult.ContainsKey(resultKey))
                {
                    resultKey = "UNKNOWN";
                }
                // Increment count for current item.
                workingResult[resultKey]++;
            }
        }

        foreach (String workingResultValue in workingResult.Keys)
        {
            // Add Count and Percent to final Dictionary.
            if  (totalItems == 0)
            {
                finalResult[checkName + "_" + workingResultValue + "_Count"] = 0;
                finalResult[checkName + "_" + workingResultValue + "_Percent"] = 0;
            }
            else
            {
                finalResult[checkName + "_" + workingResultValue + "_Count"] = workingResult[workingResultValue];
                finalResult[checkName + "_" + workingResultValue + "_Percent"] = ((double)workingResult[workingResultValue]) * 100 / totalItems;
            }
        }

        // Return final result after sorting as a SortedDictionary.
        return new SortedDictionary<string, double>(finalResult);
    }

    public static IDictionary StringMetricCalculator(List<string> stringList, string checkName)
    {
        // Initialize Dictionary to store final results for this check since we cannot modify a Dictionary over which we are iterating.
        Dictionary<string, double> finalResult = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        // Initialize Dictionary and counter to store Count results.
        // Dictionary is a clone of initializedCharDict so all possible characters are already initialized to zero so attribute columns will be consistent for any evaluated script or command.
        Dictionary<string, double> workingResult = new Dictionary<string, double>(initializedCharDict);
        double totalCharacters = 0;
        double totalSpecialCharacters = 0;

        // Initialize Dictionary and List of integers to store the length of each string in stringList for later calculations (average/maximum/minimum/median/mode/range).
        List<double> stringLengthList = new List<double>();
        Dictionary<double, int> stringLengthDict = new Dictionary<double, int>();

        // Initialize Dictionary and List of doubles and counter to store the density of each string in stringList for later calculations (average/maximum/minimum/median/mode/range).
        List<double> stringDensityList = new List<double>();
        Dictionary<double, int> stringDensityDict = new Dictionary<double, int>();
        double curStringWhitespaceTabCount = 0;
        double totalDensityValues = 0;

        // Initialize Dictionary and List of doubles and counter to store the entropy of each string in stringList for later calculations (average/maximum/minimum/median/mode/range).
        List<double> stringEntropyList = new List<double>();
        Dictionary<double, int> stringEntropyDict = new Dictionary<double, int>();
        Dictionary<char, int> curStringCharDict = new Dictionary<char, int>();
        double totalEntropyValues = 0;

        // Initialize Dictionary and List of doubles and counter to store the percentage of upper-case alpha characters among all alpha characters in each string in stringList for later calculations (average/maximum/minimum/median/mode/range).
        List<double> stringUpperAlphaPercentList = new List<double>();
        Dictionary<double, int> stringUpperAlphaPercentDict = new Dictionary<double, int>();
        double curStringAlphaCharCount = 0;
        double curStringUpperAlphaCharCount = 0;
        double totalUpperAlphaPercentValues = 0;

        // Compute the counts for each specified item.
        foreach (String curString in stringList)
        {
            // Add curString lengths for later calculations (average/maximum/minimum/median/mode/range).
            stringLengthList.Add(curString.Length);

            // Add to dictionary for Mode calculation.
            if  (stringLengthDict.ContainsKey(curString.Length))
            {
                stringLengthDict[curString.Length] = stringLengthDict[curString.Length] + 1;
            }
            else
            {
                stringLengthDict[curString.Length] = 1;
            }

            // Reset curString whitespace and tab counts for later density calculations.
            curStringWhitespaceTabCount = 0;

            // Reset Dictionary for curString character distribution for later entropy calculations.
            curStringCharDict.Clear();

            // Reset curString alpha and upper-alpha character counts for later upper-alpha character percentage calculations.
            curStringAlphaCharCount = 0;
            curStringUpperAlphaCharCount = 0;

            foreach (Char curChar in curString.ToCharArray())
            {
                // Convert current character to a string that is the UTF8 representation of the character.
                // This is to handle non-printable characters, Unicode characters, and to properly both upper- and lower-case versions of the same characters.
                String resultKey = ConvertToEncodedChar(curChar);

                // Increment count for totalCharacters for later percentage calculations.
                totalCharacters++;

                // Increment counter for tab or whitespace for later density calculations.
                switch (curChar)
                {
                    case ' ' : curStringWhitespaceTabCount++;
                        break;
                    case '\t' : curStringWhitespaceTabCount++;
                        break;
                }

                // Increment character frequency for later entropy calculations.
                if  (curStringCharDict.ContainsKey(curChar))
                {
                    curStringCharDict[curChar]++;
                }
                else
                {
                    curStringCharDict[curChar] = 1;
                }

                // Increment counter if curChar is a special character.
                if  (IsSpecialChar(curChar))
                {
                    totalSpecialCharacters++;
                }

                // Increment counter if curChar is an alpha character.
                if  (IsAlphaChar(curChar))
                {
                    curStringAlphaCharCount++;

                    // Increment counter if curChar is an upper-case alpha character.
                    if  (IsUpperAlphaChar(curChar))
                    {
                        curStringUpperAlphaCharCount++;
                    }
                }

                // Increment count for current item.
                workingResult[resultKey]++;
            }

            // Add curString density to list for later calculations (average/maximum/minimum/median/mode/range).
            double curStringDensity = 0;
            if  (curString.Length > 0)
            {
                curStringDensity = (curString.Length - curStringWhitespaceTabCount) / curString.Length;
            }

            stringDensityList.Add(curStringDensity);
            totalDensityValues += curStringDensity;

            // Add to dictionary for Mode calculation.
            if  (stringDensityDict.ContainsKey(curStringDensity))
            {
                stringDensityDict[curStringDensity]++;
            }
            else
            {
                stringDensityDict[curStringDensity] = 1;
            }

            // Calculate entropy for curString.
            double curStringEntropy = GetEntropy(curStringCharDict, curString.Length);

            // Add curString entropy to list for later calculations (average/maximum/minimum/median/mode/range).
            stringEntropyList.Add(curStringEntropy);
            totalEntropyValues += curStringEntropy;

            // Add to dictionary for Mode calculation.
            if  (stringEntropyDict.ContainsKey(curStringEntropy))
            {
                stringEntropyDict[curStringEntropy]++;
            }
            else
            {
                stringEntropyDict[curStringEntropy] = 1;
            }

            // Add curString upper-alpha character percentage (only if there are alpha characters in curString) to list for later calculations (average/maximum/minimum/median/mode/range).
            if  (curStringAlphaCharCount > 0)
            {
                double curStringUpperAlphaCharPercent = curStringUpperAlphaCharCount / curStringAlphaCharCount;

                stringUpperAlphaPercentList.Add(curStringUpperAlphaCharPercent);
                totalUpperAlphaPercentValues += curStringUpperAlphaCharPercent;

                // Add to dictionary for Mode calculation.
                if  (stringUpperAlphaPercentDict.ContainsKey(curStringUpperAlphaCharPercent))
                {
                    stringUpperAlphaPercentDict[curStringUpperAlphaCharPercent]++;
                }
                else
                {
                    stringUpperAlphaPercentDict[curStringUpperAlphaCharPercent] = 1;
                }
            }
        }

        foreach (String workingResultValue in workingResult.Keys)
        {
            // Add Count and Percent to final Dictionary.
            if  (totalCharacters == 0)
            {
                finalResult[checkName + "_CharacterDistribution_" + workingResultValue + "_Count"] = 0;
                finalResult[checkName + "_CharacterDistribution_" + workingResultValue + "_Percent"] = 0;
            }
            else
            {
                finalResult[checkName + "_CharacterDistribution_" + workingResultValue + "_Count"] = workingResult[workingResultValue];
                finalResult[checkName + "_CharacterDistribution_" + workingResultValue + "_Percent"] = ((double)workingResult[workingResultValue]) * 100 / totalCharacters;
            }
        }

        // Add Count and Percent for special characters to final Dictionary.
        if  (totalCharacters == 0)
        {
            finalResult[checkName + "_CharacterDistribution_SpecialCharacterOnly_Count"] = 0;
            finalResult[checkName + "_CharacterDistribution_SpecialCharacterOnly_Percent"] = 0;
        }
        else
        {
            finalResult[checkName + "_CharacterDistribution_SpecialCharacterOnly_Count"] = totalSpecialCharacters;
            finalResult[checkName + "_CharacterDistribution_SpecialCharacterOnly_Percent"] = ((double)totalSpecialCharacters) * 100 / totalCharacters;
        }

        // Add total count of all input strings to final Dictionary.
        finalResult[checkName + "_Count"] = stringLengthList.Count;

        // Add cumulative length of all input strings to final Dictionary.
        finalResult[checkName + "_Length_Total"] = totalCharacters;

        // Calculate length, density and entropy values and add to finalResult.
        finalResult = GetAvgMaxMinMedModRan(finalResult, stringLengthList, totalCharacters, stringLengthDict, checkName + "_Length");
        finalResult = GetAvgMaxMinMedModRan(finalResult, stringDensityList, totalDensityValues, stringDensityDict, checkName + "_Density");
        finalResult = GetAvgMaxMinMedModRan(finalResult, stringEntropyList, totalEntropyValues, stringEntropyDict, checkName + "_Entropy");
        finalResult = GetAvgMaxMinMedModRan(finalResult, stringUpperAlphaPercentList, totalUpperAlphaPercentValues, stringUpperAlphaPercentDict, checkName + "_UpperAlphaPercent");

        // Return final result after sorting as a SortedDictionary.
        return new SortedDictionary<string, double>(finalResult);
    }

    public static Dictionary<string, double> GetAvgMaxMinMedModRan(Dictionary<string, double> finalResult, List<double> inputList, double totalInputs, Dictionary<double, int> inputDictForMode, string checkName)
    {
        // Calculate the average/maximum/minimum/median/mode/range of input values.

        // Set default values if inputList is empty.
        if  (inputList.Count == 0)
        {
            // Add average/maximum/minimum/median/mode/range to final Dictionary.
            finalResult[checkName + "_Average"] = 0;
            finalResult[checkName + "_Maximum"] = 0;
            finalResult[checkName + "_Minimum"] = 0;
            finalResult[checkName + "_Median"] = 0;
            finalResult[checkName + "_Mode"] = 0;
            finalResult[checkName + "_Range"] = 0;
        }
        else
        {
            // Sort all inputList values and calculate average/maximum/minimum/median/mode/range.
            inputList.Sort();

            // Add average/maximum/minimum/median/mode/range to final Dictionary.
            finalResult[checkName + "_Average"] = totalInputs / inputList.Count;
            finalResult[checkName + "_Maximum"] = inputList[inputList.Count-1];
            finalResult[checkName + "_Minimum"] = inputList[0];
            finalResult[checkName + "_Median"] = inputList[inputList.Count / 2];
            finalResult[checkName + "_Mode"] = GetMode(inputDictForMode);
            finalResult[checkName + "_Range"] = inputList[inputList.Count-1] - inputList[0];
        }

        // Return results.
        return finalResult;
    }

    public static double GetEntropy(Dictionary<char, int> charDict, int totalChars)
    {
        // Calculate the entropy of input values.
        double entropy = 0;
        if  (totalChars > 0)
        {
            foreach (char charCountKey in charDict.Keys)
            {
                double curCharPercent = (double)charDict[charCountKey] / totalChars;
                entropy += -curCharPercent * Math.Log(curCharPercent, 2);
            }
        }

        // Return entropy value.
        return entropy;
    }

    public static double GetMode(Dictionary<double, int> inputDictionary)
    {
        // Calculate the mode of input values.
        double modeValue = double.MinValue;
        double maxInputCount = double.MinValue;
        foreach (double inputDictionaryValue in inputDictionary.Keys)
        {
            if  (inputDictionary[inputDictionaryValue] > maxInputCount)
            {
                maxInputCount = inputDictionary[inputDictionaryValue];
                modeValue = inputDictionaryValue;
            }
        }

        // Return mode value.
        return modeValue;
    }

    public static Dictionary<string, double> initializedCharDict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
    {
        // Initialize Dictionary with all possible character key values included in the character frequency analysis in StringMetricCalculator function.
        // This Dictionary will be cloned for each instance of the StringMetricCalculator function for performance purposes.

        // Initialize printable characters.
        {"SPACE_20", 0},
        {"!_21", 0},
        {"\"_22", 0},
        {"#_23", 0},
        {"$_24", 0},
        {"%_25", 0},
        {"&_26", 0},
        {"'_27", 0},
        {"(_28", 0},
        {")_29", 0},
        {"*_2a", 0},
        {"+_2b", 0},
        {",_2c", 0},
        {"-_2d", 0},
        {"._2e", 0},
        {"/_2f", 0},
        {"0_30", 0},
        {"1_31", 0},
        {"2_32", 0},
        {"3_33", 0},
        {"4_34", 0},
        {"5_35", 0},
        {"6_36", 0},
        {"7_37", 0},
        {"8_38", 0},
        {"9_39", 0},
        {":_3a", 0},
        {";_3b", 0},
        {"<_3c", 0},
        {"=_3d", 0},
        {">_3e", 0},
        {"?_3f", 0},
        {"@_40", 0},
        {"A_41", 0},
        {"B_42", 0},
        {"C_43", 0},
        {"D_44", 0},
        {"E_45", 0},
        {"F_46", 0},
        {"G_47", 0},
        {"H_48", 0},
        {"I_49", 0},
        {"J_4a", 0},
        {"K_4b", 0},
        {"L_4c", 0},
        {"M_4d", 0},
        {"N_4e", 0},
        {"O_4f", 0},
        {"P_50", 0},
        {"Q_51", 0},
        {"R_52", 0},
        {"S_53", 0},
        {"T_54", 0},
        {"U_55", 0},
        {"V_56", 0},
        {"W_57", 0},
        {"X_58", 0},
        {"Y_59", 0},
        {"Z_5a", 0},
        {"[_5b", 0},
        {"\\_5c", 0},
        {"]_5d", 0},
        {"^_5e", 0},
        {"__5f", 0},
        {"`_60", 0},
        {"a_61", 0},
        {"b_62", 0},
        {"c_63", 0},
        {"d_64", 0},
        {"e_65", 0},
        {"f_66", 0},
        {"g_67", 0},
        {"h_68", 0},
        {"i_69", 0},
        {"j_6a", 0},
        {"k_6b", 0},
        {"l_6c", 0},
        {"m_6d", 0},
        {"n_6e", 0},
        {"o_6f", 0},
        {"p_70", 0},
        {"q_71", 0},
        {"r_72", 0},
        {"s_73", 0},
        {"t_74", 0},
        {"u_75", 0},
        {"v_76", 0},
        {"w_77", 0},
        {"x_78", 0},
        {"y_79", 0},
        {"z_7a", 0},
        {"{_7b", 0},
        {"|_7c", 0},
        {"}_7d", 0},
        {"~_7e", 0},

        // Initialize non-printable (but common) UTF keys.
        {"NUL_00", 0},
        {"SOH_01", 0},
        {"STX_02", 0},
        {"ETX_03", 0},
        {"EOT_04", 0},
        {"ENQ_05", 0},
        {"ACK_06", 0},
        {"BEL_07", 0},
        {"BS_08" , 0},
        {"TAB_09", 0},
        {"LF_0A" , 0},
        {"VT_0B" , 0},
        {"FF_0C" , 0},
        {"CR_0D" , 0},
        {"SO_0E" , 0},
        {"SI_0F" , 0},
        {"DLE_10", 0},
        {"DC1_11", 0},
        {"DC2_12", 0},
        {"DC3_13", 0},
        {"DC4_14", 0},
        {"NAK_15", 0},
        {"SYN_16", 0},
        {"ETB_17", 0},
        {"CAN_18", 0},
        {"EM_19" , 0},
        {"SUB_1A", 0},
        {"ESC_1B", 0},
        {"FS_1C" , 0},
        {"GS_1D" , 0},
        {"RS_1E" , 0},
        {"US_1F" , 0},
        {"DEL_7F", 0},

        // Initialize specific special whitespaces, dashes and quotes that PowerShell explicitly handles.
        // https://github.com/PowerShell/PowerShell/blob/02b5f357a20e6dee9f8e60e3adb9025be3c94490/src/System.Management.Automation/engine/parser/CharTraits.cs#L7-L26

        // Uncommon whitespace.
        {"SpecialChar_NoBreakSpace_194_160", 0},
        {"SpecialChar_NextLine_194_133", 0},

        // Special dashes.
        {"SpecialChar_EnDash_226_128_147", 0},
        {"SpecialChar_EmDash_226_128_148", 0},
        {"SpecialChar_HorizontalBar_226_128_149", 0},

        // Special single quotes.
        {"SpecialChar_QuoteSingleLeft_226_128_152", 0},
        {"SpecialChar_QuoteSingleRight_226_128_153", 0},
        {"SpecialChar_QuoteSingleBase_226_128_154", 0},
        {"SpecialChar_QuoteReversed_226_128_155", 0},

        // Special double quotes.
        {"SpecialChar_QuoteDoubleLeft_226_128_156", 0},
        {"SpecialChar_QuoteDoubleRight_226_128_157", 0},
        {"SpecialChar_QuoteLowDoubleLeft_226_128_158", 0},

        // Initialize UNKNOWN_UNICODE and UNKNOWN_UTF keys.
        {"UNKNOWN_UNICODE", 0},
        {"UNKNOWN_UTF", 0},
    };


    public static bool IsSpecialChar(char charToCheck)
    {
        // Return true if input charToCheck is a special character. Otherwise return false.
        switch(charToCheck)
        {
            case '\t' : return false;
            case '\n' : return false;
            case ' ' : return false;
            case '0' : return false;
            case '1' : return false;
            case '2' : return false;
            case '3' : return false;
            case '4' : return false;
            case '5' : return false;
            case '6' : return false;
            case '7' : return false;
            case '8' : return false;
            case '9' : return false;
            case 'A' : return false;
            case 'B' : return false;
            case 'C' : return false;
            case 'D' : return false;
            case 'E' : return false;
            case 'F' : return false;
            case 'G' : return false;
            case 'H' : return false;
            case 'I' : return false;
            case 'J' : return false;
            case 'K' : return false;
            case 'L' : return false;
            case 'M' : return false;
            case 'N' : return false;
            case 'O' : return false;
            case 'P' : return false;
            case 'Q' : return false;
            case 'R' : return false;
            case 'S' : return false;
            case 'T' : return false;
            case 'U' : return false;
            case 'V' : return false;
            case 'W' : return false;
            case 'X' : return false;
            case 'Y' : return false;
            case 'Z' : return false;
            case 'a' : return false;
            case 'b' : return false;
            case 'c' : return false;
            case 'd' : return false;
            case 'e' : return false;
            case 'f' : return false;
            case 'g' : return false;
            case 'h' : return false;
            case 'i' : return false;
            case 'j' : return false;
            case 'k' : return false;
            case 'l' : return false;
            case 'm' : return false;
            case 'n' : return false;
            case 'o' : return false;
            case 'p' : return false;
            case 'q' : return false;
            case 'r' : return false;
            case 's' : return false;
            case 't' : return false;
            case 'u' : return false;
            case 'v' : return false;
            case 'w' : return false;
            case 'x' : return false;
            case 'y' : return false;
            case 'z' : return false;
            default : return true;
        }
    }


    public static bool IsAlphaChar(char charToCheck)
    {
        // Return true if input charToCheck is an alpha character [a-zA-Z]. Otherwise return false.
        switch(charToCheck)
        {
            case 'A' : return true;
            case 'B' : return true;
            case 'C' : return true;
            case 'D' : return true;
            case 'E' : return true;
            case 'F' : return true;
            case 'G' : return true;
            case 'H' : return true;
            case 'I' : return true;
            case 'J' : return true;
            case 'K' : return true;
            case 'L' : return true;
            case 'M' : return true;
            case 'N' : return true;
            case 'O' : return true;
            case 'P' : return true;
            case 'Q' : return true;
            case 'R' : return true;
            case 'S' : return true;
            case 'T' : return true;
            case 'U' : return true;
            case 'V' : return true;
            case 'W' : return true;
            case 'X' : return true;
            case 'Y' : return true;
            case 'Z' : return true;
            case 'a' : return true;
            case 'b' : return true;
            case 'c' : return true;
            case 'd' : return true;
            case 'e' : return true;
            case 'f' : return true;
            case 'g' : return true;
            case 'h' : return true;
            case 'i' : return true;
            case 'j' : return true;
            case 'k' : return true;
            case 'l' : return true;
            case 'm' : return true;
            case 'n' : return true;
            case 'o' : return true;
            case 'p' : return true;
            case 'q' : return true;
            case 'r' : return true;
            case 's' : return true;
            case 't' : return true;
            case 'u' : return true;
            case 'v' : return true;
            case 'w' : return true;
            case 'x' : return true;
            case 'y' : return true;
            case 'z' : return true;
            default : return false;
        }
    }


    public static bool IsUpperAlphaChar(char charToCheck)
    {
        // Return true if input charToCheck is an upper-case alpha character [A-Z]. Otherwise return false.
        switch(charToCheck)
        {
            case 'A' : return true;
            case 'B' : return true;
            case 'C' : return true;
            case 'D' : return true;
            case 'E' : return true;
            case 'F' : return true;
            case 'G' : return true;
            case 'H' : return true;
            case 'I' : return true;
            case 'J' : return true;
            case 'K' : return true;
            case 'L' : return true;
            case 'M' : return true;
            case 'N' : return true;
            case 'O' : return true;
            case 'P' : return true;
            case 'Q' : return true;
            case 'R' : return true;
            case 'S' : return true;
            case 'T' : return true;
            case 'U' : return true;
            case 'V' : return true;
            case 'W' : return true;
            case 'X' : return true;
            case 'Y' : return true;
            case 'Z' : return true;
            default : return false;
        }
    }

    public static String ConvertToEncodedChar(Char curChar)
    {
        // For efficiency we will avoid computing UTF8 encoding values for the most commmon printable characters.
        // Any characters not found in the below switch statement will have their UTF8 encoded values computed in the remainder of this function.
        switch(curChar)
        {
            case ' ': return "SPACE_20";
            case '!': return "!_21";
            case '"': return "\"_22";
            case '#': return "#_23";
            case '$': return "$_24";
            case '%': return "%_25";
            case '&': return "&_26";
            case '\'': return "'_27";
            case '(': return "(_28";
            case ')': return ")_29";
            case '*': return "*_2a";
            case '+': return "+_2b";
            case ',': return ",_2c";
            case '-': return "-_2d";
            case '.': return "._2e";
            case '/': return "/_2f";
            case '0': return "0_30";
            case '1': return "1_31";
            case '2': return "2_32";
            case '3': return "3_33";
            case '4': return "4_34";
            case '5': return "5_35";
            case '6': return "6_36";
            case '7': return "7_37";
            case '8': return "8_38";
            case '9': return "9_39";
            case ':': return ":_3a";
            case ';': return ";_3b";
            case '<': return "<_3c";
            case '=': return "=_3d";
            case '>': return ">_3e";
            case '?': return "?_3f";
            case '@': return "@_40";
            case 'A': return "A_41";
            case 'B': return "B_42";
            case 'C': return "C_43";
            case 'D': return "D_44";
            case 'E': return "E_45";
            case 'F': return "F_46";
            case 'G': return "G_47";
            case 'H': return "H_48";
            case 'I': return "I_49";
            case 'J': return "J_4a";
            case 'K': return "K_4b";
            case 'L': return "L_4c";
            case 'M': return "M_4d";
            case 'N': return "N_4e";
            case 'O': return "O_4f";
            case 'P': return "P_50";
            case 'Q': return "Q_51";
            case 'R': return "R_52";
            case 'S': return "S_53";
            case 'T': return "T_54";
            case 'U': return "U_55";
            case 'V': return "V_56";
            case 'W': return "W_57";
            case 'X': return "X_58";
            case 'Y': return "Y_59";
            case 'Z': return "Z_5a";
            case '[': return "[_5b";
            case '\\': return "\\_5c";
            case ']': return "]_5d";
            case '^': return "^_5e";
            case '_': return "__5f";
            case '`': return "`_60";
            case 'a': return "a_61";
            case 'b': return "b_62";
            case 'c': return "c_63";
            case 'd': return "d_64";
            case 'e': return "e_65";
            case 'f': return "f_66";
            case 'g': return "g_67";
            case 'h': return "h_68";
            case 'i': return "i_69";
            case 'j': return "j_6a";
            case 'k': return "k_6b";
            case 'l': return "l_6c";
            case 'm': return "m_6d";
            case 'n': return "n_6e";
            case 'o': return "o_6f";
            case 'p': return "p_70";
            case 'q': return "q_71";
            case 'r': return "r_72";
            case 's': return "s_73";
            case 't': return "t_74";
            case 'u': return "u_75";
            case 'v': return "v_76";
            case 'w': return "w_77";
            case 'x': return "x_78";
            case 'y': return "y_79";
            case 'z': return "z_7a";
            case '{': return "{_7b";
            case '|': return "|_7c";
            case '}': return "}_7d";
            case '~': return "~_7e";
        }

        // Convert curChar to string and then get UTF8 encoding as a string.
        Encoding utfEnc = new UTF8Encoding(true, true);
        Byte[] charBytes = utfEnc.GetBytes(curChar.ToString());

        // Return if curChar is a Unicode character.
        if  (charBytes.Length > 1)
        {
            // Handle specific special whitespaces, dashes and quotes that PowerShell explicitly handles.
            // https://github.com/PowerShell/PowerShell/blob/02b5f357a20e6dee9f8e60e3adb9025be3c94490/src/System.Management.Automation/engine/parser/CharTraits.cs#L7-L26
            switch (String.Join("_", charBytes))
            {
                // Uncommon whitespace.
                case "194_160": return "SpecialChar_NoBreakSpace_194_160";
                case "194_133": return "SpecialChar_NextLine_194_133";

                // Special dashes.
                case "226_128_147": return "SpecialChar_EnDash_226_128_147";
                case "226_128_148": return "SpecialChar_EmDash_226_128_148";
                case "226_128_149": return "SpecialChar_HorizontalBar_226_128_149";

                // Special single quotes.
                case "226_128_152": return "SpecialChar_QuoteSingleLeft_226_128_152";
                case "226_128_153": return "SpecialChar_QuoteSingleRight_226_128_153";
                case "226_128_154": return "SpecialChar_QuoteSingleBase_226_128_154";
                case "226_128_155": return "SpecialChar_QuoteReversed_226_128_155";

                // Special double quotes.
                case "226_128_156": return "SpecialChar_QuoteDoubleLeft_226_128_156";
                case "226_128_157": return "SpecialChar_QuoteDoubleRight_226_128_157";
                case "226_128_158": return "SpecialChar_QuoteLowDoubleLeft_226_128_158";

                default: return "UNKNOWN_UNICODE";
            }
        }

        // Convert char byte to a string.
        String charBytesUtfAsString = String.Format("{0:X2}", charBytes[0]);

        // Convert input char to string to be included in the key in result.
        String charName = curChar.ToString();

        // If char is not included in "printable character" regex then we do not want to use it as a key as it would pollute our resultant data for analysis.
        // For non-printable characters we will replace common characters with TEXT representation of the character's meaning (or UTF8 as the default) in below switch statement.
        switch  (charBytesUtfAsString)
        {
            case "00": return "NUL_00";
            case "01": return "SOH_01";
            case "02": return "STX_02";
            case "03": return "ETX_03";
            case "04": return "EOT_04";
            case "05": return "ENQ_05";
            case "06": return "ACK_06";
            case "07": return "BEL_07";
            case "08": return "BS_08";
            case "09": return "TAB_09";
            case "0A": return "LF_0A";
            case "0B": return "VT_0B";
            case "0C": return "FF_0C";
            case "0D": return "CR_0D";
            case "0E": return "SO_0E";
            case "0F": return "SI_OF";
            case "10": return "DLE_10";
            case "11": return "DC1_11";
            case "12": return "DC2_12";
            case "13": return "DC3_13";
            case "14": return "DC4_14";
            case "15": return "NAK_15";
            case "16": return "SYN_16";
            case "17": return "ETB_17";
            case "18": return "CAN_18";
            case "19": return "EM_19";
            case "1A": return "SUB_1A";
            case "1B": return "ESC_1B";
            case "1C": return "FS_1C";
            case "1D": return "GS_1D";
            case "1E": return "RS_1E";
            case "1F": return "US_1F";
            case "7F": return "DEL_7F";
            default: return "UNKNOWN_UTF";
        }
    }
}