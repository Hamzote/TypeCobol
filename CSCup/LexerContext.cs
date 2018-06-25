﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUVienna.CS_CUP;

namespace CSCup
{
    /// <summary>
    /// Structure used to store the Lexer current Context before a file inclusion.
    /// https://github.com/TypeCobolTeam/TypeCobol/issues/1000
    /// </summary>
    public struct LexerContext
    {
        /// <summary>
        /// The Current File Path.
        /// </summary>
        public string FilePath;
        public TextReader In;
        /// <summary>
        /// The Current Line
        /// </summary>
        public int CurrentLine;
        /// <summary>
        /// The Current column position
        /// </summary>
        public int CurrentPosition;
        /// <summary>
        /// First character of lookahead.
        /// </summary>
        public int NextChar;
        /// <summary>
        /// Second character of lookahead.
        /// </summary>
        public int NextChar2;
        /// <summary>
        /// Third character of lookahead.
        /// </summary>
        public int NextChar3;
        /// <summary>
        /// 4th character of lookahead.
        /// </summary>
        public int NextChar4;

        /// <summary>
        /// Handle the #use directive.
        /// the syntax is : #use filepath        .
        /// Every thing trimmed after the #use directive will be considered has the file path to be included.
        /// </summary>
        /// <param name="use">The File path to use</param>
        /// <returns>The next Lexer symbol</returns>
        public static TUVienna.CS_CUP.Runtime.Symbol HandleUseDirective(string use)
        {
            if (use == null)
            {
                return lexer.next_token();
            }
            System.IO.StreamReader sr = null;
            try
            {
                FileStream fs = new FileStream(use, FileMode.Open);
                sr = new System.IO.StreamReader(fs);
            }
            catch (Exception e)
            {
                lexer.emit_error("Fail to use file '" + use + "' : " + e.Message);
                return lexer.next_token();
            }
            lexer.SwitchLexerContext(use, sr);
            return lexer.next_token();
        }
    }
}
